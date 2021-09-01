using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using static QRCoder.PayloadGenerator;

namespace Mexty.MVVM.Model {
    public class QRGenerator {

        public Bitmap GenerarQR(string url) {
            var generator = new Url(url);
            string payload = generator.ToString();

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            var qrCodeAsBitmap = qrCode.GetGraphic(1);
            return qrCodeAsBitmap;
        }

    }
}
