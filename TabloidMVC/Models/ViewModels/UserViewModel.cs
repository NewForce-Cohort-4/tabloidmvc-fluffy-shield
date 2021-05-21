using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TabloidMVC.Models.ViewModels
{
    public class UserViewModel
    {
        public Post Post { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();
        public int activeUser { get; set; }
    }
}
