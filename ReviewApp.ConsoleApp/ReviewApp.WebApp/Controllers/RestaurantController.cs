using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReviewApp.Domain;
using ReviewApp.DataAccess;
using ReviewApp.DataAccess.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace ReviewApp.WebApp.Controllers
{
    
    public class RestaurantController : Controller
    {
        private readonly IReviewRepo _repo;
        
        public RestaurantController(IReviewRepo repo)
        {
            _repo = repo;
        }
        public static int restaurantid;
        //Get all list of restaurants
        public IActionResult Index()
        {
            var restaurants = _repo.GetAllRestaurants();

            return View(restaurants);
        }

        //Search for a restaurant
        public IActionResult SearchForARestaurantForm()
        {
            return View();
        }
        //Show the search results
        public IActionResult ShowSearchResults(string searchString)
        {
             if (searchString == null)
             {
                 return NotFound();
             }
             var foundRestaurant = _repo.FindARestaurant(searchString);
             
             if (foundRestaurant == null)
             {
                 return NotFound();
             }
             //add all found restaurants to List<>
             List<ReviewApp.Domain.Restaurant> restaurants = new List<ReviewApp.Domain.Restaurant>();
             restaurants.Add(foundRestaurant);

             return View("Index", restaurants);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        //Get restaurant details
        public IActionResult Details2(int? id)
        {
            var restaurant = _repo.GetAllRestaurants().First(x => x.Id == id);
            
            return View(restaurant.Name);
        }
        public IActionResult Details(int id)
        {
            List<ReviewApp.Domain.ReviewJoin> reviewjoins = _repo.GetReviewJoins();
            var restaurant = _repo.GetAllRestaurants().First(x => x.Id == id);
            RestaurantController.restaurantid = restaurant.Id;

            decimal averagerating = AverageRating(restaurant.Name);

            //get all reviews belonging to the restaurant
            for (int i = 0; i < reviewjoins.Count; i++)
            {
                if (reviewjoins[i].RestaurantId == restaurant.Id)
                {
                    var foundReview = _repo.SearchReviewByReviewId(reviewjoins[i].ReviewId);
                    return View(foundReview);
                }
            }
            return View();
        }

        //Average rating
        public decimal AverageRating(string restaurantname)
        {
            decimal sum = 0;
            int n = 1;
            var foundRestaurant = _repo.FindARestaurant(restaurantname);
            List<ReviewApp.Domain.ReviewJoin> reviewjoins = _repo.GetReviewJoins();
            for (int i = 0; i < reviewjoins.Count; i++)
            {
                if (reviewjoins[i].RestaurantId == foundRestaurant.Id)
                {
                    Domain.Review foundReview = _repo.SearchReviewByReviewId(reviewjoins[i].ReviewId);
                    sum += foundReview.Rating;
                    n += 1;
                }
            }
            decimal averageRating = Math.Round(sum/n, 2);
            return averageRating;
        }

        //Leave reviews
        public IActionResult Review(int id)
        {
            return View(_repo.GetReviews().First(x => x.Id == id));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult LeaveReviews()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LeaveReviews(ReviewApp.Domain.Review review)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            
            int customerId = UserController.userid;//CustomerId??
            int restaurantId = RestaurantController.restaurantid;//RestaurantId??
            //If customerId = 0 that means they were logged out, please log in again
            
            if (customerId == 0) return View("ErrorMessage", model: "Please logout and log in again!");
            _repo.AddReview(review);
            List<ReviewApp.Domain.Review> reviews = _repo.GetReviews();
            int id = reviews[reviews.Count - 1].Id;

            
            ReviewApp.Domain.ReviewJoin reviewjoin = new ReviewApp.Domain.ReviewJoin(restaurantId, customerId, id);
            _repo.AddAReviewJoin(reviewjoin);

            return RedirectToAction("Review", new { id });
        }
       
    }
}
