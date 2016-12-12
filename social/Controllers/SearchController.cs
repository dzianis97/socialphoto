using Dapper;
using Microsoft.AspNet.Identity;
using social.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace social.Controllers
{
    public class SearchController : Controller
    {
        private SqlConnection connection;
        public SearchController()
        {
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }
        // GET: Search
        [Authorize]
        [HttpGet]
        public ActionResult Index(string searchreq)
        {
            string MYuserId = User.Identity.GetUserId();
            User currentUser = connection.Query<User>("select * from Users where UserId = @userId", new { userId = MYuserId }).SingleOrDefault();
            ViewBag.UserName = currentUser.Email;
            if (currentUser.Photo == null)
                ViewBag.AvatarURL = "http://www.w3schools.com/w3images/avatar3.png";
            else
                ViewBag.AvatarURL = Url.Content("~/Images/") + "/" + currentUser.Photo;
            if (searchreq != null)
            {
                ViewBag.searchreq = searchreq;
                List<Post> posts = new List<Post>();
                IEnumerable<Photo> photos = connection.Query<Photo>("select * from Photos where Title LIKE '%'+@request+'%' OR Content LIKE '%'+@request+'%' ORDER BY PublishedTime DESC", new { request = searchreq.Trim() });
                foreach (Photo ph in photos)
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
                }

                //var resultphotos = db.Photos.Where(p => p.Title.Contains(searchreq)).OrderByDescending(x => x.PublishedTime);
                return View(posts);
            }
            else
            {
                ViewBag.searchreq = "write request here";
                List<Post> posts = new List<Post>();
                IEnumerable<Photo> photos = connection.Query<Photo>("select * from Photos ORDER BY PublishedTime DESC");
                foreach (Photo ph in photos)
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
                }
                return View(posts);
            }
            return View();

        }

    }
}