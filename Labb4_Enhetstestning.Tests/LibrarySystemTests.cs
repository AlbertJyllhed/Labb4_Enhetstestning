namespace Labb4_Enhetstestning.Tests;

[TestClass]
public class LibrarySystemTests
{
    private LibrarySystem _libSys;

    public LibrarySystemTests()
    {
        _libSys = new LibrarySystem();
    }

    [TestMethod]
    [DataRow("", "", "", 0000)]
    public void AddBook_ShouldReturnTrue_WhenAddingNewBook(
        string title, string author, string isbn, int publicationYear)
    {
        //Arrange
        var book = new Book(title, author, isbn, publicationYear);

        //Act
        var result = _libSys.AddBook(book);

        //Assert
        Assert.IsTrue(result);
    }
}
