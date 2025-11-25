using AttendanceTrackerMicroservices.Hubs;
using AttendanceTrackerMicroservices.Models;
using AttendanceTrackerMicroservices.Models.ViewModels;
using AttendanceTrackerMicroservices.Service.IService;
using AttendanceTrackerMicroservices.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using QRCoder;
using System.Text.Json;
using static QRCoder.PayloadGenerator.WiFi;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AttendanceTrackerMicroservices.Controllers
{
    public class QRController : Controller
    {
        private readonly IDistributedCache _cache;
        private readonly IAuthService _authService;
        private readonly IHubContext<RefreshHub> _hubContext;
        private static readonly object _sessionTokenLock = new object();

        public QRController(
            IDistributedCache cache,
            IAuthService authService,
            IHubContext<RefreshHub> hubContext)
        {
            _cache = cache;
            _authService = authService;
            _hubContext = hubContext;
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
                LoginRequestDTO loginRequestDTO = new()
                {
                    UserName = model.UserName,
                    Password = model.Password
                };
                
                ResponseDTO response = await _authService.ValidateUser(loginRequestDTO);
                if (response != null && response.IsSuccess)
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

                    // Map the response data to UserDTO
                    string jsonString = JsonConvert.SerializeObject(response.Result);
                    UserDTO user = JsonConvert.DeserializeObject<UserDTO>(jsonString);

                    // Unfortunately, RedirectToAction does not support passing complex object
                    // so we have to make use of TempData[] here and collect the data on the next
                    // RecordAttendance() method
                    TempData["Token"] = model.Token;
                    TempData["UserInfo"] = JsonConvert.SerializeObject(user);
                    return RedirectToAction("RecordAttendance", "QR");
                }
                else
                {
                    return UnauthorizedAction("Incorrect password and username!");
                }
            }
            // If we got this far, something failed, redisplay form
            return UnauthorizedAction("Something went wrong, please rescan QR and try again...");
        }

        public IActionResult RecordAttendance()
        {
            UserDTO userDTO = null;
            if (TempData["UserInfo"] != null)
            {
                string userJson = TempData["UserInfo"] as string;
                userDTO = JsonConvert.DeserializeObject<UserDTO>(userJson);
            }

            AuthenticationVM model = new AuthenticationVM
            {
                Token = TempData["Token"] as string,
                IsCheckIn = UserShouldCheckIn(userDTO?.ID),
                Id = userDTO?.ID
            };

            return View(model);
        }

        // PRIVATE METHODS

        /// <summary>
        /// Validates whether the provided token is valid and matches the current session.
        /// Otherwise, the token is expired since new token is generated 
        /// <see cref="_cache.SetString(SD.GUID_SESSION, GenerateNewToken())"/> 
        /// new valid 
        /// </summary>
        ///
        /// <param name="token">The authentication token to validate.</param>
        ///
        /// <returns>
        /// <c>true</c> if the token is not null/empty and matches the cached session GUID; 
        /// otherwise, <c>false</c>.
        /// </returns>
        ///
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
        ///
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
        ///
        /// <returns>
        /// A <c>string</c> representation of a newly generated globally unique identifier (GUID).
        /// </returns>
        ///
        /// <remarks>
        /// This method is used for generating authentication tokens
        /// </remarks>
        private string GenerateNewToken()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Determines if the current user should check in based on attendance records.
        /// This method retrieves attendance records for the logged-in user on the current date.
        /// It checks if there are any existing records with an unset check-out time.
        /// If such a record exists, the user is considered already checked in and should not check in again.
        /// </summary>
        ///
        /// <param name="userId">User ID</param>
        ///
        /// <note>
        /// The method assumes that `CheckOut == DateTime.MinValue` indicates an unchecked-out record.
        /// </note>
        ///
        /// <returns>
        /// Returns `true` if the user should check in, otherwise `false`.
        /// </returns>
        private bool UserShouldCheckIn(string userId)
        {
            //var userAttendanceRecords = _unitOfWork.DailyAttendanceRecord.GetAll(
            //    a => a.EmployeeId == currentUserId && a.CheckIn.Date == DateTime.Today, includeProperties: "Employee");

            //bool userShouldCheckIn = true;

            //if (userAttendanceRecords == null)
            //{
            //    // No check-in records found for today, so this is user's first check-in of the day
            //    return userShouldCheckIn;
            //}

            //// When enter here, it means the user has already checked in today
            //// Search if there are any records pending for check out. Otherwise, this is a new check-in request
            //foreach (var record in userAttendanceRecords)
            //{
            //    if (record.CheckOut == DateTime.MinValue)
            //    {
            //        // Invalid check out record indicating that there is a pending check out
            //        // So abort the check-in process
            //        userShouldCheckIn = false;
            //        break;
            //    }
            //}

            //return userShouldCheckIn;
            return true;
        }
    }
}
