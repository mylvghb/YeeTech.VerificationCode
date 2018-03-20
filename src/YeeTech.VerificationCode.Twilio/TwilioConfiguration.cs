namespace YeeTech.VerificationCode.Twilio
{
    public abstract class TwilioConfiguration : ITwilioConfiguration
    {
        public virtual string AccountSid { get; set; }

        public virtual string AuthToken { get; set; }
    }
}