using System;

namespace YeeTech.VerificationCode.Interface
{
    public interface IMessageVerificationCodeFactory : IVerificationCodeFactory
    {
        string From { get; set; }

        void Send(string to);

        void Send<T>(Func<string, T> options) where T : class;

        void SendAsync(string to);

        void SendAsync<T>(Func<string, T> options) where T : class;
    }
}