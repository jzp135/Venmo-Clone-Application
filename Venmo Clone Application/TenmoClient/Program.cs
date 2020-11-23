using System;
using System.Collections.Generic;
using TenmoClient.Data;
using TenmoServer.Models;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();
        private static readonly AccountService accountService = new AccountService();
        private static readonly TransferService transferService = new TransferService();

        static void Main(string[] args)
        {
            Run();
        }
        private static void Run()
        {
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = consoleService.PromptForLogin();
                        API_User user = authService.Login(loginUser);
                        if (user != null)
                        {
                            UserService.SetLogin(user);
                        }

                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered = false;
                    while (!isRegistered) //will keep looping until user is registered
                    {
                        LoginUser registerUser = consoleService.PromptForLogin();
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                            loginRegister = -1; //reset outer loop to allow choice for login
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }
            
            MenuSelection();
        }

        private static void MenuSelection()
        {
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == 1)
                {
                    double balance = accountService.GetBalance();
                    consoleService.PrintBalance(balance);
                    MenuSelection();
                }
                else if (menuSelection == 2)
                {
                    List<TransferClient> transfers = transferService.GetTransfers();
                    consoleService.PrintTransfers(transfers);
                    int id = -1;
                    while (id == -1)
                    {
                        id = consoleService.PromptForTransferID("view");
                        if (id == 0)
                        {
                            MenuSelection();
                        }
                    }
                    consoleService.PrintTransferDetails(id, transfers);
                    MenuSelection();
                }
                else if (menuSelection == 3)
                {
                    List<TransferClient> transfers = transferService.GetPendingTransfers();
                    consoleService.PrintPendingTransfers(transfers);
                    if (transfers.Count != 0)
                    {
                        int id = -1;
                        while (id == -1)
                        {
                            id = consoleService.PromptForTransferID("approve or reject");
                            if (id == 0)
                            {
                                MenuSelection();
                            }
                        }
                        int newStatus = consoleService.PromptForApproval();
                        if (newStatus == 0)
                        {
                            MenuSelection();
                        }
                        double currentBalance = accountService.GetBalance();
                        TransferClient returnTransfer = transferService.UpdateTransfer(id, transfers, newStatus, currentBalance);
                        if (returnTransfer != null)
                        {
                            consoleService.PrintResult(returnTransfer.Status);
                        }
                    }
                    MenuSelection();
                }
                else if (menuSelection == 4)
                {
                    List<UserDTO> list = accountService.GetUsers();
                    consoleService.PrintUsers(list);
                    int id = -1;
                    while (id == -1)
                    {
                        id = consoleService.PromptForUserId(list, "send to");
                        if (id == 0)
                        {
                            MenuSelection();
                        }
                    }
                    double amount = -1;
                    while (amount == -1)
                    {
                        amount = consoleService.PromptForAmount("send");
                        if (amount == 0)
                        {
                            MenuSelection();
                        }
                    }
                    if (amount > accountService.GetBalance())
                    {
                        Console.WriteLine("\nYou don't have enough money to transfer.");
                        MenuSelection();
                    }
                    TransferClient returnTransfer = transferService.SendTransfer(id, amount);
                    if (returnTransfer != null)
                    {
                        consoleService.PrintResult(returnTransfer.Status);
                    }
                    MenuSelection();
                }
                else if (menuSelection == 5)
                {
                    List<UserDTO> list = accountService.GetUsers();
                    consoleService.PrintUsers(list);
                    int id = -1;
                    while (id == -1)
                    {
                        id = consoleService.PromptForUserId(list, "request from");
                        if (id == 0)
                        {
                            MenuSelection();
                        }
                    }
                    double amount = -1;
                    while (amount == -1)
                    {
                        amount = consoleService.PromptForAmount("request");
                        if (amount == 0)
                        {
                            MenuSelection();
                        }
                    }
                    TransferClient pendingTransfer = transferService.RequestTransfer(id, amount);
                    if (pendingTransfer != null)
                    {
                        consoleService.PrintResult(pendingTransfer.Status);
                    }
                    MenuSelection();
                }
                else if (menuSelection == 6)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new API_User()); //wipe out previous login info
                    Run(); //return to entry point
                }
                else
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }
    }
}
