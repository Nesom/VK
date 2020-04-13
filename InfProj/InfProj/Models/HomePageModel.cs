using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfProj.Models
{
    public class HomePageModel
    {
        public User User { get; set; }
        public IEnumerable<PostModel> Posts {get;set;}
        public HomePageModel(User user, IEnumerable<PostModel> posts)
        {
            User = user;
            Posts = posts;
        }
    }
}
