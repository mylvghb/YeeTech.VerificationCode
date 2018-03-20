using System;
using YeeTech.VerificationCode.Interface;

namespace YeeTech.VerificationCode
{
    public class MathVerificationCode : IVerificationCode
    {
        public string Generate()
        {
            return Generate(out var _);
        }

        public string Generate(out string result)
        {
            var random = new Random();
            int num1 = random.Next(0, 10), num2 = random.Next(0, 10);
            string text;
            switch (random.Next(0, 4))
            {
                case 0:
                    // +
                    text = $"{num1}+{num2}=?";
                    result = (num1 + num2).ToString();
                    break;

                case 1:
                    // -
                    if (num1 < num2)
                    {
                        text = $"{num2}-{num1}=?";
                        result = (num2 - num1).ToString();
                    }
                    else
                    {
                        text = $"{num1}-{num2}=?";
                        result = (num1 - num2).ToString();
                    }

                    break;
                default:
                    // *
                    text = $"{num1}×{num2}=?";
                    result = (num1 + num1).ToString();
                    break;
            }

            return text;
        }
    }
}