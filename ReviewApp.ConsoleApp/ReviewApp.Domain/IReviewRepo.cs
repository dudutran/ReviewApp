using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewApp.Domain
{
        public interface IReviewRepo
        {
            Review AddReview(Review review);
            List<Review> GetReviews();
            Restaurant FindARestaurant(string name);
            List<Restaurant> GetAllRestaurants();
            Restaurant FindARestaurantByZipcode(string zipcode);
            List<Customer> GetAllCustomers();
            Customer AddAUser(Customer customer);
            Customer SearchUsersByUserName(string userName);
            Review SearchReviewByReviewId(int id);
            List<ReviewJoin> GetReviewJoins();
            
            ReviewJoin AddAReviewJoin(ReviewJoin reviewjoin);

    }
        
}
