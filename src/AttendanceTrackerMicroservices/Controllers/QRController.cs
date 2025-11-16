using AttendanceTrackerMicroservices.Models.ViewModels;
using AttendanceTrackerMicroservices.Service.IService;
using AttendanceTrackerMicroservices.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using QRCoder;
using static QRCoder.PayloadGenerator.WiFi;

namespace AttendanceTrackerMicroservices.Controllers
{
    public class QRController : Controller
    {
        private readonly IDistributedCache _cache;
        private readonly IAuthService _authService;
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

        public IActionResult UnauthorizedAction(string message)
        {
            TempData["ErrorMessage"] = message;
            return View("OnFailRecord", message);
        }

        /// <summary>
        /// Display the authentication login page for user to perform check-in and check-out
        /// </summary>
        /// <param name="token">The authentication token to validate if it is expired</param>
        /// <returns>
        /// <item>An unauthorized response if the token is invalid or expired <see cref="UnauthorizedAction(string)"/></item>
        /// <item>The authentication view with the token populated in the view model if token validation succeeds</item>
        /// </returns>
        /// <remarks>
        /// This method performs the following steps:
        /// <list type="number">
        /// <item>Validates the token using <see cref="IsTokenValid(string)"/></item>
        /// <item>Returns an unauthorized action with an error message if validation fails</item>
        /// <item>Creates an <see cref="AuthenticationVM"/> instance with the valid token</item>
        /// <item>Returns the authentication view with the populated view model</item>
        /// </list>
        /// </remarks>
        public IActionResult Authentication(string token)
        {
            if (!IsTokenValid(token))
            {
                return UnauthorizedAction("Token has expired. Please rescan the QR code.");
            }

            AuthenticationVM authenticationVM = new()
            {
                Token = token,
            };

            return View(authenticationVM);
        }

        [HttpPost]
        public async Task<IActionResult> Authentication(AuthenticationVM model)
        {
            // Check if token has expired
            if (!IsTokenValid(model.Token))
            {
                return UnauthorizedAction("Token has expired. Please rescan the QR code.");
            }

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    // Update a new token after each successful authentication for check in/out
                    // Ensures only one thread enters at a time.
                    // In case multiple users submitted request at the same time
                    lock (_sessionTokenLock)
                    {
                        _cache.SetString(SD.GUID_SESSION, GenerateNewToken());
                        GenerateQRCode();
                    }

                    // force refresh the page on all clients
                    await _hubContext.Clients.All.SendAsync("RefreshPage");

                    // Unfortunately, RedirectToAction does not support passing complex object
                    // so we have to make use of TempData[] here and collect the data on the next
                    // RecordAttendance() method
                    TempData["Token"] = model.Token;
                    return RedirectToAction("RecordAttendance", "Home", new { area = "QR" });
                }
                else
                {
                    return RedirectToAction("UnauthorizedAction", "Home",
                        new { area = "QR", message = "Incorrect username and password!" });
                }
            }

            // If we got this far, something failed, redisplay form
            return RedirectToAction("UnauthorizedAction", "Home",
                new { area = "QR", message = "Something went wrong, please rescan QR and try again..." });
        }

        // PRIVATE METHODS

        /// <summary>
        /// Validates whether the provided token is valid and matches the current session.
        /// Otherwise, the token is expired since new token is generated 
        /// <see cref="_cache.SetString(SD.GUID_SESSION, GenerateNewToken())"/> 
        /// new valid 
        /// </summary>
        /// <param name="token">The authentication token to validate.</param>
        /// <returns>
        /// <c>true</c> if the token is not null/empty and matches the cached session GUID; 
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Token will be <c>invalid</c> if user has yet to generate their token or expired 
        /// </remarks>
        private bool IsTokenValid(string token)
        {
            return (!string.IsNullOrEmpty(token)) && (_cache.GetString(SD.GUID_SESSION) == token);
        }

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
            string authenticationUrl = Url.Action(
                "Authentication",
                "QR",
                new { token = _cache.GetString(SD.GUID_SESSION) },
                Request.Scheme,
                Request.Host.ToString()
            );
                //$"{Url.Action("Authentication", "QR", Request.Scheme, Request.Host.ToString())}?token={_cache.GetString(SD.GUID_SESSION)}";

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
