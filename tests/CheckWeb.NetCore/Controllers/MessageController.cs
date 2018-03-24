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
            _factory.Send(request.To);
        }
    }

    public class MessageRequest
    {
        public string To { get; set; }
    }
}