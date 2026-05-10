namespace Labb4_Enhetstestning.Tests;

[TestClass]
public class LibrarySystemTests
{
    private LibrarySystem _libSys = new LibrarySystem();

    [TestInitialize]
    public void Setup()
    {
        _libSys = new LibrarySystem();
    }

    // add tests ----------------------------------------------------------------------------------
    [TestMethod]
    [DataRow("Dune", "Frank Herbert", "9780441013593", 1965)] // ISBN-13 with 978 prefix
    [DataRow("How to Make a Few Billion Dollars", "Brad Jacobs", "9798886451740", 2024)] // ISBN-13 with 979 prefix
    [DataRow("The Hobbit", "J.R.R. Tolkien", "0547928227", 1937)] // ISBN-10
    [DataRow("The Divine Comedy", "Dante Alighieri", "014044893X", 1320)] // ISBN-10 with X
    public void AddBook_ShouldReturnTrue_BookShouldExist(
        string title, string author, string isbn, int publicationYear)
    {
        var book = new Book(title, author, isbn, publicationYear);
        bool result = _libSys.AddBook(book);
        Assert.IsTrue(result);
        Assert.IsNotNull(_libSys.SearchByISBN(isbn)); // check if the book ended up in the list
    }

    [TestMethod]
    public void AddBook_ShouldReturnFalse_WhenAddingDuplicate()
    {
        var book = new Book("1984", "George Orwell", "9780451524935", 1949); // exists in default list
        bool result = _libSys.AddBook(book);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void AddBook_ShouldReturnFalse_WhenAddingNull()
    {
        bool result = _libSys.AddBook(null);
        Assert.IsFalse(result);
    }

    // remove tests -------------------------------------------------------------------------------
    [TestMethod]
    public void RemoveBook_ShouldReturnTrue_BookShouldBeGone()
    {
        string isbn = "9780451524935"; // exists in the default list
        bool result = _libSys.RemoveBook(isbn);
        Assert.IsTrue(result);
        Assert.IsNull(_libSys.SearchByISBN(isbn)); // check if the book is gone
    }

    [TestMethod]
    [DataRow("0000000000")] // ISBN that doesn't exist
    [DataRow("")]
    [DataRow(null)]
    public void RemoveBook_ShouldReturnFalse_WhenBookNotFound(string isbn)
    {
        bool result = _libSys.RemoveBook(isbn);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void RemoveBook_ShouldReturnFalse_WhenRemovingBorrowed()
    {
        string isbn = "9780451524935"; // exists in the default list
        _libSys.BorrowBook(isbn);
        bool result = _libSys.RemoveBook(isbn);
        Assert.IsFalse(result);
    }

    // search tests -------------------------------------------------------------------------------
    [TestMethod]
    public void SearchByISBN_BookShouldExist()
    {
        string isbn = "9780451524935"; // exists in the default list
        var result = _libSys.SearchByISBN(isbn);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    [DataRow("The Great Gatsby")]
    [DataRow("the great gatsby")]
    [DataRow("THE GREAT GATSBY")]
    [DataRow("Great")]
    public void SearchByTitle_BookListShouldNotBeEmpty(string title)
    {
        var result = _libSys.SearchByTitle(title);
        Assert.IsNotEmpty(result);
    }

    [TestMethod]
    [DataRow("Jane Austen")]
    [DataRow("jane austen")]
    [DataRow("JANE AUSTEN")]
    [DataRow("Jane")]
    public void SearchByAuthor_BookListShouldNotBeEmpty(string author)
    {
        var result = _libSys.SearchByAuthor(author);
        Assert.IsNotEmpty(result);
    }

    // borrow tests -------------------------------------------------------------------------------
    [TestMethod]
    public void BorrowBook_ShouldMarkBookAsBorrowed()
    {
        string isbn = "9780451524935"; // exists in the default list
        _libSys.BorrowBook(isbn);
        var result = _libSys.SearchByISBN(isbn);
        Assert.IsTrue(result.IsBorrowed);
    }

    [TestMethod]
    public void BorrowBook_ShouldReturnFalse_WhenAlreadyBorrowed()
    {
        string isbn = "9780451524935"; // exists in the default list
        _libSys.BorrowBook(isbn);
        bool result = _libSys.BorrowBook(isbn);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void BorrowBook_BorrowDateShouldBeCorrect()
    {
        string isbn = "9780451524935"; // exists in the default list
        var before = DateTime.Now;

        _libSys.BorrowBook(isbn);

        var after = DateTime.Now;
        var result = _libSys.SearchByISBN(isbn);

        Assert.IsTrue(result.BorrowDate.HasValue); // check that there is a date
        Assert.IsTrue(result.BorrowDate >= before && result.BorrowDate <= after);
    }

    // return tests -------------------------------------------------------------------------------
    [TestMethod]
    public void ReturnBook_BorrowDateShouldBeReset()
    {
        string isbn = "9780451524935"; // exists in the default list

        bool borrowed = _libSys.BorrowBook(isbn);
        Assert.IsTrue(borrowed, "Setup failed: book could not be borrowed");

        bool returned = _libSys.ReturnBook(isbn);
        Assert.IsTrue(returned, "Setup failed: book could not be returned");

        var result = _libSys.SearchByISBN(isbn);
        Assert.IsFalse(result.IsBorrowed);
        Assert.IsNull(result.BorrowDate);
    }

    [TestMethod]
    public void ReturnBook_ShouldReturnFalse_WhenBookIsNotBorrowed()
    {
        string isbn = "9780451524935"; // exists in the default list
        bool result = _libSys.ReturnBook(isbn);
        Assert.IsFalse(result);
    }

    // overdue tests ------------------------------------------------------------------------------
    [TestMethod]
    public void IsBookOverdue_ShouldReturnTrue_WhenBookIsLate()
    {
        string isbn = "9780451524935"; // exists in the default list

        _libSys.BorrowBook(isbn);

        var book = _libSys.SearchByISBN(isbn);
        book.BorrowDate = DateTime.Now.AddDays(-31);

        bool result = _libSys.IsBookOverdue(isbn, 30);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsBookOverdue_ShouldReturnFalse_WhenBookIsNotLate()
    {
        string isbn = "9780451524935"; // exists in the default list

        _libSys.BorrowBook(isbn);

        var book = _libSys.SearchByISBN(isbn);
        book.BorrowDate = DateTime.Now.AddDays(-29);

        bool result = _libSys.IsBookOverdue(isbn, 30);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsBookOverdue_ShouldReturnFalse_WhenBookIsNotBorrowed()
    {
        string isbn = "9780451524935"; // exists in the default list
        bool result = _libSys.IsBookOverdue(isbn, 30);
        Assert.IsFalse(result);
    }

    // late fee tests -----------------------------------------------------------------------------
    [TestMethod]
    [DataRow(10, 5.0)] // 10 * 0.5 = 5.0
    [DataRow(1, 0.5)] // 1  * 0.5 = 0.5
    [DataRow(0, 0.0)] // daysLate <= 0 should return 0
    [DataRow(-1, 0.0)] // negative days should return 0
    public void CalculateLateFee_ShouldReturnCorrectFee(int daysLate, double expectedFee)
    {
        string isbn = "9780451524935";
        _libSys.BorrowBook(isbn);

        var result = _libSys.CalculateLateFee(isbn, daysLate);

        Assert.AreEqual((decimal)expectedFee, result);
    }
}
