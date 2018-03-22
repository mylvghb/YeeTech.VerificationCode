using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YeeTech.VerificationCode.Interface;

namespace CheckWeb.NetCore.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly IMessageVerificationCodeFactory _factory;

        public MessageController(IMessageVerificationCodeFactory factory)
        {
            _factory = factory;
        }

        public void Post(MessageRequest request)
        {
            var a = 1;
        }
    }

    public class MessageRequest
    {
        public string To { get; set; }
    }
}