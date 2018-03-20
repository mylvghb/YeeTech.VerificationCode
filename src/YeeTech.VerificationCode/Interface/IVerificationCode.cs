namespace YeeTech.VerificationCode.Interface
{
    public interface IVerificationCode
    {
        string Generate();

        string Generate(out string result);
    }
}