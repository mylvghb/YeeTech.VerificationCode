using ServiceStack;
using YeeTech.VerificationCode.Interface;

namespace CheckWeb.AspNet
{
    public class VerificationCodeService : Service
    {
        public IImageVerificationCodeFactory Factory { get; set; }

        public object Get(GetVcodeRequest request)
        {
            var bytes = Factory.Draw();

            return new HttpResult(bytes, MimeTypes.ImageJpg);
        }
    }

    [Route("/vcode")]
    public class GetVcodeRequest
    {
    }
}