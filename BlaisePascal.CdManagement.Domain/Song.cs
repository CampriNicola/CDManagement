namespace BlaisePascal.CdManagement.Domain
{
    public class Song
    {
        private string Title { get; set; }
        public string Author { get; set; }

        /// <summary>
        /// In seconds
        /// </summary>
        private int Duration { get; set; }

        public Song(string title, string author, int duration)
        {
            SetTitle(title);
            SetAuthor(author);
            SetDuration(duration);
        }

        //Getters
        public string GetTitle() => Title;
        public string GetAuthor() => Author;
        public int GetDuration() => Duration;

        //Setters
        public void SetTitle(string title) => Title = title;
        public void SetAuthor(string author) => Author = author;
        public void SetDuration(int duration) => Duration = duration;

        public override string ToString()
        {
            return $"Title: {Title} \n Author: {Author}, Duration: {Duration}(sec)";
        }

        public bool ShortSong(int treshold) => Duration < treshold;
    }
}
