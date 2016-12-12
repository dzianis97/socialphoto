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
    public class CreateController : Controller
    {
        private SqlConnection connection;
        public CreateController()
        {
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }
        // GET: Create
        private string SavePhotoAndGetName(HttpPostedFileBase file, string userName)
        {
            if (file != null)
            {
                string PhotoName = System.IO.Path.GetFileName(file.FileName);
                string physicalPath = Server.MapPath("~/PostedImages/" + userName + PhotoName);

                file.SaveAs(physicalPath);

                return userName + PhotoName;
            }
            else
                return "User.png";
        }
        [Authorize]
        public ActionResult Index()
        {
            string MYuserId = User.Identity.GetUserId();
            User currentUser = connection.Query<User>("select * from Users where UserId = @userId", new { userId = MYuserId }).SingleOrDefault();
            ViewBag.UserName = currentUser.UserName;
            if (currentUser.Photo == null)
                ViewBag.AvatarURL = "http://www.w3schools.com/w3images/avatar3.png";
            else
                ViewBag.AvatarURL = Url.Content("~/Images/") + "/" + currentUser.Photo;

            return View();
        }

        [HttpPost]
        public ActionResult Index(UploadPhotoModel pic)
        {


            if (ModelState.IsValid)
            {


                Photo photo = new Photo
                {
                    Title = pic.Title,
                    Content = pic.Content,
                    PublishedTime = DateTime.Now,
                    Image = SavePhotoAndGetName(pic.Image, User.Identity.GetUserId()),
                    SenderId = User.Identity.GetUserId(),
                    LikesCount = 0,
                    isPrivate = pic.isPrivate
                };

                connection.Execute("insert into Photos(Title, Content, PublishedTime, Image, SenderId, LikesCount, isPrivate) values(@title, @content, @publishedTime, @image, @senderId, @likesCount, @isPrivate)", photo);
                int lastid = connection.Query<int>("select Id from Photos where Title = @title AND Content = @content AND PublishedTime = @publishedTime AND Image = @image AND SenderId = @senderId", photo).SingleOrDefault();
                connection.Execute("insert into PrivatePhotoAcesses(PhotoId, UserId) values(@photoId, @userId)", new { photoId = lastid, userId = User.Identity.GetUserId() });

                //db.Photos.Add(photo);
                //db.SaveChanges();
                return Redirect("/Home/");
                //return RedirectToAction("Index");
            }

            return RedirectToAction("Create");
            //return "Failed";
        }
    }
}