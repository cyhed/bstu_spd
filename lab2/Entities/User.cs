using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2.Entities
{
    internal class User
    {
        public int id { get; private set; } 
        public String Email { get; private set; } = String.Empty;
        public String Password { get; private set; } = String.Empty;       

        public User(int id) { this.id = id; }

        public User(User user) { 
            this.id = user.id;
            this.Email = user.Email;             
            this.Password = user.Password;            
        }

        public User(int id, String email, String password) { 
            this.id = id; 
            this.Email = email;
            this.Password = password;
        }

        public void SetEmail(String email) { this.Email = email; } 
        public void SetPassword(String password) {  this.Password = password; }
        
    }
}
