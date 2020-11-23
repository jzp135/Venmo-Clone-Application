using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDAO : ITransferDAO
    {
        private readonly string connectionString;

        public TransferSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public bool UpdateTransfer(Transfer pendingTransfer)
        {
            bool isSuccessful = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE transfers SET transfer_status_id = @statusID WHERE transfer_id = @transferId;", conn);
                    cmd.Parameters.AddWithValue("@statusID", pendingTransfer.Status);
                    cmd.Parameters.AddWithValue("@transferId", pendingTransfer.ID);

                    int rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());
                    if (rowsAffected == 1)
                    {
                        isSuccessful = true;
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return isSuccessful;
        }

        public bool AddBalance(int receiverId, double amount)
        {
            
            bool isSuccessful = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE accounts SET balance = (balance + @amount) WHERE account_id = @receiverId;", conn);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@receiverId", receiverId);

                    int rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());
                    if (rowsAffected == 1)
                    {
                        isSuccessful = true;
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return isSuccessful;
        }

        public bool SubtractBalance(int senderId, double amount)
        {

            bool isSuccessful = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE accounts SET balance = (balance - @amount) WHERE account_id = @senderId;", conn);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@senderId", senderId);

                    int rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());
                    if (rowsAffected == 1)
                    {
                        isSuccessful = true;
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return isSuccessful;
        }

        public Transfer LogTransfer(int senderId, int receiverId, double amount, int transferType = 1, int transferStatus = 2)
        {
            Transfer result = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                                                    "VALUES (@transferType, @transferStatus, @senderId, @receiverId, @amount); " +
                                                    "SELECT * FROM transfers WHERE transfer_id = (SELECT Max(transfer_id) FROM transfers);", conn);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@receiverId", receiverId);
                    cmd.Parameters.AddWithValue("@senderId", senderId);
                    cmd.Parameters.AddWithValue("@transferType", transferType);
                    cmd.Parameters.AddWithValue("@transferStatus", transferStatus);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        result = GetTransferFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return result;
        }

        private Transfer GetTransferFromReader(SqlDataReader reader)
        {
            Transfer t = new Transfer()
            {
                Type = Convert.ToInt32(reader["transfer_type_id"]),
                Status = Convert.ToInt32(reader["transfer_status_id"]),
                ID = Convert.ToInt32(reader["transfer_id"]),
                From = Convert.ToInt32(reader["account_from"]),
                To = Convert.ToInt32(reader["account_to"]),
                Amount = Convert.ToDouble(reader["amount"]),
            };

            return t;
        }

        public Transfer TransferDetails(int transferId)
        {
            Transfer result = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount " +
                                                    "FROM users u " +
                                                    "JOIN accounts a ON a.user_id = u.user_id " +
                                                    "FULL OUTER JOIN transfers t ON account_from = a.account_id OR account_to = account_id " +
                                                    "WHERE transfer_id = @transferId;", conn);
                    cmd.Parameters.AddWithValue("@transferId", transferId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        result = GetTransferFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return result;
        }

        public List<Transfer> TransferHistory(int userId)
        {
            List<Transfer> result = new List<Transfer>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount " +
                                                    "FROM users u " +
                                                    "JOIN accounts a ON a.user_id = u.user_id " +
                                                    "FULL OUTER JOIN transfers t ON account_from = a.account_id OR account_to = account_id " +
                                                    "WHERE u.user_id = @userId; ", conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Transfer transfer = GetTransferFromReader(reader);
                        AddSenderUsername(transfer);
                        AddReceiverUsername(transfer);
                        result.Add(transfer);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return result;
        }

        public void AddSenderUsername(Transfer transfer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT username FROM users WHERE user_id = @accountFrom;", conn);
                    cmd.Parameters.AddWithValue("@accountFrom", transfer.From);
                    transfer.UsernameFrom = Convert.ToString(cmd.ExecuteScalar());
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public void AddReceiverUsername(Transfer transfer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT username FROM users WHERE user_id = @accountTo;", conn);
                    cmd.Parameters.AddWithValue("@accountTo", transfer.To);
                    transfer.UsernameTo = Convert.ToString(cmd.ExecuteScalar());
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public List<Transfer> ViewPendingTransfers(int userId)
        {
            List<Transfer> result = new List<Transfer>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount " +
                                                    "FROM users u " +
                                                    "JOIN accounts a ON a.user_id = u.user_id " +
                                                    "FULL OUTER JOIN transfers t ON account_from = account_id " +
                                                    "WHERE u.user_id = @userId AND transfer_status_id = 1; ", conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Transfer transfer = GetTransferFromReader(reader);
                        AddSenderUsername(transfer);
                        AddReceiverUsername(transfer);
                        result.Add(transfer);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return result;
        }
      
    }
}
