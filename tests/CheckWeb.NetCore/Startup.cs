using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YeeTech.VerificationCode;
using YeeTech.VerificationCode.Interface;
using YeeTech.VerificationCode.Mail;

namespace CheckWeb.NetCore
{
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
}