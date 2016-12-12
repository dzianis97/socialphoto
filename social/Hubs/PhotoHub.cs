using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using social.Models;

namespace social.Hubs
{
    public class PhotoHub : Hub
    {
        public void Send(dynamic message)
        {
            /*ApplicationContext db = new ApplicationContext();
            //message.PostId = db.Photos.Last().Id + 1;
            message.PostId = 1;
            string SenderId = message.SenderId.ToString();
            var sender = db.Users.First(u => u.Id == SenderId);
            var photo = message.Image;
            
            message.SenderPhoto = sender.Photo;
            message.SenderName = sender.SurName;
            // message.date = DateTime.Now;
            message.LikesCount = "0";
            //message.PostId = db.Photos.Last().Id + 1;
            Clients.Others.addMessage(message);
            db.Dispose();*/
        }
    }
}