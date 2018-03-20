namespace YeeTech.VerificationCode.Mail
{
    public interface IMailSmtpConfiguration
    {
        string SmtpServer { get; set; }

        int SmtpPort { get; set; }

        string SmtpUsername { get; set; }

        string SmtpPassword { get; set; }

        bool UseSsl { get; set; }

        bool RemoveXOauth2 { get; set; }
    }
}