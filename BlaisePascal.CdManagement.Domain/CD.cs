using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BlaisePascal.CdManagement.Domain
{
    public class CD
    {
        // Properties
        public List<Song> Songs { get; private set; }
        private string Author { get; set; }
        private string Title { get; set; }

        // Constructor
        public CD(string author, string title, List<Song> songs) 
        {
            Author = author;
            Title = title;
            Songs = songs;
        }

        // Getter
        public string GetTitle() => Title;
        public string GetAuthor() => Author;

        // Setter
        public void SetAuthor(string author) { Author = author; }
        public void SetTitle(string title) { Title = title; }


        // Methods
        public override string ToString()
        {
            string msg = "";
            foreach(Song s in Songs)
            {
                msg += s.GetTitle;
                msg += "\n";
            }

            return msg;
        }

        public int GetDuration()
        {
            int totDuration = 0;
            foreach(Song s in Songs)
            {
                totDuration += s.GetDuration();
            }
            return totDuration;
        }
    }
}
