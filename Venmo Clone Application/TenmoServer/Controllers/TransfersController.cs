using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]

    public class TransfersController : ControllerBase
    {
        private readonly ITransferDAO transferDAO;
        private readonly IAccountDAO accountDAO;
        public TransfersController(ITransferDAO _transferDAO, IAccountDAO _accountDAO)
        {
            transferDAO = _transferDAO;
            accountDAO = _accountDAO;
        }

        [HttpPut]
        public IActionResult SendTransfer(TransferToSend transfer)
        {

            //Step 1 Get user id
            int userId = (int)GetCurrentUserId();

            //Step 2 Subtract from Sender
            bool subtractSuccess = transferDAO.SubtractBalance(userId, transfer.Amount);
            if (!subtractSuccess)
            {
                return StatusCode(500, "Server error subtracting balance");
            }

            //Step 3 Add to Receiver
            bool addSuccess = transferDAO.AddBalance(transfer.To, transfer.Amount);
            if (!addSuccess)
            {
                return StatusCode(500, "Server error adding balance");
            }

            Transfer returnTransfer = transferDAO.LogTransfer(userId, transfer.To, transfer.Amount, transfer.Type, transfer.Status);
            if (returnTransfer == null) //if return transfer is null, the funds were exchanged, but the transfer was not added to the transfers table
            {
                return StatusCode(500, "Server error logging transfer");
            }

            return Ok(returnTransfer);
        }

        [HttpGet]
        public IActionResult GetTransfers()
        {
            int userId = (int)GetCurrentUserId();
            List<Transfer> result = null;
            result = transferDAO.TransferHistory(userId);
            if (result != null)
            {
                return Ok(result);
            }
            return StatusCode(500);
        }

        [HttpGet("{transferId}")]
        public IActionResult GetTransferDetails(int transferId)
        {
            IActionResult result = NotFound(); //"Oops it looks we don't have a transfer with that ID! Are you sure it is correct?"
            Transfer transfer = transferDAO.TransferDetails(transferId);
            if (transfer != null)
            {
                result = Ok(transfer);
            }
            return result;
        }

        [HttpPost]
        public IActionResult RequestTransfer(Transfer transfer)
        {
            int userId = (int)GetCurrentUserId();
            int type = 1;
            int status = 1;
            Transfer requestedTransfer = transferDAO.LogTransfer(transfer.From, userId, transfer.Amount, type, status);
            if (requestedTransfer == null)
            {
                return StatusCode(500);
            }

            return Created($"/transfers/{requestedTransfer.ID}", requestedTransfer);
        }

        [HttpGet("pending")]
        public IActionResult PendingTransfers()
        {
            int userId = (int)GetCurrentUserId();
            List<Transfer> pendingTransfer = transferDAO.ViewPendingTransfers(userId);
            if (pendingTransfer == null)
            {
                return StatusCode(500);
            }

            return Ok(pendingTransfer);
        }

        [HttpPut("approve_deny")]
        public IActionResult ApproveDenyTransfer(Transfer pendingTransfer)
        {
            const int approved = 2;
            if (pendingTransfer.Status == approved)
            {
                if (!transferDAO.UpdateTransfer(pendingTransfer))
                {
                    return StatusCode(500);
                }
                else if (!transferDAO.SubtractBalance(pendingTransfer.From, pendingTransfer.Amount))
                {
                    return StatusCode(500);
                }
                else if (!transferDAO.AddBalance(pendingTransfer.To, pendingTransfer.Amount))
                {
                    return StatusCode(500);
                }
                else
                {
                    return Ok(pendingTransfer);
                }
            }
            else 
            {
                transferDAO.UpdateTransfer(pendingTransfer);
                return NoContent();
            }
        }

        private int? GetCurrentUserId()
        {
            string userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return null;
            int.TryParse(userId, out int userIdInt);
            return userIdInt;
        }
    }
}
