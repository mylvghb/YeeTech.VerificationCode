# Cross-Platform Verification Solution for .NET

YeeTech.VerificationCode is a cross-platform, fast, high performance and extendible verification code solution. It provides very rich interface that drawing image verification code and sending verification code pass Email, SMS, Voice etc. You can custom implement interfaces that draw and send verification code by yourself.

## YeeTech.VerificationCode Libraies

* `YeeTech.VerificationCode`&emsp;Verification code core. it provid generate diffrence verification code, delegates and rich interfaces that draw or send verification code.
* `YeeTech.VerificationCode.Image`&emsp;Dawing verification code image.
* `YeeTech.VerificationCode.Mail`&emsp;Sending verification code message pass email.
* `YeeTech.VerificationCode.Twilio`&emsp;Sending SMS,Voice verification code pass Twilio gateway.

## Live Examples

**image verification code service**

1. use servicestack:
<pre>
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            new AppHost().Init();
        }
    }

    public class AppHost : AppHostBase
    {
        public AppHost() : base("Verification Code Service", Assembly.GetExecutingAssembly())
        {
        }

        public override void Configure(Container container)
        {
            container.Register\<IVerificationCode\>(new GeneralVerificationCode());
            container.RegisterAs\<ImageVerificationCodeProvider, IImageVerificationCodeFactory\>()
                .InitializedBy((c, f) =>
                {
                    f.Width = 80;
                    f.Height = 32;
                    f.ImageDrawCompletedHandler = text =>
                    {
                        // store in session
                    };
                })
                .ReusedWithin(ReuseScope.Container);
        }
    }

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
</pre>