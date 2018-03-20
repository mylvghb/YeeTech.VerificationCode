using System;
using System.Net;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using YeeTech.VerificationCode.Interface;

namespace YeeTech.VerificationCode.Twilio
{
    /// <summary>
    ///     语音验证码提供程序
    /// </summary>
    public class VoiceVerificationCodeProvider : IMessageVerificationCodeFactory
    {
        private readonly ITwilioConfiguration _configuration;
        private readonly ITemplateParser _parser;
        private readonly IVerificationCode _verificationCode;

        public VoiceVerificationCodeProvider(ITwilioConfiguration configuration,
            IVerificationCode verificationCode = null, ITemplateParser parser = null)
        {
            _configuration = configuration;
            _verificationCode = verificationCode ?? new GeneralVerificationCode();
            _parser = parser;
        }

        public MessageSentHandlerDelegate<CallResource> MessageSentHandler { get; set; }

        public string Url { get; set; }

        public VerificationCodoeExceptionHandlerDelegate VerificationCodoeExceptionHandler { get; set; }

        public string From { get; set; }

        public void Send(string to)
        {
            if (string.IsNullOrEmpty(From)) throw new Exception("'From' cannot be empty");
            if (string.IsNullOrEmpty(to)) throw new ArgumentException(nameof(to));
            var code = _verificationCode.Generate(out var result);
            var text = _parser.Parse(code);
            TwilioClient.Init(_configuration.AccountSid, _configuration.AuthToken);
            var message = CallResource.Create(
                from: new PhoneNumber(From),
                to: new PhoneNumber(to),
                url: new Uri(
                    $"{Url ?? "http://twimlets.com/message?" + WebUtility.UrlEncode("Message[0]=")}{text}"
                )
            );
            MessageSentHandler?.Invoke(result, message);
        }

        public void Send<T>(Func<string, T> options) where T : class
        {
            try
            {
                if (options == null) throw new ArgumentException(nameof(options));
                TwilioClient.Init(_configuration.AccountSid, _configuration.AuthToken);
                var code = _verificationCode.Generate(out var result);
                var param = options.Invoke(code);
                if (!(param is CreateCallOptions)) return;
                var callOptions = param as CreateCallOptions;
                var call = CallResource.Create(callOptions);
                MessageSentHandler?.Invoke(result, call);
            }
            catch (Exception ex)
            {
                VerificationCodoeExceptionHandler?.Invoke(ex);
            }
        }

        public async void SendAsync(string to)
        {
            if (string.IsNullOrEmpty(From)) throw new Exception("'From' cannot be empty");
            if (string.IsNullOrEmpty(to)) throw new ArgumentException(nameof(to));
            var code = _verificationCode.Generate(out var result);
            var text = _parser.Parse(code);
            TwilioClient.Init(_configuration.AccountSid, _configuration.AuthToken);
            var message = await CallResource.CreateAsync(
                from: new PhoneNumber(From),
                to: new PhoneNumber(to),
                url: new Uri(
                    $"{Url ?? "http://twimlets.com/message?" + WebUtility.UrlEncode("Message[0]=")}{text}"
                )
            );
            MessageSentHandler?.Invoke(result, message);
        }

        public async void SendAsync<T>(Func<string, T> options) where T : class
        {
            try
            {
                if (options == null) throw new ArgumentException(nameof(options));
                TwilioClient.Init(_configuration.AccountSid, _configuration.AuthToken);
                var code = _verificationCode.Generate(out var result);
                var param = options.Invoke(code);
                if (!(param is CreateCallOptions)) return;
                var callOptions = param as CreateCallOptions;
                var call = await CallResource.CreateAsync(callOptions);
                MessageSentHandler?.Invoke(result, call);
            }
            catch (Exception ex)
            {
                VerificationCodoeExceptionHandler?.Invoke(ex);
            }
        }
    }
}