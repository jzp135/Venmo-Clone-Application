using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoServer.Models;

namespace TenmoClient
{
    public class TransferService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();
        /// <summary>
        /// Queries the Server for a list of transfers based on the current authorized user.
        /// </summary>
        /// <returns>List of transfers.</returns>
        public List<TransferClient> GetTransfers()
        {
            List<TransferClient> list = new List<TransferClient>();
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            RestRequest request = new RestRequest(API_BASE_URL + "transfers");
            IRestResponse<List<TransferClient>> response = client.Get<List<TransferClient>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return null;
            }
            else if (!response.IsSuccessful)
            {
                Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
          
                return null;
            }

            list = response.Data;
            return list;
        }
        /// <summary>
        /// Updates the databse to represent a transfer from the current user to the specific receiverId provided.
        /// </summary>
        /// <param name="receiverId"></param> ReceiverID cannot be the same as the current user (checks when prompting for receiverID.
        /// <param name="amount"></param>
        /// <returns>Transfer class with all parameters added.</returns>
        public TransferClient SendTransfer(int receiverId, double amount)
        {
            TransferToSend t = new TransferToSend(receiverId, amount);
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            RestRequest request = new RestRequest(API_BASE_URL + "transfers");
            request.AddJsonBody(t);
            IRestResponse<TransferClient> response = client.Put<TransferClient>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                Console.WriteLine(response.StatusCode + response.ErrorMessage);
                return null;
            }
            else if (!response.IsSuccessful)
            {
                Console.WriteLine(response.StatusCode + response.ErrorMessage);
                return null;
            }

            return response.Data;
        }
        /// <summary>
        ///  Queries the Server for a list of pending transfers based on the current authorized user.
        /// </summary>
        /// <returns>List of transfers.</returns>
        public List<TransferClient> GetPendingTransfers()
        {
            List<TransferClient> list = new List<TransferClient>();
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            RestRequest request = new RestRequest(API_BASE_URL + "transfers/pending");
            IRestResponse<List<TransferClient>> response = client.Get<List<TransferClient>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return null;
            }
            else if (!response.IsSuccessful)
            {
                Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);

                return null;
            }

            list = response.Data;
            return list;
        }
        /// <summary>
        /// 1- Matches id to the ID property of a specific transfer, then verifies if the current user has enough money to fulfill a specific transfer.
        /// 2- Changes the status code of the specifice transfer to represent the decision (Approve or Deny) that the user gave.
        /// 3- Queries the Database to transfer the money and update the transfer with the new status code. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="transfers"></param>
        /// <param name="newStatusCode"></param>
        /// <param name="currentBalance"></param>
        /// <returns>Returns the server data as a Transfer object with all properties complete.</returns>
        public TransferClient UpdateTransfer(int id, List<TransferClient> transfers, int newStatusCode, double currentBalance)
        {
            TransferClient pendingTransfer = new TransferClient();
            foreach (TransferClient t in transfers)
            {
                if (id == t.ID)
                {
                    if (t.Amount > currentBalance)
                    {
                        Console.WriteLine("\nYou don't have enough money to fulfill this request.");
                        return null;
                    }
                    pendingTransfer = t;
                    pendingTransfer.Status = newStatusCode;
                    break;
                }
            }
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            RestRequest request = new RestRequest(API_BASE_URL + "transfers/approve_deny");
            request.AddJsonBody(pendingTransfer);
            IRestResponse<TransferClient> response = client.Put<TransferClient>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                Console.WriteLine(response.StatusCode + response.ErrorMessage);
                return null;
            }
            else if (!response.IsSuccessful)
            {
                Console.WriteLine(response.StatusCode + response.ErrorMessage);
                return null;
            }

            return response.Data;
        }
        /// <summary>
        /// Updates the databse to represent a requested transfer from the current user to the specific receiverId provided.
        /// Does not transfer the actual money just inserts into transfer table.
        /// </summary>
        /// <param name="receiverId"></param> ReceiverID cannot be the same as the current user (checks when prompting for receiverID.
        /// <param name="amount"></param>
        /// <returns>Transfer class with all parameters added.</returns>
        public TransferClient RequestTransfer(int id, double amount)
        {
            TransferClient t = new TransferClient();
            t.From = id; 
            t.Amount = amount;
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            RestRequest request = new RestRequest(API_BASE_URL + "transfers");
            request.AddJsonBody(t);
            IRestResponse<TransferClient> response = client.Post<TransferClient>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                Console.WriteLine(response.StatusCode + response.ErrorMessage);
                return null;
            }
            else if (!response.IsSuccessful)
            {
                Console.WriteLine(response.StatusCode + response.ErrorMessage);
                return null;
            }

            return response.Data;
        }
    }
}
