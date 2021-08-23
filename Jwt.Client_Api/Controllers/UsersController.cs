using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Jwt.Client_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {

        //[Authorize]
        [Authorize(UserType = "worker")]
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(">>>>>>>>>>    Authorised :-)    <<<<<<<<<<<<");
        }
    }
}
