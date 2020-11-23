using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        public int? ID { get; set; }

        public int To { get; set; }

        public int From { get; set; }

        public double Amount { get; set; }

        public int Type { get; set; }

        public int Status { get; set; }

        public string UsernameTo { get; set; } = null;

        public string UsernameFrom { get; set; } = null;
    }
    public class TransferToSend
    {
        public int To { get; set; }
        public double Amount { get; set; }
        public int Type { get; } = 2;
        public int Status { get; } = 2;

    }
}
