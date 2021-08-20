using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AwesomeAPI.Authentication;
using AwesomeAPI.Authentication.Extensions;
using AwesomeAPI.Authentication.Filters;
using AwesomeAPI.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AwesomeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [RoleAuthorize(RoleNames.Administrator, RoleNames.PowerUser)]
        [HttpGet]
        public IActionResult Test()
        {
            var user = new User
            {
                FirstName = User.GetFirstName(),
                LastName = User.GetLastName(),
                Email = User.GetEmail()
            };
            return Ok(user);
        }
    }
}