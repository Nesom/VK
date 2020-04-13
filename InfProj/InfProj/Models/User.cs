using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfProj.Models
{
    public class User
    {
        public byte[] Image { get; set; }
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Login { get; set; }
        public string FIO { get; set; }
        public string Number { get; set; }

        public int? RoleId { get; set; }
        public Role Role { get; set; }
        public List<User> Requests { get; set; } 
        public List<User> Friends { get; set; }
        public User()
        {
            Requests = new List<User>();
            Friends = new List<User>();
        }
    }
}
