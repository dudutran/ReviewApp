using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReviewApp.Domain;
using ReviewApp.DataAccess;
using ReviewApp.DataAccess.Entities;

namespace ReviewApp.WebApp.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly IReviewRepo _repo;
        private readonly ReviewDbContext _context;
        public RestaurantController(IReviewRepo repo, ReviewDbContext context)
        {
            _repo = repo;
            _context = context;
        }
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

        //Get restaurant details
        public IActionResult Details2(int? id)
        {
            //RestaurantId=>find and match restaurantId in jointable, then we can take the reviewjoin.reviewId, then get review
            //List<ReviewApp.Domain.ReviewJoin> reviewjoins = _repo.GetReviewJoins();
            //for (int i = 0; i < reviewjoins.Count; i++)
            //{
            //    if (reviewjoins[i].RestaurantId == 1)
            //    {
            //        var foundReview = _repo.SearchReviewByReviewId(reviewjoins[i].ReviewId);
            //    }
            //}
            // return View(_repo.FindARestaurant(name));
            var restaurant = _repo.GetAllRestaurants().First(x => x.Id == id);
            
            return View(restaurant.Name);
        }
        public IActionResult Details(int id)
        {
            List<ReviewApp.Domain.ReviewJoin> reviewjoins = _repo.GetReviewJoins();
            var restaurant = _repo.GetAllRestaurants().First(x => x.Id == id);
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

        //Leave reviews
        public IActionResult Reviews(int id)
        {
            return View(_repo.GetReviews().First(x => x.Id == id));
            
        }
        [HttpGet]
        public IActionResult LeaveReviews()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LeaveReviews(ReviewApp.Domain.Review review)
        {
            _repo.AddReview(review);
            List<ReviewApp.Domain.Review> reviews = _repo.GetReviews();
            int id = reviews[reviews.Count - 1].Id;

            return RedirectToAction("Reviews", new { id });
        }
       
           
        
    }
}
