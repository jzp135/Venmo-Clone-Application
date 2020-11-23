using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class TransferClient
    {
        public int? ID { get; set; }

        public int To { get; set; }

        public int From { get; set; }

        public double Amount { get; set; }

        public int Type { get; set; }

        public string TypeName
        {
            get
            {
                if (Type == 1)
                {
                    return "Request";
                }
                else
                {
                    return "Send";
                }
            }
        }

        public int Status { get; set; }

        public string StatusName
        {
            get
            {
                if (Status == 1)
                {
                    return "Pending";
                }
                else if (Status == 2)
                {
                    return "Approved";
                }
                else
                {
                    return "Rejected";
                }
            }
        }

        public string UsernameTo { get; set; } = null;

        public string UsernameFrom { get; set; } = null;

    }

    public class TransferToSend : TransferClient
    {

        public TransferToSend (int receiverId, double amount)
        {
            this.To = receiverId;
            this.Amount = amount;
        }

        public TransferToSend() { }
      
    }
}
