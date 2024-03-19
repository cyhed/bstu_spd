using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lab2.Entities;
using lab3.data;

namespace lab2.data
{
    internal class StorageUsers : IStorageUsers
    {
        List<User> Users = new List<User>();

        public void AddUser(User user)
        {
            this.Users.Add(user);            
        }

        public User GetUserbyLogin(String login) { 
            foreach (User user in Users)
            {
                if (user.Login == login) return user;
            }
            return null;
        }
        public bool FoundUserbyLogin(String login) {
            foreach (User user in this.Users)
            {
                if (user.Login.Equals(login)) 
                    return true;
            }
            return false;
        } 
    }
}
