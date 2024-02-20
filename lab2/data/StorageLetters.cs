using lab2.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2.data
{
    internal class StorageLetters : IStorageLetters
    {
        List<Letter> letters = new List<Letter>();
        public void AddLetter(Letter letter) { 
            letters.Add(letter);
        }
        public List<Letter> GetLetter(string recipient)
        {         
            var selectedLetters = letters.Where(p => p.GetRecipients().Contains(recipient));

            List<Letter> sel = new List<Letter>();
            foreach (Letter letter in selectedLetters)
                sel.Add(letter);

            return sel;
        }

        public void RemoveLetter(string idLetter)
        {
            letters.Remove((Letter)(from letter in letters where letter.id.Equals(idLetter) select letter));
        }
    }
}
