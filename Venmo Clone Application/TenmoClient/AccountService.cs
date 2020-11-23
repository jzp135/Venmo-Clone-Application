using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;
using TenmoClient.Data;

namespace TenmoClient
{
    public class AccountService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();
       
        //Account Endpoints
        public double GetBalance()
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            RestRequest request = new RestRequest(API_BASE_URL + "accounts/balance");
            IRestResponse<double> response = client.Get<double>(request);
            // Check responses for error
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                return -1;
            }

            return response.Data;
        }

        public List<UserDTO> GetUsers()
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            RestRequest request = new RestRequest(API_BASE_URL + "accounts/users");
            IRestResponse<List<UserDTO>> response = client.Get <List<UserDTO>>(request);
            // Check responses for error
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
            }

            return response.Data;
        }
    }
}
