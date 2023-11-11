using Group5_MusicPlayer.Data;
using Group5_MusicPlayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Net;

namespace Group5_MusicPlayer.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly MusicPlayerDbContext context;

        public AuthorizationController(MusicPlayerDbContext context1)
        {
            context = context1;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public IActionResult Login()
        {
            return View("/Views/Authorizations/Login.cshtml");
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            User user = context.Users.FirstOrDefault(x => x.Email == email
                                                        && x.Password == password);
            if (user == null)
            {
                ViewBag.error = "No account found! Please check your inputs!";
                return View("/Views/Authorizations/Login.cshtml");
            }
            if (user.Role == 2)
            {
                ViewBag.error = "Your account is suspended! Please contact admin for more information!";
                return View("/Views/Authorizations/Login.cshtml");
            }

            //var songs = context.Songs.Include(s => s.Author).Include(s => s.Category).Where(s => s.IsPrivate == false);
            if (user.Role == 0)
            {
                HttpContext.Session.SetString("ID", user.UserId.ToString());
                HttpContext.Session.SetString("Account", "Admin");
                return RedirectToAction("Index", "Home");
            }      
            HttpContext.Session.SetString("Account", "User");
            HttpContext.Session.SetString("ID", user.UserId.ToString());
            return RedirectToAction("SetPlayListIdSession", "PlayLists", new { userid = user.UserId});

        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public IActionResult Register()
        {
            return View("/Views/Authorizations/Register.cshtml");
        }
        [HttpPost]
        public IActionResult Register(string username, string email, string password, string confirmPassword, string phone)
        {
            User existingEmail = context.Users.FirstOrDefault(u => u.Email == email);
            if (existingEmail != null)
            {
                ViewBag.MessageError = "Email taken!";
                return View("/Views/Authorizations/Register.cshtml");
            }
            if (!isValidEmail(email))
            {
                ViewBag.MessageError = "Invalid Email!";
                return View("/Views/Authorizations/Register.cshtml");
            }
            if (password.Length < 6)
            {
                ViewBag.MessageError = "Password must be longer than 6 characters";
                return View("/Views/Authorizations/Register.cshtml");
            }

            if (!password.Equals(confirmPassword))
            {
                ViewBag.MessageError = "Passwords do not match! Please confirm!";
                return View("/Views/Authorizations/Register.cshtml");
            }
            try
            {
                User appUser = new User()
                {
                    UserName = username,
                    Email = email,
                    Password = password,
                    Phone = phone,
                    Role = 1
                };
                context.Add(appUser);
                context.SaveChanges();
                User userToAddPlaylist = context.Users.FirstOrDefault(u => u.Email == email);
                Playlist playlist = new Playlist()
                {
                    Title = "Personal Playlist",
                    UserId = userToAddPlaylist.UserId
                };
                context.Add(playlist);
                context.SaveChanges();
                ViewBag.RegisterMessage = "Registered Successfully";
                return View("/Views/Authorizations/Login.cshtml");
            }
            catch (Exception ex)
            {
                ViewBag.MessageError = username + " " + phone + " " + email + " " + password;
                return View("/Views/Authorizations/Register.cshtml");
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public IActionResult Logout()
        {
            var user = HttpContext.Session.GetString("Account");
            if (user != null)
            {
                HttpContext.Session.Clear();
            }
            var userID = HttpContext.Session.GetString("ID");
            if (userID != null)
            {
                HttpContext.Session.Clear();
            }
            return RedirectToAction("Index", "Home");
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        bool isValidEmail(string input)
        {
            try
            {
                var email = new System.Net.Mail.MailAddress(input);
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
