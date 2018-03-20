namespace YeeTech.VerificationCode.Mail
{
    public class MailSmtpConfiguration : IMailSmtpConfiguration
    {
        public MailSmtpConfiguration()
        {
            RemoveXOauth2 = true;
        }

        public string SmtpServer { get; set; }

        public int SmtpPort { get; set; }

        public string SmtpUsername { get; set; }

        public string SmtpPassword { get; set; }

        public bool UseSsl { get; set; }

        public bool RemoveXOauth2 { get; set; }
    }
}