using AttendanceTrackerMicroservices.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using QRCoder;

namespace AttendanceTrackerMicroservices.Controllers
{
    public class QRController : Controller
    {
        private readonly IDistributedCache _cache;
        private static readonly object _sessionTokenLock = new object();

        public QRController(IDistributedCache cache)
        {
            _cache = cache;
        }

        public IActionResult Index()
        {
            // Ensures only one thread enters at a time.
            // In case multiple users submitted request at the same time
            lock (_sessionTokenLock)
            {
                // Generate and initialize unique GUID token for the QR code which expires when the user check in or check out
                // Used at the first time the app is loaded
                if (string.IsNullOrEmpty(_cache.GetString(SD.GUID_SESSION)))
                {
                    // Double-check inside the lock
                    if (string.IsNullOrEmpty(_cache.GetString(SD.GUID_SESSION)))
                    {
                        _cache.SetString(SD.GUID_SESSION, GenerateNewToken());
                    }
                }
                GenerateQRCode();
            }
            return View();
        }

        // PRIVATE METHODS

        /// <summary>
        /// Generates a QR code containing an authentication URL with an embedded session token.
        /// </summary>
        /// <remarks>
        /// This method performs the following steps:
        /// - Constructs an authentication URL using the current request scheme and a cached session token.
        /// - Converts the QR code to a PNG byte array and encodes it as a Base64 URI.
        /// - Stores the resulting image URI in <c>ViewBag.QrCodeUri</c> for rendering in the view.
        /// </remarks>
        private void GenerateQRCode()
        {
            // Define and embed token/session into authentication page URL
            string authenticationUrl =
                $"{Url.Action("Authentication", "Home", new { area = "QR" }, Request.Scheme)}?token={_cache.GetString(SD.GUID_SESSION)}";

            // QR Code Generation on Page Load
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeInfo = qrGenerator.CreateQrCode(authenticationUrl, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeInfo);
            byte[] qrCodeBytes = qrCode.GetGraphic(60);

            // Convert QR Code to Base64 URI
            string qrUri = $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
            ViewBag.QrCodeUri = qrUri;
        }

        /// <summary>
        /// Generates a new unique token using a GUID use to identify user in tracking attendance.
        /// </summary>
        /// <returns>
        /// A <c>string</c> representation of a newly generated globally unique identifier (GUID).
        /// </returns>
        /// <remarks>
        /// This method is used for generating authentication tokens
        private string GenerateNewToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
