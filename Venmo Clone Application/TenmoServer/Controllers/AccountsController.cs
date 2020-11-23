using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]

    public class AccountsController : ControllerBase
    {
        private readonly IAccountDAO accountDAO;
        private readonly IUserDAO userDAO;

        public AccountsController(IAccountDAO _accountDAO, IUserDAO _userDAO)
        {
            accountDAO = _accountDAO;
            userDAO = _userDAO;
        }

        [HttpGet("balance")]
        public IActionResult GetBalance()
        {
            int userId = (int)GetCurrentUserId();
            double result = accountDAO.GetBalance(userId);
            return Ok(result);
        }

        [HttpGet("users")]
        public IActionResult GetUserDTOs()
        {
            List<UserDTO> users = userDAO.GetUserDTOs();
            if (users == null || users.Count == 0)
            {
                return StatusCode(500);
            }

            return Ok(users);
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
