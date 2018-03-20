using System;

namespace YeeTech.VerificationCode.Interface
{
    public interface IVerificationCodeFactory
    {
        VerificationCodoeExceptionHandlerDelegate VerificationCodoeExceptionHandler { get; set; }
    }
}