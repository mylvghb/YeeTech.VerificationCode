using System;

namespace YeeTech.VerificationCode
{
    [Flags]
    public enum GeneralCodeFlags : byte
    {
        Number = 1,

        Letter = 2,

        LowerLetter = 4,

        UpperLetter = 8,

        Chinese = 16
    }
}