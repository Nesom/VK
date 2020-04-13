using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.IO;
using InfProj.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace RolesApp.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext db;
        public HomeController(ApplicationContext context)
        {
            db = context;
        }
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> Index()
        {
            var user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);

            if (user == null)
                return RedirectToAction("Login", "Account");
            var posts = await db.Posts.ToListAsync();
            return View(new HomePageModel(user,posts));
        }
        [Authorize(Roles = "admin")]
        public IActionResult About()
        {
            return Content("Вход только для администратора");
        }
        public IActionResult Accept(string who)
        {
            var user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            var friend = db.Users.FirstOrDefault(u => u.Login == who);

            user.Friends.Add(friend);
            friend.Friends.Add(user);

            user.Requests.Remove(friend);
            //var request = db.Requests.FirstOrDefault(r => r.Who == who && r.Whom == User.Identity.Name);

            //db.Requests.Remove(request);
            db.Users.Update(user);
            db.Users.Update(friend);

            db.SaveChanges();
            return RedirectToAction("Friends");
        }
        public IActionResult Decline(string who)
        {
            var user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            var friend = db.Users.FirstOrDefault(u => u.Login == who);

            user.Requests.Remove(friend);
            db.Users.Update(user);

            db.SaveChanges();
            return RedirectToAction("Friends");
            //var request = db.Requests.FirstOrDefault(r => r.Who == who && r.Whom == User.Identity.Name);
            //db.Requests.Remove(request);
            //db.SaveChanges();
            //return RedirectToAction("Friends");
        }
        public IActionResult FriendRequests()
        {
            var requests = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name).Requests;
            return View(requests);
        }
        [HttpGet]
        public async Task<IActionResult> Friends()
        {
            var friends = await db.Users.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
            return View( friends.Friends);
        }
        [HttpPost]
        public IActionResult Friends(string login)
        {
            var user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            var friend = db.Users.FirstOrDefault(u => u.Login == login);
            //if (db.Requests.FirstOrDefault(u => u == new FriendRequest { Who = login, Whom = User.Identity.Name }) != null)
            if (user.Requests.Contains(friend))
            {
                user.Friends.Add(friend);
                friend.Friends.Add(user);
                if (user == friend) return RedirectToAction("Friends", "Home");
                user.Requests.Remove(friend);
                //db.Requests.Remove(new FriendRequest { Who = login, Whom = User.Identity.Name });
                db.Users.Update(user);
                db.Users.Update(friend);
            }
            else
            {
                friend.Requests.Add(user);
                db.Users.Update(friend);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Posts()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(IFormFile uploadImage, string text, string name)
        {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(uploadImage.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)uploadImage.Length);
                }
            var t = new PostModel {Name=name, Time=DateTime.Now, Text = text, Owner = User.Identity.Name, Image = imageData };
                db.Posts.Add(t);
                db.SaveChanges();

                return RedirectToAction("Index");
        }
    }
}