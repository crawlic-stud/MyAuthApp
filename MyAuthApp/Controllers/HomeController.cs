using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyAuthApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using MyAuthApp.Areas.Identity.Data;
using MyAuthApp.Data;
using Microsoft.AspNetCore.Http;

namespace MyAuthApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UsersDbContext _context;

        public HomeController(ILogger<HomeController> logger, 
            UserManager<User> userManager, UsersDbContext context, SignInManager<User> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var validated = await Validate();
            if (!validated)
            {
                return Redirect("Index");
            }
            return View();
        }

        public async Task<bool> Validate()
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            _logger.LogInformation("check started");
            if (user is null)
            {
                _logger.LogInformation("user is deleted");
                await _signInManager.SignOutAsync();
                return false;
            }
            else if (user.IsBlocked)
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("user is blocked");
                return false;
            }
            _logger.LogInformation("check ended");
            return true;
        }

        [HttpPost]
        public ActionResult Submit(IFormCollection fc)
        {
            string action = fc["action"];
            string[] Ids = fc["user"].ToString().Split(",");

            _logger.LogInformation(Ids.ToString() + "   " + action);

            switch (action)
            {
                case "Delete":
                    return Delete(Ids);
                case "Block":
                    return Block(Ids);
                case "Unblock":
                    return Unblock(Ids);
            }
            return Redirect("Index");
        }

        [HttpPost]
        public ActionResult Delete(string[] Ids)
        {
            foreach (var Id in Ids)
            {
                if (Id == "on") { continue; }
                foreach (var user in _context.Users.ToList())
                {
                    if (user.Id == Id)
                    {
                        _context.Remove(user);
                        _context.SaveChanges();
                    }
                }
            }

            return Redirect("Index");
        }

        [HttpPost]
        public ActionResult Block(string[] Ids)
        {
            foreach (var Id in Ids)
            {
                if (Id == "on") { continue; }
                foreach (var user in _context.Users.ToList())
                {
                    if (user.Id == Id)
                    {
                        user.IsBlocked = true;
                        _context.Update(user);
                        _context.SaveChanges();
                    } 
                }
            }

            return Redirect("Index");
        }

        [HttpPost]
        public ActionResult Unblock(string[] Ids)
        {

            foreach (var Id in Ids)
            {
                if (Id == "on") { continue; }
                foreach (var user in _context.Users.ToList())
                {
                    if (user.Id == Id)
                    {
                        user.IsBlocked = false;
                        _context.Update(user);
                        _context.SaveChanges();
                    }
                }
            }
            return Redirect("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}
