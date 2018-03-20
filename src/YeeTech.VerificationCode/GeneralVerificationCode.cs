using System;
using System.Text;
using System.Text.RegularExpressions;
using YeeTech.VerificationCode.Interface;

namespace YeeTech.VerificationCode
{
    public class GeneralVerificationCode : IVerificationCode
    {
        private static readonly Regex numberAndLetter =
            new Regex("^(?=.*[a-zA-Z])(?=.*[0-9])[a-zA-Z0-9]+$", RegexOptions.Compiled);

        private static readonly Regex numberAndLowerLetter =
            new Regex("^(?=.*[a-z])(?=.*[0-9])[a-z0-9]+", RegexOptions.Compiled);

        private static readonly Regex numberAndUpperLetter =
            new Regex("^(?=.*[A-Z])(?=.*[0-9])[A-Z0-9]+", RegexOptions.Compiled);

        private int _length;

        public GeneralVerificationCode() : this(GeneralCodeFlags.Number)
        {
        }

        public GeneralVerificationCode(GeneralCodeFlags codeFlags, int length = 4)
        {
            CodeFlags = codeFlags;
            Length = length;
        }

        public GeneralCodeFlags CodeFlags { get; set; }

        public int Length
        {
            get => _length;
            set
            {
                if (value < 4) throw new ArgumentException(nameof(Length));

                _length = value;
            }
        }

        public string Generate()
        {
            switch (CodeFlags)
            {
                case GeneralCodeFlags.Number:
                    return CreateLetterNumberCode(Length, CodeFlags);
                case GeneralCodeFlags.Letter:
                    return CreateLetterNumberCode(Length, CodeFlags);
                case GeneralCodeFlags.LowerLetter:
                    return CreateLetterNumberCode(Length, CodeFlags);
                case GeneralCodeFlags.UpperLetter:
                    return CreateLetterNumberCode(Length, CodeFlags);
                case GeneralCodeFlags.Number | GeneralCodeFlags.Letter:
                {
                    var text = CreateLetterNumberCode(Length, CodeFlags);
                    while (!numberAndLetter.IsMatch(text)) text = CreateLetterNumberCode(Length, CodeFlags);
                    return text;
                }
                case GeneralCodeFlags.Number | GeneralCodeFlags.LowerLetter:
                {
                    var text = CreateLetterNumberCode(Length, CodeFlags);
                    while (!numberAndLowerLetter.IsMatch(text)) text = CreateLetterNumberCode(Length, CodeFlags);
                    return text;
                }
                case GeneralCodeFlags.Number | GeneralCodeFlags.UpperLetter:
                {
                    var text = CreateLetterNumberCode(Length, CodeFlags);
                    while (!numberAndUpperLetter.IsMatch(text)) text = CreateLetterNumberCode(Length, CodeFlags);
                    return text;
                }
                case GeneralCodeFlags.Chinese:
                    return CreateChineseCode(Length);
                default:
                    throw new ArgumentOutOfRangeException(nameof(CodeFlags), CodeFlags, null);
            }
        }

        public string Generate(out string result)
        {
            var text = Generate();
            result = text;
            return text;
        }

        private static string CreateLetterNumberCode(int length, GeneralCodeFlags flags)
        {
            var builder = new StringBuilder();
            var random = new Random();
            for (var i = 0; i < length; i++)
            {
                var num2 = random.Next();
                switch (flags)
                {
                    case GeneralCodeFlags.Number:
                        builder.Append((char) (0x30 + (ushort) (num2 % 10)));
                        break;
                    case GeneralCodeFlags.LowerLetter:
                        builder.Append((char) (0x61 + (ushort) (num2 % 0x1a)));
                        break;
                    case GeneralCodeFlags.UpperLetter:
                        builder.Append((char) (0x41 + (ushort) (num2 % 0x1a)));
                        break;
                    case GeneralCodeFlags.Letter:
                        builder.Append((char) ((num2 % 2 == 0 ? 0x61 : 0x41) + (ushort) (num2 % 0x1a)));
                        break;
                    case GeneralCodeFlags.Number | GeneralCodeFlags.LowerLetter:
                        if (num2 % 2 == 0)
                            builder.Append((char) (0x30 + (ushort) (num2 % 10)));
                        else
                            builder.Append((char) (0x61 + (ushort) (num2 % 0x1a)));
                        break;
                    case GeneralCodeFlags.Number | GeneralCodeFlags.UpperLetter:
                        if (num2 % 2 == 0)
                            builder.Append((char) (0x30 + (ushort) (num2 % 10)));
                        else
                            builder.Append((char) (0x41 + (ushort) (num2 % 0x1a)));
                        break;
                    case GeneralCodeFlags.Number | GeneralCodeFlags.Letter:
                        if (num2 % 2 == 0)
                            builder.Append((char) (0x30 + (ushort) (num2 % 10)));
                        else if (num2 % 3 == 0)
                            builder.Append((char) (0x41 + (ushort) (num2 % 0x1a)));
                        else
                            builder.Append((char) (0x61 + (ushort) (num2 % 0x1a)));
                        break;
                    case GeneralCodeFlags.Chinese:
                        throw new NotSupportedException();
                    default:
                        throw new ArgumentOutOfRangeException(nameof(flags), flags, null);
                }
            }

            return builder.ToString();
        }

        private static string CreateChineseCode(int length)
        {
            var builder = new StringBuilder();
            var rm = new Random();
            var gb = Encoding.GetEncoding("gb2312");

            for (var i = 0; i < length; i++)
            {
                var regionCode = rm.Next(16, 56);
                var positionCode = rm.Next(1, regionCode == 55 ? 90 : 95);

                var regionCodeMachine = regionCode + 160; 
                var positionCodeMachine = positionCode + 160;  

                var bytes = new[] {(byte) regionCodeMachine, (byte) positionCodeMachine};
                builder.Append(gb.GetString(bytes));
            }

            return builder.ToString();
        }
    }
}