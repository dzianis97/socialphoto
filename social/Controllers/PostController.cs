using Dapper;
using Microsoft.AspNet.Identity;
using social.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace social.Controllers
{
    public class PostController : Controller
    {
        private SqlConnection connection;
        public PostController()
        {
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }
        // GET: ViewPost
        [Authorize]
        public ActionResult Index(int id)
        {
            try
            {

                string MYuserId = User.Identity.GetUserId();
                User currentUser = connection.Query<User>("select * from Users where UserId = @userId", new { userId = MYuserId }).SingleOrDefault();
                ViewBag.currentUser = currentUser;

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
            if (id < 1)
                Response.Redirect("Home");
            Photo ph = connection.Query<Photo>("select * from Photos where Id = @photoId", new { photoId = id }).SingleOrDefault();
            if (ph.isPrivate)
            {
                int acess = connection.Query<Like>("select * from PrivatePhotoAcesses where PhotoId = @photoId AND UserId = @userId", new { photoId = ph.Id, userId = User.Identity.GetUserId() }).Count();
                if (acess < 1)
                    return HttpNotFound();
            }
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
            ViewBag.CurPost = ps;
            IEnumerable<Comment> comments = connection.Query<Comment>("select * from Comments WHERE PhotoId=@photoId ORDER BY PublishedTime DESC", new { photoId = id });
            foreach (Comment com in comments)
            {
                com.UserCommented= connection.Query<User>("select * from Users where UserId = @userId", new { userId = com.UserId }).SingleOrDefault();
            }
            return View(comments);
        }
        [Authorize]
        [HttpPost]
        public string Index()//add comment
        {

            int photoid = Int32.Parse(Request.Form["PhotoId"]);
            string content = Request.Form["Content"];
            string MYuserId = User.Identity.GetUserId();
            User currentUser = connection.Query<User>("select * from Users where UserId = @userId", new { userId = MYuserId }).SingleOrDefault();
            connection.Execute("insert into Comments(Content, PhotoId, UserId, PublishedTime) values(@content, @photoId, @userId, @publishedTime)", new { content = content, photoId = photoid, userId = MYuserId, publishedTime = DateTime.Now });
            Response.Redirect("/Post/Index/" + photoid);
            return "ok";
        }
        [Authorize]
        public ActionResult Edit(int? id)
        {
          
                string MYuserId = User.Identity.GetUserId();
                User currentUser = connection.Query<User>("select * from Users where UserId = @userId", new { userId = MYuserId }).SingleOrDefault();
            ViewBag.currentUser = currentUser;
            ViewBag.UserName = currentUser.Email;
                if (currentUser.Photo == null)
                    ViewBag.AvatarURL = "http://www.w3schools.com/w3images/avatar3.png";
                else
                    ViewBag.AvatarURL = Url.Content("~/Images/") + "/" + currentUser.Photo;
           
            if (id == null)
            {
                return HttpNotFound();
            }
            Photo ph = connection.Query<Photo>("select * from Photos where Id = @photoId", new { photoId = id }).SingleOrDefault();
            if ((ph.SenderId == User.Identity.GetUserId())||(currentUser.isAdmin==true)) {
                IEnumerable<User> users = connection.Query<User>("select * from Users WHERE UserId IN (SELECT UserId FROM PrivatePhotoAcesses WHERE PhotoId = @photoId)", new {photoId = ph.Id });
                ViewBag.users = users;
                IEnumerable<User> allusers = connection.Query<User>("select * from Users WHERE UserId NOT IN (SELECT UserId FROM PrivatePhotoAcesses WHERE PhotoId = @photoId)", new { photoId = ph.Id });
                ViewBag.allusers = allusers;
                return View(ph);
            }
            else
            {
                return HttpNotFound();
            }         
            
        }
        [Authorize]
        [HttpPost]
        public ActionResult Edit(Photo photo)
        {
            Photo oldph = connection.Query<Photo>("select * from Photos where Id = @photoId", new { photoId = photo.Id }).SingleOrDefault();
            connection.Execute("UPDATE Photos SET Title=@title, Content = @content, isPrivate = @isPrivate WHERE id=@Id", photo);
            string toadds = Request.Form["AddUsersId"];
            string[] adds = toadds.Split(';'); 
            for (int i=0; i<adds.Length-1; i++)
            {
                connection.Execute("insert into PrivatePhotoAcesses(PhotoId, UserId) values(@photoId, @userId)", new {photoId=photo.Id, userId=adds[i] });
            }
            string todels = Request.Form["DeleteUsersId"];
            string[] dels = todels.Split(';');
            for (int i = 0; i < dels.Length - 1; i++)
            {
                if (dels[i] == User.Identity.GetUserId())
                    continue;
                connection.Execute("delete from PrivatePhotoAcesses where PhotoId = @photoId AND UserId = @userId", new { photoId = photo.Id, userId = dels[i] });
            }
            connection.Execute("insert into PostEvents(PhotoId, UserId, ActionName, DateTime, Content) values(@photoId, @userId, 'edit', @date, @content)", new { photoId = photo.Id, userId = User.Identity.GetUserId(), date = DateTime.Now, content = "old:"+ new JavaScriptSerializer().Serialize(oldph) });

            return RedirectToAction("Index", new { id=photo.Id});
        }

        private bool DeletePhotoFile(string Name)
        {
            if (Name != null)
            {
                
                string physicalPath = Server.MapPath("~/PostedImages/" + Name);

                FileInfo myfileinf = new FileInfo(physicalPath);
                myfileinf.Delete();

                return true;
            }
            else
                return false;
        }
        [Authorize]
        public ActionResult Delete(int? id)
        {
            
            User currentUser = connection.Query<User>("select * from Users where UserId = @userId", new { userId = User.Identity.GetUserId() }).SingleOrDefault();
            ViewBag.currentUser = currentUser;
            if (id == null)
            {
                return HttpNotFound();
            }
            Photo ph = connection.Query<Photo>("select * from Photos where Id = @photoId", new { photoId = id }).SingleOrDefault();
            if ((ph.SenderId == User.Identity.GetUserId()) || (currentUser.isAdmin == true))
                {
                connection.Execute("delete from Photos where Id = @photoId", new { photoId=ph.Id });
                connection.Execute("delete from Likes where PhotoId = @photoId", new { photoId = ph.Id });
                connection.Execute("delete from Comments where PhotoId = @photoId", new { photoId = ph.Id });
                connection.Execute("delete from PrivatePhotoAcesses where PhotoId = @photoId", new { photoId = ph.Id });
                connection.Execute("insert into PostEvents(PhotoId, UserId, ActionName, DateTime, Content) values(@photoId, @userId, 'delete', @date, @content)", new { photoId = ph.Id, userId = currentUser.UserId, date = DateTime.Now, content = new JavaScriptSerializer().Serialize(ph) });
                DeletePhotoFile(ph.Image);
                return Redirect("/");
            }
            return Redirect("/");
        }
        [Authorize]
        public ActionResult DeleteComment(int? id)
        {
            User currentUser = connection.Query<User>("select * from Users where UserId = @userId", new { userId = User.Identity.GetUserId() }).SingleOrDefault();
            ViewBag.currentUser = currentUser;
            if (id == null)
            {
                return HttpNotFound();
            }
            Comment com = connection.Query<Comment>("select * from Comments where Id = @photoId", new { photoId = id }).SingleOrDefault();
            if ((com.UserId == User.Identity.GetUserId()) || (currentUser.isAdmin == true))
            {
                connection.Execute("delete from Comments where Id = @photoId", new { photoId = com.Id });
                connection.Execute("insert into CommentEvents(CommentId, UserId, ActionName, DateTime, Content) values(@photoId, @userId, 'delete', @date, @content)", new { photoId = com.Id, userId = User.Identity.GetUserId(), date = DateTime.Now, content = new JavaScriptSerializer().Serialize(com) });
                return Redirect("/");
            }
            return Redirect("/");
        }

    }
}