using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Intrepid.AspNetCore.Identity.Admin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;

namespace Intrepid.AspNetCore.Identity.Admin.Controllers
{
    
    public class HomeController : BaseController
    {
        public UserManager<IdentityUser> Manager { get; }

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> manager)
        {
            this.Manager = manager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> CreateDummyUser()
        {
            
            var user = new IdentityUser();
            user.Email = "schang2@softtect.net";
            user.EmailConfirmed = true;
            
            user.Id = Guid.NewGuid().ToString();
            user.UserName = user.Email;
            await this.Manager.CreateAsync(user, "Fr1edChicken16!");
            return Ok();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
