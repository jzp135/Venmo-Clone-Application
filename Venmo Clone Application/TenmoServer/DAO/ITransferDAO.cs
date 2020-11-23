using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {
        List<Transfer> TransferHistory(int userId);

        Transfer TransferDetails(int transferId);

        List<Transfer> ViewPendingTransfers(int userId);
        Transfer LogTransfer(int senderId, int receiverId, double amount, int transferType, int transferStatus);

        bool AddBalance(int receiverId, double amount);

        bool SubtractBalance(int senderId, double amount);

        bool UpdateTransfer(Transfer pendingTransfer);

    }
}
