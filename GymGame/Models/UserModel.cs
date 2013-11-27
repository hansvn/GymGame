using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GymGame.Models
{
    public class UserModel
    {
        public int ID { get; set; }
        public string name { get; set; }

        private GymGameModelDataContext dc = new GymGameModelDataContext();

        public User SaveUser(User u)
        {
            if (u == null)
            {
                throw new Exception("User must be given...");
            }
            dc.Users.InsertOnSubmit(u);
            dc.SubmitChanges();
            
            return u;
        }

        public List<User> getAllUsers()
        {
            var result = (from u in dc.Users
                          select u).ToList<User>();
            return result;
        }

        public User logIn(int fbId)
        {
            if (fbId == null)
            {
                throw new Exception("Facebook ID must be given...");
            }
            var result = (from u in dc.Users
                          where u.FB_UserId == fbId
                          select u).Single();
            return result;
        }
    }
}

