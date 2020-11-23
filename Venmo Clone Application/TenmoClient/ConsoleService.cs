using System;
using System.Collections.Generic;
using TenmoClient.Data;
using TenmoServer.Models;

namespace TenmoClient
{
    public class ConsoleService
    {

        /// <summary>
        /// Prompts for transfer ID to view, approve, or reject
        /// </summary>
        /// <param name="action">String to print in prompt. Expected values are "Approve" or "Reject" or "View"</param>
        /// <returns>ID of transfers to view, approve, or reject</returns>
        public int PromptForTransferID(string action)
        {
            Console.WriteLine("");
            Console.Write("Please enter transfer ID to " + action + " (0 to cancel): ");
            if (!int.TryParse(Console.ReadLine(), out int actionId))
            {
                Console.WriteLine("Invalid input. Only input a number.");
                return -1;
            }
            else if (actionId == 0)
            {
                Console.WriteLine("Returning to main menu...\n");
                return actionId;
            }
            else
            {
                return actionId;
            }
        }
        /// <summary>
        /// Prompts for a UserId from list (Printed previously). 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="action"></param> String to print in prompt Expected values are "Send to" or "Request from"
        /// <returns>Returns the ID as an int</returns>
        public int PromptForUserId(List<UserDTO> list, string action)
        {
            bool exists = false;
            int result = -1;
            Console.WriteLine("");
            Console.Write("Please enter ID you wish to " + action + "(0 to cancel): ");
            if (!int.TryParse(Console.ReadLine(), out int actionId))
            {
                Console.WriteLine("Invalid input. Only input a number.");
                return result;
            }
            foreach(UserDTO user in list)
            {
                if(user.UserId == actionId)
                {
                    result = user.AccountNumber;
                    if (actionId == UserService.GetUserId())
                    {
                        Console.WriteLine("You can't " + action +  " yourself.");
                        result = -1;
                    }
                    exists = true;
                    break;
                }
            }
            if (actionId == 0)
            {
                Console.WriteLine("Returning to main menu...\n");
                result = actionId;
            }
            else if (!exists) 
            {
                Console.WriteLine("Account does not exist please try again.");
                return result;
            }

            return result;
        }
        //Base Code
        public LoginUser PromptForLogin()
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();
            string password = GetPasswordFromConsole("Password: ");

            LoginUser loginUser = new LoginUser
            {
                Username = username,
                Password = password
            };
            return loginUser;
        }

