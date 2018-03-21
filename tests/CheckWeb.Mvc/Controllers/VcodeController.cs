using System.Web.Mvc;
using YeeTech.VerificationCode;
using YeeTech.VerificationCode.Image;
using YeeTech.VerificationCode.Interface;

namespace CheckWeb.Mvc.Controllers
{
    public class VcodeController : Controller
    {
        public ActionResult GetGeneralVerificationCode(GeneralCodeFlags flags, int length = 4)
        {
            IImageVerificationCodeFactory factory = new ImageVerificationCodeProvider(
                    new GeneralVerificationCode(flags, length)
                )
            {
                Width = 89,
                Height = 32
            };
            return File(factory.Draw(), "image/jpg");
        }
    }
}