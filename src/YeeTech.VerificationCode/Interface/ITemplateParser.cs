namespace YeeTech.VerificationCode.Interface
{
    public interface ITemplateParser
    {
        string Template { get; set; }

        string Parse(object value);
    }
}