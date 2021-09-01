using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReviewApp.Domain;

namespace ReviewApp.WebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IReviewRepo _repo;
        public UserController(IReviewRepo repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            return View("Welcome to review app");
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
        public IActionResult CreateAnAccount(ReviewApp.Domain.Customer customer)
        {
            _repo.AddAUser(customer);
            List<ReviewApp.Domain.Customer> customers = _repo.GetAllCustomers();
            int id = customers[customers.Count - 1].Id;

            return RedirectToAction("Details", new { id });
        }

        //Log-in
        public IActionResult LogInForm()
        {
            return View();
        }
        public IActionResult LogIn(string username, string password)
        {
            //usernameinput and passwordinput
            var user = _repo.SearchUsersByUserName(username);
            if((user.UserName != null ) && (user.Password != null))
            {
                return NotFound();//should have a error message
            }
            else
            {
                return RedirectToAction("Index");
            }
            //return View();
        }
    }
}
