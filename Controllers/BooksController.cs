using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace lab5.Controllers
{
    // worked individually
    public class BooksController : Controller
    {
        public IActionResult Index()
        {
            IList<Models.Books> booksList = new List<Models.Books>();

            string path = Request.PathBase + "App_Data/books.xml";
            XmlDocument doc = new XmlDocument();

            if (System.IO.File.Exists(path))
            {
                doc.Load(path);
                XmlNodeList books = doc.GetElementsByTagName("book");

                foreach(XmlElement b in books)
                {
                    Models.Books book = new Models.Books();

                    book.ID = b.GetElementsByTagName("id")[0].InnerText;
                    book.Title = b.GetElementsByTagName("title")[0].InnerText;
                    book.FirstName = b.GetElementsByTagName("firstname")[0].InnerText;
                    book.MiddleName = b.GetElementsByTagName("middlename").Count > 0 ? b.GetElementsByTagName("middlename")[0].InnerText : "";
                    book.LastName = b.GetElementsByTagName("lastname")[0].InnerText;

                    booksList.Add(book);
                }
            }

            return View(booksList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var book = new Models.Books();
            return View(book);
        }

        //this will load when a form is submitted via post (form data passed as model)
        [HttpPost]
        public IActionResult Create(Models.Books b)
        {
            string path = Request.PathBase + "App_Data/books.xml";
            XmlDocument doc = new XmlDocument();

            if (System.IO.File.Exists(path))
            {
                doc.Load(path);

                // get the count of books
                int booksCount = doc.GetElementsByTagName("book").Count;

                // if the cuurent count is equl or more than 5 remove first book
                if (booksCount >= 5)
                {
                    doc.DocumentElement.RemoveChild(doc.DocumentElement.FirstChild);
                }

                XmlElement book = _CreateBookElement(doc, b);

                doc.DocumentElement.AppendChild(book);

            }
            else
            {
                XmlNode dec = doc.CreateXmlDeclaration("1.0", "utf-8", "");
                doc.AppendChild(dec);
                XmlNode root = doc.CreateElement("books");

                XmlElement book = _CreateBookElement(doc, b);
                root.AppendChild(book);
                doc.AppendChild(root);
            }
            doc.Save(path);
            return RedirectToAction("Index");
        }

        private XmlElement _CreateBookElement(XmlDocument doc, Models.Books newBook)
        {
            XmlElement book = doc.CreateElement("book");

            XmlNode title = doc.CreateElement("title");
            title.InnerText = newBook.Title;
            book.AppendChild(title);

            int newIndex = 0001;
            if (doc.LastChild != null)
            {
                int lastIndex = Int32.Parse(doc.SelectSingleNode("//book[last()]//id").InnerText);
                newIndex = lastIndex + 1;
            }
            XmlNode id = doc.CreateElement("id");
            // pad with leading zeros
            id.InnerText = newIndex.ToString("D4");
            book.AppendChild(id);

            XmlNode author = doc.CreateElement("author");

            XmlNode firstname = doc.CreateElement("firstname");
            firstname.InnerText = newBook.FirstName;
            author.AppendChild(firstname);

            XmlNode middlename = doc.CreateElement("middlename");
            middlename.InnerText = newBook.MiddleName;
            author.AppendChild(middlename);

            XmlNode lastname = doc.CreateElement("lastname");
            lastname.InnerText = newBook.LastName;
            author.AppendChild(lastname);

            book.AppendChild(author);

            return book;
        }

    }
}
