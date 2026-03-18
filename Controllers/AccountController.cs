using Microsoft.AspNetCore.Mvc;
using FirstWebApp.Models;
using FirstWebApp.Data;
using FirstWebApp.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace FirstWebApp.Controllers
{
    public class AccountsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordService _passwordService;
        private readonly EmailService _emailService;

        public AccountsController(
            AppDbContext context,
            PasswordService passwordService,
            EmailService emailService)
        {
            _context = context;
            _passwordService = passwordService;
            _emailService = emailService;
        }

        // ================= LOGIN =================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.Email == model.Email);

            if (user != null &&
                !string.IsNullOrEmpty(model.Password) &&
                !string.IsNullOrEmpty(user.PasswordHash) &&
                _passwordService.VerifyPassword(model.Password, user.PasswordHash))
            {
                var otp = new Random().Next(100000, 999999).ToString();

                HttpContext.Session.SetString("login_otp", otp);
                HttpContext.Session.SetString("login_email", user.Email!);

                _emailService.SendOtp(user.Email!, otp);

                return RedirectToAction("VerifyOtp");
            }

            ViewBag.Error = "Invalid Email or Password";
            return View();
        }

        // ================= LOGIN OTP =================

        [HttpGet]
        public IActionResult VerifyOtp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(string otp)
        {
            var sessionOtp = HttpContext.Session.GetString("login_otp");
            var email = HttpContext.Session.GetString("login_email");

            if (sessionOtp == null || email == null)
            {
                return RedirectToAction("Login");
            }

            if (otp == sessionOtp)
            {
                HttpContext.Session.Remove("login_otp");

                var user = _context.Users
                    .FirstOrDefault(x => x.Email == email);

                if (user == null)
                    return RedirectToAction("Login");

                // ⭐ PRODUCTION CLAIMS
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Email!), // for identity
                    new Claim("FullName", user.Name ?? ""),
                    new Claim("Role", user.Role ?? "User")
                };

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
                    });

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid OTP";
            return View();
        }

        // ================= DASHBOARD =================

        [Authorize]
        public IActionResult Dashboard()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return RedirectToAction("Login");

            var user = _context.Users
                .FirstOrDefault(x => x.Id.ToString() == userId);

            ViewBag.UserName = user?.Name;

            return View();
        }

        // ================= PROFILE WIDGETS =================

        [Authorize]
        public IActionResult ProfileWidgets()
        {
            var users = _context.Users.ToList();

            ViewBag.UserName = User.FindFirst("FullName")?.Value;

            return View(users);
        }

        // ================= EDIT PROFILE =================

        [Authorize]
        [HttpGet]
        public IActionResult EditProfile(int id)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.Id == id);

            if (user == null)
                return RedirectToAction("ProfileWidgets");

            return View(user);
        }

        [Authorize]
        [HttpPost]
        public IActionResult EditProfile(User model, string? password)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.Id == model.Id);

            if (user == null)
                return RedirectToAction("ProfileWidgets");

            user.Name = model.Name;
            user.Email = model.Email;
            user.Phone = model.Phone;
            user.DOB = model.DOB;
            user.Role = model.Role;
            user.Status = model.Status;
            user.Updated_at = DateTime.Now;

            if (!string.IsNullOrEmpty(password))
            {
                user.PasswordHash = _passwordService.HashPassword(password);
            }

            _context.SaveChanges();

            TempData["success"] = "Profile Updated Successfully";

            return RedirectToAction("ProfileWidgets");
        }

        // ================= FORGOT PASSWORD =================

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                ViewBag.Error = "Email not found";
                return View();
            }

            var otp = new Random().Next(100000, 999999).ToString();

            HttpContext.Session.SetString("reset_otp", otp);
            HttpContext.Session.SetString("reset_email", email);

            _emailService.SendOtp(email, otp);

            return RedirectToAction("VerifyResetOtp");
        }

        // ================= RESET OTP =================

        [HttpGet]
        public IActionResult VerifyResetOtp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyResetOtp(string otp)
        {
            var sessionOtp = HttpContext.Session.GetString("reset_otp");

            if (otp == sessionOtp)
            {
                return RedirectToAction("ResetPassword");
            }

            ViewBag.Error = "Invalid OTP";
            return View();
        }

        // ================= RESET PASSWORD =================

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Password not match";
                return View();
            }

            var email = HttpContext.Session.GetString("reset_email");

            var user = _context.Users
                .FirstOrDefault(x => x.Email == email);

            if (user != null)
            {
                user.PasswordHash = _passwordService.HashPassword(password);
                user.Updated_at = DateTime.Now;
                _context.SaveChanges();
            }

            HttpContext.Session.Clear();

            TempData["Success"] = "Password Changed Successfully";

            return RedirectToAction("Login");
        }

        // ================= LOGOUT =================

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}