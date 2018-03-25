# Cross-Platform Verification Solution for .NET

YeeTech.VerificationCode is a cross-platform, fast, high performance and extendible verification code solution. It provides very rich interface that drawing image verification code and sending verification code pass Email, SMS, Voice etc. You can custom implement interfaces that draw and send verification code by yourself.

## YeeTech.VerificationCode Libraies

* `YeeTech.VerificationCode`&emsp;Verification code core. it provid generate diffrence verification code, delegates and rich interfaces that draw or send verification code.
* `YeeTech.VerificationCode.Image`&emsp;Dawing verification code image.
* `YeeTech.VerificationCode.Mail`&emsp;Sending verification code message pass email.
* `YeeTech.VerificationCode.Twilio`&emsp;Sending SMS,Voice verification code pass Twilio gateway.

## Live Examples

**image verification code service**

1. use servicestack ioc (.NET Framework):
```csharp
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
```

2. use asp.net mvc(.NET Framework):
    
```csharp
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
```

**Send verificaiton code pass Email**

use asp.net core(.NET Core)

```csharp
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            var server = Configuration.GetValue<string>("MailSmtp:SmtpServer");
            var port = Configuration.GetValue<int>("MailSmtp:SmtpPort");
            var username = Configuration.GetValue<string>("MailSmtp:SmtpUsername");
            var password = Configuration.GetValue<string>("MailSmtp:SmtpPassword");
            var useSsl = Configuration.GetValue<bool>("MailSmtp:UseSsl");

            services.AddSingleton<IMailSmtpConfiguration>(new MailSmtpConfiguration
            {
                SmtpServer = server,
                SmtpPort = port,
                SmtpUsername = username,
                SmtpPassword = password,
                UseSsl = useSsl
            });

            services.AddSingleton<IVerificationCode>(new GeneralVerificationCode(
                GeneralCodeFlags.Number, 6
            ));
            services.AddSingleton<ITemplateParser>(new DefaultTemplateParser
            {
                Template = "【吾乐购】{{Code}} 是您的验证码，非本人操作请忽略"
            });
            services.AddSingleton<IMessageVerificationCodeFactory>(provider =>
            {
                return new MailVerificationCodeProvider(
                    provider.GetService<IMailSmtpConfiguration>(),
                    provider.GetService<IVerificationCode>(),
                    provider.GetService<ITemplateParser>()
                )
                {
                    From = "postmaster@wulegou.top",
                    MessageSentHandler = (text, message) =>
                    {
                        // store code in session and log the message
                    }
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

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
```

More examples at [Tests](https://github.com/mylvghb/YeeTech.VerificationCode/tree/master/tests)