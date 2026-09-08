using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using TMS.Core.Auth;

namespace TMS.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string userType, string? returnUrl = null)
        {
            bool success;
            UserSession? session;
            string errorMessage;

            if (string.Equals(userType, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                success = _authService.AuthenticateAdmin(username, password, out session, out errorMessage);
            }
            else
            {
                success = _authService.AuthenticateUser(username, password, out session, out errorMessage);
            }

            if (!success || session == null)
            {
                ViewBag.Error = errorMessage;
                ViewBag.Username = username;
                ViewBag.UserType = userType;
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, session.UserId.ToString()),
                new Claim(ClaimTypes.Name, session.Username),
                new Claim("FullName", session.FullName),
                new Claim(ClaimTypes.Email, session.Email),
                new Claim(ClaimTypes.Role, session.IsAdmin ? "Admin" : "User"),
                new Claim("Department", session.Department)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(
            string fullName,
            DateTime? dob,
            string gender,
            string email,
            string phone,
            string address,
            string department,
            string course,
            string yearOfJoining,
            string collegeOrOrg,
            string username,
            string password,
            string confirmPassword)
        {
            bool ok = _authService.CreateUser(
                fullName, dob, gender, email, phone, address, department,
                course, yearOfJoining, collegeOrOrg, username, password, confirmPassword,
                "Active", out string error);

            if (!ok)
            {
                ViewBag.Error = error;
                return View();
            }

            TempData["SuccessMessage"] = "Account registered successfully! You can now log in.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string username, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                ViewBag.Error = "Password must be at least 6 characters long.";
                return View();
            }

            bool ok = _authService.ResetPassword(username, newPassword, out string error);
            if (!ok)
            {
                ViewBag.Error = error;
                return View();
            }

            TempData["SuccessMessage"] = "Password has been successfully updated. Please log in.";
            return RedirectToAction("Login");
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
