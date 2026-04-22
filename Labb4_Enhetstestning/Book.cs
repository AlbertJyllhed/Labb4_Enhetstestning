namespace Labb4_Enhetstestning
{
    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int PublicationYear { get; set; }
        public bool IsBorrowed { get; set; }
        public DateTime? BorrowDate { get; set; }

        public Book(string title, string author, string isbn, int publicationYear)
        {
            Title = title;
            Author = author;
            ISBN = isbn;
            PublicationYear = publicationYear;
            IsBorrowed = false;
            BorrowDate = null;
        }
    }
}
