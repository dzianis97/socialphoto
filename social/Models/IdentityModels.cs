using System;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;

namespace social.Models
{
    public class User : IUser
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string SurName { get; set; }
        public DateTime BirthDay { get; set; }
        public string Photo { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public bool isAdmin { get; set; }
        public virtual ICollection<Photo> SendedPhotos { get; set; }


        public User()
        {
            SendedPhotos = new List<Photo>();
            //ReceivedMessages = new List<Message>();
            //SecondFriends = new List<ApplicationUser>();
        }
        string IUser.Id
        {
            get { return UserId; }
        }
    }
}