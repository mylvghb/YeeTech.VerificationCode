using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using NUnit.Framework;
using YeeTech.VerificationCode;
using YeeTech.VerificationCode.Interface;

namespace VerificationCode.Tests
{
    [TestFixture]
    public class GeneralVerificationCodeTests
    {
        [Test]
        public void UseDefaultConstrucotrGenerateANumberVerificationCode()
        {
            IVerificationCode verificationCode = new GeneralVerificationCode();
            var code = verificationCode.Generate();
            Assert.IsTrue(Regex.IsMatch(code, "^\\d{4}$"));
        }

        [Test]
        public void CheckNumberAndUpperLetter()
        {
            Assert.That(() =>
            {
                IVerificationCode verificationCode = new GeneralVerificationCode(
                    GeneralCodeFlags.Number | GeneralCodeFlags.UpperLetter,
                    6
                );
                var code = verificationCode.Generate();
                var pattern = new Regex("^(?=.*[A-Z])(?=.*[0-9])[A-Z0-9]+");
                return pattern.IsMatch(code) && code.Length == 6;
            });
        }

        [Test]
        public void ChineseVerificationCodePerformanceAnalyse()
        {
            IVerificationCode verificationCode = new GeneralVerificationCode(GeneralCodeFlags.Chinese);
            var watch = Stopwatch.StartNew();
            const int count = 1000000;
            for (var i = 0; i < count; i++)
            {
                verificationCode.Generate();
            }
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
        }
    }
}