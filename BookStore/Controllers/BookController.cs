using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7006/api");
        HttpClient client;

        List<Book> bookdata = new List<Book>();
        List<Book> books = new List<Book>();
        public BookController()
        {
            client = new HttpClient();
            client.BaseAddress = baseAddress;
        }

        public IActionResult Index()
        {
            List<Book> books = new List<Book>();
            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/book").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                books = JsonConvert.DeserializeObject<List<Book>>(data);
            }
            return View(books);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateOrUpdate(Book book)
        {
            string data = JsonConvert.SerializeObject(book);
            StringContent stringContent = new StringContent(data, Encoding.UTF8, "application/json");

            if (book.Id == 0)
            {
                HttpResponseMessage response = client.PostAsync(client.BaseAddress + "/book", stringContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                return View("Create", book);
            }
            else
            {
                HttpResponseMessage response = client.PutAsync(client.BaseAddress + $"/book?Id={book.Id}", stringContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                return View("Create", book);
            }
        }


        public ActionResult Delete(int Id)
        {
            HttpResponseMessage response = client.DeleteAsync(client.BaseAddress + $"/book?Id={Id}").Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return BadRequest("Something went Wrong");
        }

        public ActionResult Edit(Book book)
        {
            return View("Create", book);
        }

        public ActionResult Details(int id)
        {
            client.BaseAddress = baseAddress;
            HttpResponseMessage response = client.GetAsync(baseAddress + $"/books/id?Id={id}").Result;
            string data = response.Content.ReadAsStringAsync().Result;
            var book = JsonConvert.DeserializeObject<Book>(data);
            return View(book);

        }
        public IActionResult Privacy()
        {
            return View();
        }


        public ActionResult Search(string searchString)
        {
            List<Book> books = new List<Book>();
            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/book/" + searchString).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                books = JsonConvert.DeserializeObject<List<Book>>(data);
            }
            return View("Index", books);
        }

        public ActionResult AddCart(int id)
        {
            client.BaseAddress = baseAddress;
            HttpResponseMessage response = client.GetAsync(baseAddress + "book").Result;
            string data = response.Content.ReadAsStringAsync().Result;
            bookdata = JsonConvert.DeserializeObject<List<Book>>(data);
            var book = bookdata.Where(e => e.Id == id).FirstOrDefault();
            if (book == null)
            {
                return NoContent();
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7103/api/");

                //HTTP POST

                var postTask = client.PostAsJsonAsync<Book>("Cart", book);

                postTask.Wait();

                var result = postTask.Result;

                if (result.IsSuccessStatusCode)

                {

                    return RedirectToAction("Home");
                }

            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            return View();
        }

        public ActionResult Cart()
        {
            client.BaseAddress = baseAddress;
            HttpResponseMessage response = client.GetAsync(baseAddress + "Cart").Result;
            string data = response.Content.ReadAsStringAsync().Result;
            bookdata = JsonConvert.DeserializeObject<List<Book>>(data);
            return View(bookdata);
        }

        public ActionResult DeleteinCart(int id)
        {
            using (var client = new HttpClient())

            {
                client.BaseAddress = baseAddress;
                var delete = client.DeleteAsync($"{id}");
                delete.Wait();
                var results = delete.Result;
                if (results.IsSuccessStatusCode)
                {
                    return RedirectToAction("Cart");
                }
            }
            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View();
        }
    }
}