        private string GetPasswordFromConsole(string displayMessage)
        {
            string pass = "";
            Console.Write(displayMessage);
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Backspace Should Not Work
                if (!char.IsControl(key.KeyChar))
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Remove(pass.Length - 1);
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine("");
            return pass;
        }
        /// <summary>
        /// Prompts the user for the amount of money they wish to send or request.
        /// </summary>
        /// <param name="action"></param> Expected Values are "send" or "request".
        /// <returns>The amount they wish to transfer. If 0, the program should send the user to the main menu.</returns>
        public double PromptForAmount(string action)
        {
            const double transferMax = 10000.00;
            Console.Write($"Please enter amount to {action}. Max amount is $10,000.00 (Press 0 to cancel): ");
            if (!double.TryParse(Console.ReadLine(), out double actionId))
            {
                Console.WriteLine("Invalid input. Only input a number.");
                return -1;
            }
            else if (actionId < 0)
            {
                Console.WriteLine("Invalid input. Please input a positive number.");
                return -1;
            }
            else if (actionId == 0)
            {
                Console.WriteLine("Returning to main menu...\n");
                return 0;
            }
            else if (actionId >= transferMax)
            {
                Console.WriteLine("Invalid input. Please input a number less than 10,000.");
                return -1;
            }
            else
            {
                return actionId;
            }
        }
        /// <summary>
        /// Prompts the user if they would like to approve or reject a specific request. (printed previously)
        /// </summary>
        /// <returns>ints corresponding to database status codes: 2 - Approve, 3 - Reject. 
        /// 0 is used to send users back to main menu. </returns>
        public int PromptForApproval()
        {
            int result = -1;
            const int statusCodeApproved = 2;
            const int statusCodeRejected = 3;
            Console.WriteLine("1: Approve");
            Console.WriteLine("2: Reject");
            Console.WriteLine("0: Don't Approve or Reject");
            Console.WriteLine("---------");
            Console.Write("Please choose an option: ");
            while (result == -1)
            {
                if (!int.TryParse(Console.ReadLine(), out int actionId))
                {
                    Console.WriteLine("Invalid input. Only input a number.");
                    continue;
                }
                else if (actionId == 0)
                {
                    Console.WriteLine("Returning to main menu...\n");
                    result = actionId;
                }
                else if (actionId == 1)
                {
                    result = statusCodeApproved;
                }
                else if (actionId == 2)
                {
                    result = statusCodeRejected;
                }
                else
                {
                    Console.WriteLine("Invalid input. Enter 0, 1, or 2.");
                    continue;
                }
            }

            return result;
        }
        /// <summary>
        /// Prints the users balance passed in.
        /// </summary>
        /// <param name="balance"></param>
        public void PrintBalance(double balance)
        {
            if (balance > 0)
            {
                Console.WriteLine($"\nYour current account balance is: {balance:C}");
            }
            else if (balance == 0)
            {
                Console.WriteLine("Your account is empty. Get out there and request some transfers!");
            }

        }
        /// <summary>
        /// Loops through the transfers list and prints ID, Sender or Receiver, and the Amount of each transfer.
        /// </summary>
        /// <param name="transfers"></param>
        public void PrintTransfers(List<TransferClient> transfers)
        {
            string header = String.Format("{0,-10}{1,-19}{2}","ID", "From/To", "Amount");

            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("Transfers");
            Console.WriteLine(header);
            Console.WriteLine("--------------------------------------------");

            if (transfers.Count == 0)
            {
                Console.WriteLine("\nThere are no transfers to display");
            }
            else
            {
                foreach (TransferClient transfer in transfers)
                {

                    string bodyfrom = String.Format("{0,-10}{1,-6}{2,-13}{3:C}", transfer.ID, "From:", transfer.UsernameFrom, transfer.Amount);
                    string bodyto = String.Format("{0,-10}{1,-6}{2,-13}{3:C}", transfer.ID, "To:", transfer.UsernameTo, transfer.Amount);

                    if (transfer.From == UserService.GetUserId())
                    {
                        Console.WriteLine(bodyto);
                    }
                    else
                    {
                        Console.WriteLine(bodyfrom);
                    }
                }
            }
            Console.WriteLine("\n---------");
        }
        /// <summary>
        /// Loops through the transfers list. Use with GetPendingTransfers() to print only the pending transfers 
        /// with correct menu syntax. e.g. Not saying "From/To" or specifying the same values.
        /// </summary>
        /// <param name="transfers"></param>
        public void PrintPendingTransfers(List<TransferClient> transfers)
        {
            string header = String.Format("{0,-10}{1,-19}{2}", "ID", "To", "Amount");

            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("Pending Transfers");
            Console.WriteLine(header);
            Console.WriteLine("--------------------------------------------");

            if (transfers.Count == 0)
            {
                Console.WriteLine("\nThere are no pending transfers to display");
            }
            else
            {
                foreach (TransferClient transfer in transfers)
                {
                    string bodyto = String.Format("{0,-10}{1,-19}{2:C}", transfer.ID, transfer.UsernameTo, transfer.Amount);
                    Console.WriteLine(bodyto);
                }
            }
            Console.WriteLine("\n---------");
        }

        internal void PrintTransferDetails(int id, List<TransferClient> transfer)
        {
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("Transfer Details");
            Console.WriteLine("--------------------------------------------");
            
            foreach (TransferClient t in transfer)
            {
                if (t.ID == id)
                {
                    Console.WriteLine($"ID: {t.ID}");
                    Console.WriteLine($"From: {t.UsernameFrom}");
                    Console.WriteLine($"To: {t.UsernameTo}");
                    Console.WriteLine($"Type: {t.TypeName}");
                    Console.WriteLine($"Status: {t.StatusName}");
                    Console.WriteLine($"Amount: {t.Amount:C}");
                }
            }
        }
        /// <summary>
        /// Prints message based on the outcome of a transfer. If outcome not one of the 3 expected values, returns rejected.
        /// </summary>
        /// <param name="status"></param> The possible status codes returned by a transfer. Expected values: 1 (pending), 2 (success), 3 (rejected)
        internal void PrintResult(int status)
        {
            if (status == 2)
            {
                Console.WriteLine("Transfer successfully completed.");
            }
            else if (status == 1)
            {
                Console.WriteLine("Request sent. Waiting on approval.");
            }
            else
            {
                Console.WriteLine("Request rejected.");
            }
        }

        /// <summary>
        /// Prints the list of users passed in, with their IDs.
        /// </summary>
        /// <param name="list"></param>
        internal void PrintUsers(List<UserDTO> list)
        {
            string header = String.Format("{0,-10}{1}", "ID", "Name");

            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("Users");
            Console.WriteLine(header);
            Console.WriteLine("--------------------------------------------");

            foreach (UserDTO u in list)
            {
                string body = string.Format("{0,-10}{1}", u.UserId, u.Username);
                Console.WriteLine(body);
            }
        }


    }
}
