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

        public Boolean SaveUser(User u)
        {
            dc.Users.InsertOnSubmit(u);
            dc.SubmitChanges();
            
            return u.Id;
        }

        public List<User> getAllUsers()
        {
            var result = (from u in dc.Users
                          select u).ToList<User>();
            return result;
        }
    }
}

