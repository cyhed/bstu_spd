using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lab2.Entities;

namespace lab2.data
{
    internal class StorageUsers
    {
        List<User> Users = new List<User>();

        public void AddUser(User user)
        {
            this.Users.Add(user);            
        }

        public User GetUserbyEmail(String email) { 
            foreach (User user in Users)
            {
                if (user.Email == email) return user;
            }
            return null;
        }
        public bool FoundUserbyEmail(String email) {
            foreach (User user in this.Users)
            {
                if (user.Email.Equals(email)) 
                    return true;
            }
            return false;
        } 
    }
}
