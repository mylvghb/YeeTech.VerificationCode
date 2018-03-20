using System;

namespace YeeTech.VerificationCode
{
    public delegate void VerificationCodoeExceptionHandlerDelegate(Exception ex);

    public delegate void AuthenticateHandlerDelegate<in T>(T obj);

    public delegate void MessageSentHandlerDelegate<in T>(string text, T obj);

    public delegate void ImageDrawCompletedHandlerDelegate(string text);
}