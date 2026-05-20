using QRCoder;

namespace Recensement.API.Services
{
    public class QrService
{
    public string GenerateQrCode(string secret)
    {
        using QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(secret, QRCodeGenerator.ECCLevel.Q);
        Base64QRCode qrCode = new Base64QRCode(qrCodeData);
        return qrCode.GetGraphic(20); // Mamerina string base64 azo ampiasaina amin'ny <img>
    }
}
}