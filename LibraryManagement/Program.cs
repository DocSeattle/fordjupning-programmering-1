using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Alba.CsConsoleFormat;

/** 
Functions:
[X] - Add Book.
[X] - Borrow Book.
[X] - Return Book.
[X] - Browse Books.
[X] - Browse Borrowers.
*/

public class Book()
{
  [JsonProperty("Name")]
  public string? Name;

  [JsonProperty("Author")]
  public string? Author;

  [JsonProperty("Borrower")]
  public string? Borrower;

  [JsonProperty("Available")]
  public bool? Available;
}

public class Borrower()
{
  public string? BorrowerName;
}

class LibraryManagement
{
  static void Main()
  {
    //TOP-LEVEL
    string jsonPath = "LibraryManagement/Books.json";
    string jsonContent = File.ReadAllText(jsonPath);
    JObject json = JObject.Parse(jsonContent);
    JToken? AvailableBooksToken = json.SelectToken("AvailableBooks");
    JToken? UnavailableBooksToken = json.SelectToken("UnavailableBooks");
    List<Book>? Available = AvailableBooksToken!.ToObject<List<Book>>();
    List<Book>? Unavailable = UnavailableBooksToken!.ToObject<List<Book>>();
    //START
    Console.WriteLine("Welcome to LibraryManagement. Please proceed with the following options:");
    Console.WriteLine("1. Add a new book. \n2. Browse available books. \n3. Borrow a book. \n4. Return a book. \n5. Browse borrowers. \n6. Exit");

    string userInput = inputCatch();
    if (userInput == null) { throw new NullReferenceException(userInput); }
    while (RegexParse(userInput) < 1 || RegexParse(userInput) > 6) { Console.WriteLine("Err: Option doesn't exist."); RegexParse(userInput); }
    switch (RegexParse(userInput))
    {
      case 1:
        Console.WriteLine("Adding New Book.");
        AddNewBook();
        break;
      case 2:
        Console.WriteLine("Browsing Available Books.");
        BrowseAvailableBooks();
        break;
      case 3:
        Console.WriteLine("Borrowing Book.");
        BorrowBook();
        break;
      case 4:
        Console.WriteLine("Returning Book.");
        ReturnBook();
        break;
      case 5:
        Console.WriteLine("Browsing Borrowers.");
        BrowseBorrowers();
        break;
      case 6:
        Environment.Exit(0);
        break;
    }
    Environment.Exit(0);
    // FUNC
    string inputCatch()
    {
      string? value_in = Console.ReadLine();
      while (value_in == String.Empty || value_in == null)
      {
        Console.WriteLine("Input can't be null or empty, try again");
        value_in = Console.ReadLine()!;
      }
      string value_out = value_in;
      return value_out;
    }
    int RegexParse(string input)
    {
      return Int32.Parse(Regex.Replace(input, "[^0-9]", ""));
    }
    void AddNewBook()
    {
      Console.WriteLine("Book Title: ");
      string _name = inputCatch();
      Console.WriteLine("Author: ");
      string _author = inputCatch();

      bool _available = true;
      string _borrower = String.Empty;
      Console.WriteLine("Is the book available? [Y/N]");
      string? _tempInput = null;
      while (_tempInput == null || _tempInput == String.Empty)
      {
        int i = 0;
        if (i > 0)
        { Console.WriteLine("Input can't be null or empty, please try again."); }
        _tempInput = Console.ReadLine()!;
        i++;
      }
      if (_tempInput.Contains('Y')) { _available = true; }
      if (_tempInput.Contains('N'))
      {
        _available = false;
        Console.WriteLine("Who is borrowing the book?");
        _borrower = inputCatch();
      }
      Book _book = new Book()
      {
        Name = _name,
        Author = _author,
        Borrower = _borrower,
        Available = _available
      };
      if (_available)
      {
        Available!.Add(_book);
        SortAndWriteJson();
      }
      if (!_available)
      {
        Unavailable!.Add(_book);
        SortAndWriteJson();
      }
      Main();
    }

    void BrowseAvailableBooks()
    {
      var _books = json.SelectToken("AvailableBooks")?.ToObject<List<Book>>();
      _books!.ToArray<Book>();
      var headerThickness = new LineThickness(LineWidth.Double, LineWidth.Double);

      var doc = new Document(
        new Span("Author: ") { Color = ConsoleColor.Gray }, "\n",
        new Span("Title: ") { Color = ConsoleColor.Gray }, "\n",
        new Span("Borrower: ") { Color = ConsoleColor.Gray },
         new Grid
         {
           Color = ConsoleColor.Gray,
           Columns = { GridLength.Star(2), GridLength.Star(1), GridLength.Star(1) },
           Children =
           {
            new Cell("Title") {Stroke = headerThickness},
            new Cell("Author") {Stroke = headerThickness},
            new Cell("Borrower") {Stroke = headerThickness},
            _books!.Select(Book => new[] {
                new Cell(Book.Name),
                new Cell(Book.Author),
                new Cell(Book.Borrower),
            })
           }
         });
      Console.Clear();
      ConsoleRenderer.RenderDocument(doc);
      Console.WriteLine("Press any key to continue");
      Console.ReadKey();
      Main();
    }

    void BorrowBook()
    {
      Console.WriteLine("Who is borrowing the book?");
      string _user = inputCatch();
      Console.WriteLine("Please type the name of the book you wish to borrow.");
      string _bookSearch = inputCatch();
      Book? _bookTarget = Available?.SingleOrDefault(book => book.Name == _bookSearch);
      _bookTarget!.Borrower = _user;
      Unavailable?.Add(_bookTarget);
      Available?.Remove(_bookTarget);
      SortAndWriteJson();      
    }
    void ReturnBook() 
    {
      string _user = String.Empty;
      Console.WriteLine("Please type the name of the book you wish to return");
      string _bookSearch = inputCatch();
      Book? _bookTarget = Unavailable?.SingleOrDefault(book => book.Name == _bookSearch);
      _bookTarget!.Borrower = _user;
      Available?.Add(_bookTarget);
      Unavailable?.Remove(_bookTarget);
      Console.WriteLine("Thank you for your patronage.");
      SortAndWriteJson();
    }
    void BrowseBorrowers() 
    {
      var headerThickness = new LineThickness(LineWidth.Double, LineWidth.Double);

      var _books = Unavailable!.OrderBy(o => o.Borrower);

      var doc = new Document(
        new Span("Borrower: ") { Color = ConsoleColor.Gray },"\n",
        new Span("Title: ") { Color = ConsoleColor.Gray }, "\n",
        new Span("Author: ") { Color = ConsoleColor.Gray },
         new Grid
         {
           Color = ConsoleColor.Gray,
           Columns = { GridLength.Star(2), GridLength.Star(1), GridLength.Star(1) },
           Children =
           {
            new Cell("Title") {Stroke = headerThickness},
            new Cell("Author") {Stroke = headerThickness},
            new Cell("Borrower") {Stroke = headerThickness},
            _books!.Select(Book => new[] {
                new Cell(Book.Borrower),
                new Cell(Book.Name),
                new Cell(Book.Author),
            })
           }
         });
      Console.Clear();
      ConsoleRenderer.RenderDocument(doc);
      Console.WriteLine("Press any key to continue");
      Console.ReadKey();
      Main();
    }

    void SortAndWriteJson()
    {
      var _available = Available!.OrderBy(book => book.Author);
      var _unavailable = Unavailable!.OrderBy(book => book.Author);
      json["AvailableBooks"] = JArray.FromObject(_available!);
      json["UnavailableBooks"] = JArray.FromObject(_unavailable!);
      File.WriteAllText(jsonPath, json.ToString());
      Main();
    }
    ///END
    ///END
    ///END
  }
}
