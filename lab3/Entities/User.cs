using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2.Entities
{
    internal class User
    {
        public string Id { get; private set; } 
        public String Login { get; private set; } = String.Empty;
        public String Password { get; private set; } = String.Empty;       

        public User() { this.Id = this.Id = Guid.NewGuid().ToString(""); }

        public User(User user) { 
            this.Id = user.Id;
            this.Login = user.Login;             
            this.Password = user.Password;            
        }

        public User(String login, String password) { 
            this.Id = Guid.NewGuid().ToString(""); 
            this.Login = login;
            this.Password = password;
        }

        public void SetLogin(String login) { this.Login = login; } 
        public void SetPassword(String password) {  this.Password = password; }
        
    }
}
