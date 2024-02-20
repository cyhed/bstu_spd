using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2.Entities
{
    internal class Letter
    {
        static private int _count = 0;
        public String id { get; private set; }
        String? author = null;
        List<String?> recipients = new List<string?>();
        String? text = null;

        public Letter() {
            this.id = _count.ToString();
            _count++;
        }
      
        public Letter(String author, List<String?> recipients, String? text = null)
        {
            this.id = _count.ToString();
            _count++;
            this.author = author;

            this.recipients.Clear();            
            foreach (String?  recipient in recipients)
            {
                this.recipients.Add(recipient);
            }
           
            this.text = text;
        }
        public Letter(Letter letter)
        {
            this.id = _count.ToString();
            _count++;

            this.author = letter.author;

            this.recipients.Clear();
            foreach (String? recipient in letter.recipients)
            {
                this.recipients.Add(recipient);
            }
            
            this.text = letter.text;
        }
        public void AddRecipient(String recipient)
        {
            recipients.Add(recipient);
        }

        public void SetAuthor(String author) { 
            this.author = author;
        }

        public void SetText(String text) {
            this.text = text;
        }
        public String GetText() {
            return this.text;
        }
        

        public List<String?> GetRecipients() {
            return recipients;
        }

        public void AddLineInText(String line) {            
            text = text + line + "\n";
        }

        

    }
}
