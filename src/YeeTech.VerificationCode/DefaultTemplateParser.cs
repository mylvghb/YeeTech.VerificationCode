using System;
using System.Text.RegularExpressions;
using YeeTech.VerificationCode.Interface;

namespace YeeTech.VerificationCode
{
    public class DefaultTemplateParser : ITemplateParser
    {
        public string Template { get; set; }

        public string Parse(object value)
        {
            if (string.IsNullOrEmpty(Template))
                throw new Exception("Template cannot be empty");

            return Regex.Replace(Template, "({{VerificationCode}})|({{Code}})", value.ToString(),
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        }
    }
}