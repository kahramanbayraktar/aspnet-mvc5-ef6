using System;

namespace Ced.Web.Models.User
{
    public class UserListModel
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }
        
        public string Surname { get; set; }

        public string AdLogonName { get; set; }

        public bool Active { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}