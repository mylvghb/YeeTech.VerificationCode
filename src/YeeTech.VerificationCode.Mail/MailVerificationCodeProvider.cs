using System;
using MailKit.Net.Smtp;
using MimeKit;
using YeeTech.VerificationCode.Interface;

namespace YeeTech.VerificationCode.Mail
{
    public class MailVerificationCodeProvider : IMessageVerificationCodeFactory
    {
        private readonly IMailSmtpConfiguration _configuration;
        private readonly ITemplateParser _parser;
        private readonly IVerificationCode _verificationCode;

        public MailVerificationCodeProvider(IMailSmtpConfiguration configuration,
            IVerificationCode verificationCode = null, ITemplateParser parser = null)
        {
            _configuration = configuration;
            _verificationCode = verificationCode ?? new GeneralVerificationCode();
            _parser = parser;
        }

        public AuthenticateHandlerDelegate<SmtpClient> AuthenticateHandlerDelegate { get; set; }

        public MessageSentHandlerDelegate<MimeMessage> MessageSentHandler { get; set; }

        public VerificationCodoeExceptionHandlerDelegate VerificationCodoeExceptionHandler { get; set; }

        public string From { get; set; }

        public void Send(string to)
        {
            try
            {
                if (string.IsNullOrEmpty(From)) throw new Exception("'From' cannot be empty");
                if (string.IsNullOrEmpty(to)) throw new ArgumentException(nameof(to));
                var code = _verificationCode.Generate(out var result);
                var text = _parser.Parse(code);
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(From));
                message.To.Add(new MailboxAddress(to));
                message.Body = new TextPart {Text = text};
                using (var emailClient = new SmtpClient())
                {
                    emailClient.Connect(_configuration.SmtpServer, _configuration.SmtpPort, _configuration.UseSsl);
                    if (AuthenticateHandlerDelegate != null)
                    {
                        AuthenticateHandlerDelegate.Invoke(emailClient);
                    }
                    else
                    {
                        if (_configuration.RemoveXOauth2) emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                        emailClient.Authenticate(_configuration.SmtpUsername, _configuration.SmtpPassword);
                    }

                    emailClient.Send(message);
                    MessageSentHandler?.Invoke(result, message);
                    emailClient.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                VerificationCodoeExceptionHandler?.Invoke(ex);
            }
        }

        public void Send<T>(Func<string, T> options) where T : class
        {
            try
            {
                if (options == null) throw new ArgumentException(nameof(options));
                var code = _verificationCode.Generate(out var result);
                var param = options.Invoke(code);
                if (!(param is MimeMessage)) return;
                var message = param as MimeMessage;
                using (var emailClient = new SmtpClient())
                {
                    emailClient.Connect(_configuration.SmtpServer, _configuration.SmtpPort, _configuration.UseSsl);
                    if (AuthenticateHandlerDelegate != null)
                    {
                        AuthenticateHandlerDelegate.Invoke(emailClient);
                    }
                    else
                    {
                        if (_configuration.RemoveXOauth2) emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                        emailClient.Authenticate(_configuration.SmtpUsername, _configuration.SmtpPassword);
                    }

                    emailClient.Send(message);
                    MessageSentHandler?.Invoke(result, message);
                    emailClient.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                VerificationCodoeExceptionHandler?.Invoke(ex);
            }
        }

        public async void SendAsync(string to)
        {
            try
            {
                if (string.IsNullOrEmpty(From)) throw new Exception("'From' cannot be empty");
                if (string.IsNullOrEmpty(to)) throw new ArgumentException(nameof(to));
                var code = _verificationCode.Generate(out var result);
                var text = _parser.Parse(code);
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(From));
                message.To.Add(new MailboxAddress(to));
                message.Body = new TextPart { Text = text };
                using (var emailClient = new SmtpClient())
                {
                    await emailClient.ConnectAsync(_configuration.SmtpServer, _configuration.SmtpPort, _configuration.UseSsl);
                    if (AuthenticateHandlerDelegate != null)
                    {
                        AuthenticateHandlerDelegate.Invoke(emailClient);
                    }
                    else
                    {
                        if (_configuration.RemoveXOauth2) emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                        emailClient.Authenticate(_configuration.SmtpUsername, _configuration.SmtpPassword);
                    }

                    await emailClient.SendAsync(message);
                    MessageSentHandler?.Invoke(result, message);
                    emailClient.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                VerificationCodoeExceptionHandler?.Invoke(ex);
            }
        }

        public async void SendAsync<T>(Func<string, T> options) where T : class
        {
            try
            {
                if (options == null) throw new ArgumentException(nameof(options));
                var code = _verificationCode.Generate(out var result);
                var param = options.Invoke(code);
                if (!(param is MimeMessage)) return;
                var message = param as MimeMessage;
                using (var emailClient = new SmtpClient())
                {
                    await emailClient.ConnectAsync(_configuration.SmtpServer, _configuration.SmtpPort,
                        _configuration.UseSsl);
                    if (AuthenticateHandlerDelegate != null)
                    {
                        AuthenticateHandlerDelegate.Invoke(emailClient);
                    }
                    else
                    {
                        if (_configuration.RemoveXOauth2) emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                        emailClient.Authenticate(_configuration.SmtpUsername, _configuration.SmtpPassword);
                    }

                    await emailClient.SendAsync(message);
                    MessageSentHandler?.Invoke(result, message);
                    emailClient.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                VerificationCodoeExceptionHandler?.Invoke(ex);
            }
        }
    }
}