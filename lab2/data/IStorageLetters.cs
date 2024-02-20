using lab2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2.data
{
    internal interface IStorageLetters
    {
        public void AddLetter(Letter letter);
        public List<Letter> GetLetter(string recipient);
        public void RemoveLetter(string idLetter);
    }
}
