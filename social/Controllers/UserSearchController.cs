using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using Microsoft.AspNet.Identity;
using social.Models;
using System.Data.SqlClient;
using System.Configuration;

namespace social.Controllers
{
    public class UserSearchController : Controller
    {
        private SqlConnection connection;
        public UserSearchController()
        {
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }
        // GET: UserSearch
        [Authorize]
        public ActionResult Index()
        {
            string MYuserId = User.Identity.GetUserId();
            User currentUser = connection.Query<User>("select * from Users where UserId = @userId", new { userId = MYuserId }).SingleOrDefault();
            ViewBag.UserName = currentUser.Email;
            if (currentUser.Photo == null)
                ViewBag.AvatarURL = "http://www.w3schools.com/w3images/avatar3.png";
            else
                ViewBag.AvatarURL = Url.Content("~/Images/") + "/" + currentUser.Photo;
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SearchModel searchModel)
        {
            IEnumerable<User> users = connection.Query<User>("select * from Users");
            if (!String.IsNullOrEmpty(searchModel.UserName))
                users = connection.Query<User>("select * from Users where UserName = @userName", new { userName = searchModel.UserName });
            if (!String.IsNullOrEmpty(searchModel.SurName))
                users = connection.Query<User>("select * from Users where SurName = @userName", new { userName = searchModel.SurName });
            if ((searchModel.AgeMin.Year > 1800) && (searchModel.AgeMin.Year < DateTime.Now.Year))
            {
                try
                {
                    users = connection.Query<User>("select * from Users where BirthDay >= @userName", new { userName = searchModel.AgeMin });
                }
                catch (System.Data.SqlTypes.SqlTypeException)
                {
                    users = connection.Query<User>("select * from Users where BirthDay >= @userName", new { userName = DateTime.Parse("1/1/1754 12:00:00") });
                }
            }

            if ((searchModel.AgeMax.Year > 1800) && (searchModel.AgeMax.Year < DateTime.Now.Year))
            {
                try
                {
                    users = connection.Query<User>("select * from Users where BirthDay <= @userName", new { userName = searchModel.AgeMax });
                }
                catch (System.Data.SqlTypes.SqlTypeException)
                {
                    users = connection.Query<User>("select * from Users where BirthDay <= @userName", new { userName = DateTime.Now });
                }
            }
            if (!String.IsNullOrEmpty(searchModel.Gender))
                users = connection.Query<User>("select * from Users where Gender = @userName", new { userName = searchModel.Gender });
            return View(users);

            //return View();
        }
    }
}