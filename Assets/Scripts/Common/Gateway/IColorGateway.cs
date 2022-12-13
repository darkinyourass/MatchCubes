namespace Common.Gateway
{
    public interface IColorGateway
    {
        void SetColorValue(ColorType colorType);
        ColorType GetColorType(ColorType colorType);
    }
}