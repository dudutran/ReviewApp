using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReviewApp.Domain;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;


namespace ReviewApp.WebApp.Controllers
{
    public class UserController : Controller
    {
        public static int userid;
        private readonly IReviewRepo _repo;
        public UserController(IReviewRepo repo)
        {
            _repo = repo;
        }

        [Authorize]
        public IActionResult Index()
        {
            TempData["Message"] = "you login successfully!";
            return View();
        }
        public IActionResult Details(int id)
        {

            return View(_repo.GetAllCustomers().First(x => x.Id == id));
        }
        [HttpGet]
        public IActionResult CreateAnAccount()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAnAccount(ReviewApp.Domain.Customer customer)
        {
            //validation check
            //if(!ModelState.IsValid)
            //{
            //    return View();
            //}

            List<ReviewApp.Domain.Customer> customers = _repo.GetAllCustomers();
            
            int id = customers[customers.Count - 1].Id;
            try
            {
                _repo.AddAUser(customer);
            }
            catch( InvalidOperationException e)
            {
                ModelState.AddModelError("Email", e.Message);
                ModelState.AddModelError("Username", e.Message);
                return View();
            }
           
            return RedirectToAction("Details", new { id });
        }

        //Log-in
        
        [HttpGet("login")]
        public IActionResult LogIn(string returnUrl)
        {
            
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Validate(string username, string password, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            var user = _repo.SearchUsersByUserName(username);
            userid = user.Id; //stored this value for add reference to ReviewJoin table

            if (username == user.UserName && password == user.Password)
            {
                var claims = new List<Claim>();
                claims.Add(new Claim("username", username));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, username));
                
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);
                if (returnUrl == null) return RedirectToAction("Index");
                return Redirect(returnUrl);
                
            }
            
            TempData["Error"] = "Login fail. Password or Username is invalid";
            return View("login");
        }

        //log out
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        //Denied page if a user is not an admin
        [HttpGet("denied")]
        public IActionResult Denied()
        {
            return View();
        }
        

    }
}
