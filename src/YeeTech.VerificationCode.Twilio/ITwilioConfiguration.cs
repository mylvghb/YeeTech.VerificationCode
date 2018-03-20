namespace YeeTech.VerificationCode.Twilio
{
    public interface ITwilioConfiguration
    {
        string AccountSid { get; set; }

        string AuthToken { get; set; }
    }
}