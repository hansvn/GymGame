using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GymGame.Models;

namespace GymGame.Classes
{
    public class Leader
    {
        public Quiz quiz { get; set; }
        public User user { get; set; }
        public int goodAnswers { get; set; }
        public int questions { get; set; }
        public int score { get; set; }
        

    }
}