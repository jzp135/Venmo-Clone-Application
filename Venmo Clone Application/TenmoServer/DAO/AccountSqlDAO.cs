using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountSqlDAO : IAccountDAO
    {
        private readonly string connectionString;

        public AccountSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public Account AccountGet(int userId)
        {
            Account account = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT user_id, account_id, balance FROM accounts WHERE user_id = @userId;", conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows && reader.Read())
                    {
                        account = GetAccountFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return account;
        }

        private Account GetAccountFromReader(SqlDataReader reader)
        {
            Account a = new Account()
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                AccountId = Convert.ToInt32(reader["account_id"]),
                Balance = Convert.ToDouble(reader["balance"])
            };

            return a;
        }

        public double GetBalance(int userId)
        {
            double returnBalance;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT balance FROM accounts WHERE user_id = @userId;", conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    returnBalance = Convert.ToDouble(cmd.ExecuteScalar());
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnBalance;
        }
        
    }
}
