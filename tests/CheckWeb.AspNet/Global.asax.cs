using System;
using System.Reflection;
using System.Web;
using Funq;
using ServiceStack;
using YeeTech.VerificationCode;
using YeeTech.VerificationCode.Image;
using YeeTech.VerificationCode.Interface;

namespace CheckWeb.AspNet
{
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
            container.Register<IVerificationCode>(new GeneralVerificationCode());
            container.RegisterAs<ImageVerificationCodeProvider, IImageVerificationCodeFactory>()
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
}