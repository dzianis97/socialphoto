using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using social.Models;
using System.IO;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Data.SqlClient;
using System.Configuration;
using Dapper;

namespace social.Controllers
{
    public class HomeController : Controller
    {

        private SqlConnection connection;
        public HomeController()
            : this(new UserManager<User>(new Identity.UserStore()))
        {
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }

        public HomeController(UserManager<User> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<User> UserManager { get; private set; }

        public ActionResult Index()
        {
            //string curuserid = User.Identity.GetUserId();
            try
            {

                string MYuserId = User.Identity.GetUserId();
                User currentUser = connection.Query<User>("select * from Users where UserId = @userId", new { userId = MYuserId }).SingleOrDefault();
                ViewBag.UserName = currentUser.Email;
                if (currentUser.Photo == null)
                    ViewBag.AvatarURL = "http://www.w3schools.com/w3images/avatar3.png";
                else
                    ViewBag.AvatarURL = Url.Content("~/Images/") + "/" + currentUser.Photo;
            }
            catch (System.NullReferenceException)
            {
                RedirectToAction("Login", "Account");
            }
            IEnumerable<Like> likes = connection.Query<Like>("select * from Likes");
            
            ViewBag.Likes = likes;
            List<Post> posts=new List<Post>();
            IEnumerable<Photo> photos = connection.Query<Photo>("select * from Photos ORDER BY PublishedTime DESC");
            foreach (Photo ph in photos.Take(20))
            {
                if (ph.isPrivate == false)
                {
                    User postUser = connection.Query<User>("select * from Users where UserId = @userId", new { userId = ph.SenderId }).SingleOrDefault();
                    Post ps = new Post();
                    ps.Id = ph.Id;
                    ps.Content = ph.Content;
                    ps.Title = ph.Title;
                    ps.Image = ph.Image;
                    ps.LikesCount = ph.LikesCount;
                    ps.PublishedTime = ph.PublishedTime.ToString();
                    ps.SenderId = ph.SenderId;
                    ps.SenderName = postUser.SurName;
                    ps.SenderPhoto = postUser.Photo;
                    if (postUser.Photo == null)
                        ps.SenderPhoto = "avatar3.png";
                    posts.Add(ps);
                }
                else
                {
                    int acess = connection.Query<Like>("select * from PrivatePhotoAcesses where PhotoId = @photoId AND UserId = @userId", new { photoId = ph.Id, userId = User.Identity.GetUserId() }).Count();
                    if (acess > 0)
                    {
                        User postUser = connection.Query<User>("select * from Users where UserId = @userId", new { userId = ph.SenderId }).SingleOrDefault();
                        Post ps = new Post();
                        ps.Id = ph.Id;
                        ps.Content = ph.Content;
                        ps.Title = ph.Title;
                        ps.Image = ph.Image;
                        ps.LikesCount = ph.LikesCount;
                        ps.PublishedTime = ph.PublishedTime.ToString();
                        ps.SenderId = ph.SenderId;
                        ps.SenderName = postUser.SurName;
                        ps.SenderPhoto = postUser.Photo;
                        if (postUser.Photo == null)
                            ps.SenderPhoto = "avatar3.png";
                        posts.Add(ps);
                    }
                }
            }
            
            return View(posts);
            //return View(db.Photos.OrderByDescending(x => x.PublishedTime).Take(30));
        }

        [Authorize]
        public string Like(int id)
        {
            
            string usr = User.Identity.GetUserId();
            Photo ph = connection.Query<Photo>("select * from Photos where Id = @photoId", new { photoId = id }).SingleOrDefault();
            int likes = connection.Query<Like>("select * from Likes where PhotoId = @photoId AND UserId = @userId", new { photoId = id, userId = usr }).Count();
            if (likes > 0)
            {
                return "You are already liked";
            }
            else
            {
                Like like = new Like();
                like.PhotoId = id;
                like.UserId = User.Identity.GetUserId();
                connection.Execute("insert into Likes(PhotoId, UserId) values(@photoId, @userId)", like);
                ph.LikesCount++;
                connection.Execute("UPDATE Photos SET LikesCount=@likesCounts WHERE id=@photoId", new { photoId = ph.Id, likesCounts = ph.LikesCount });
            }

            return ph.LikesCount.ToString() + " Likes";
            
        
        }
 

        public ActionResult MobileView()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}