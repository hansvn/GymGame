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

        public User saveUser(User u)
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

        public User logIn(User user)
        {
            if (user.FB_UserId == null)
            {
                throw new Exception("Facebook ID must be given...");
            }
            var result = (from u in dc.Users
                          where u.FB_UserId.Equals(user.FB_UserId)
                          select u).Single();
            //check if username has changed:
            if (result.Firstname.Equals(user.Firstname) && result.Lastname.Equals(user.Lastname))
            {
                return result;
            }
            else
            {
                return saveUser(user);
            }
        }

        public Boolean userExists(String fbId)
        {
            if (fbId == null)
            {
                throw new Exception("Facebook ID must be given...");
            }

            //check if users exists (with .any() function)
            var value = (from u in dc.Users
                        where u.FB_UserId.Equals(fbId)
                        select u).Any();

            return value;
        }
    }
}

