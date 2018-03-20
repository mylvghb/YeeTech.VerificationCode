namespace YeeTech.VerificationCode.Interface
{
    public interface IImageVerificationCodeFactory : IVerificationCodeFactory
    {
        int Width { get; set; }

        int Height { get; set; }

        ImageDrawCompletedHandlerDelegate ImageDrawCompletedHandler { get; set; }

        byte[] Draw();
    }
}