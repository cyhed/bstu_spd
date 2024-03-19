using lab2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3.data
{
    internal interface IStorageUsers
    {
        public void AddUser(User user);
        public User GetUserbyLogin(String login);
        public bool FoundUserbyLogin(String login);
    }
}
