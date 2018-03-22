using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
//            services.AddOptions();
            var mailSmtp = Configuration.GetSection("MailSmtp");
            services.Configure<MailSmtpConfiguration>(mailSmtp);
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
                        Configuration.Get<MailSmtpConfiguration>(),
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
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
