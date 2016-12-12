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
    public class LogsController : Controller
    {
        private SqlConnection connection;
        public LogsController()
        {
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }
        // GET: Logs
        public ActionResult Index()
        {
            string MYuserId = User.Identity.GetUserId();
            User currentUser = connection.Query<User>("select * from Users where UserId = @userId", new { userId = MYuserId }).SingleOrDefault();

            if (currentUser.isAdmin == true)
            {
                IEnumerable<LogModel> postlogs = connection.Query<LogModel>("select * from PostEvents ORDER BY DateTime DESC");
                IEnumerable<LogModel> commentlogs = connection.Query<LogModel>("select * from CommentEvents ORDER BY DateTime DESC");
                ViewBag.postlogs = postlogs;
                ViewBag.commentlogs = commentlogs;
                return View();
            }
            else
                return HttpNotFound();
            
        }
    }
}