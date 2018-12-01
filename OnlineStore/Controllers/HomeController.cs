using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OnlineStore.Models;
using System.Data.Entity;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace OnlineStore.Controllers
{
    public class HomeController : Controller
    {
        private DBContext db = new DBContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetCategories()
        {
            var categories = db.Categories;

            return PartialView("_Categories", categories);
        }

        public ActionResult GetProducts()
        {
            var products = db.Products.Include(t => t.Category);          

            return PartialView("_Products", products);
        }

        public ActionResult Search(string searchText)
        {
            var products = db.Products.Include(p => p.Category).Where(t => t.Name.Contains(searchText));

            return View(products);
        }

        [HttpPost]
        public JsonResult AddComment(Comment comment)
        {
            bool isAdd = false;
            string message = "Упс, щось пішло не так. Спробуйте пізніше...";
            bool captcha = ReCaptcha.ValidateCaptcha(Request["g-recaptcha-response"]);

            if (comment.ProductId == null || String.IsNullOrWhiteSpace(comment.Author) || String.IsNullOrWhiteSpace(comment.Text) || !captcha)
            {
                message = "Заповніть усі поля!";
            }            
            else
            {
                var ukraineTime = TimeZoneInfo.ConvertTime(DateTime.Now,
                 TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"));

                comment.Time = ukraineTime.ToString("HH:mm MM/dd/yyyy");

                db.Comments.Add(comment);
                db.SaveChanges();

                isAdd = true;
            }

            return Json(new { IsAdd = isAdd, Message = message, JsonRequestBehavior.AllowGet });
        }

        [HttpPost]
        public JsonResult SendFeedback(string userName, string userEmail, string userMessage)
        {
            bool isSend = false;
            string message = "Упс, щось пішло не так. Спробуйте пізніше...";
            bool captcha = ReCaptcha.ValidateCaptcha(Request["g-recaptcha-response"]);

            if (String.IsNullOrWhiteSpace(userName) || String.IsNullOrWhiteSpace(userEmail) || String.IsNullOrWhiteSpace(userMessage) || !captcha)
            {
                message = "Заповніть усі поля!";
            }
            else
            {
                try
                {
                    EmailService.SendEmail("denisukrvin@gmail.com", "Нове повідомлення", $"Надійшло нове повідомлення через форму зворотнього зв'язку.\n\nІм'я: {userName}\nE-mail: {userEmail}\nПовідомлення: {userMessage}");

                    isSend = true;
                    message = "Ваше повідомлення успішно відправлено!";
                }
                catch (Exception ex)
                {
                    message = $"Помилка: {ex.Message}";
                }             
            }

            return Json(new { IsSend = isSend, Message = message, JsonRequestBehavior.AllowGet });
        }

        [HttpPost]
        public JsonResult SubscribeToNewsletter(string userEmail)
        {
            bool isSubscribe = false;
            string message = "Упс, щось пішло не так. Спробуйте пізніше...";

            if (String.IsNullOrWhiteSpace(userEmail))
            {
                message = "Введіть ваш e-mail";
            }
            else
            {
                if (IsValidEmail(userEmail))
                {
                    UserEmail user = null;
                    user = db.UserEmails.FirstOrDefault(u => u.Email == userEmail);

                    if (user == null)
                    {
                        UserEmail user1 = new UserEmail();
                        user1.Email = userEmail;

                        db.UserEmails.Add(user1);
                        db.SaveChanges();

                        isSubscribe = true;
                        message = "Ви успішно підписались на новини";
                    }
                    else
                    {
                        message = "Даний e-mail уже підписаний на новини";
                    }
                }
                else
                {
                    message = "Введіть корректний e-mail";
                }                          
            }

            return Json(new { IsSubscribe = isSubscribe, Message = message, JsonRequestBehavior.AllowGet });
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        public JsonResult GetCities()
        {
            bool isGet = false;
            List<string> cities = new List<string>();

            try
            {
                string url = "https://api.novaposhta.ua/v2.0/json/Address/getCities";

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{ \"modelName\" : \"Address\", \"calledMethod\" : \"getCities\", \"apiKey\" : \"488581286ab30d43d49bef122644094a\" }";

                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    dynamic json = JsonConvert.DeserializeObject(responseText);

                    if (json.success == true)
                    {
                        foreach (var dataItem in json.data)
                        {
                            string dataItemDescription = dataItem.Description;
                            cities.Add(dataItemDescription);
                        }

                        isGet = true;
                    }
                }               
            }
            catch (WebException)
            {
                isGet = false;              
            }

            return Json(new { IsGet = isGet, Cities = cities, JsonRequestBehavior.AllowGet });
        }

        [HttpPost]
        public JsonResult GetWarehouses(string city)
        {
            bool isGet = false;
            List<string> warehouses = new List<string>();

            try
            {
                string url = "https://api.novaposhta.ua/v2.0/json/AddressGeneral/getWarehouses";

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = $"{{ \"modelName\" : \"AddressGeneral\", \"calledMethod\" : \"getWarehouses\", \"methodProperties\" : {{ \"CityName\" : \"{city}\" }}, \"apiKey\" : \"488581286ab30d43d49bef122644094a\" }}";

                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    dynamic json = JsonConvert.DeserializeObject(responseText);

                    if (json.success == true)
                    {
                        foreach (var dataItem in json.data)
                        {
                            string dataItemDescription = dataItem.Description;
                            warehouses.Add(dataItemDescription);
                        }

                        isGet = true;
                    }
                }            
            }
            catch (WebException)
            {
                isGet = false;
            }

            return Json(new { IsGet = isGet, Warehouses = warehouses, JsonRequestBehavior.AllowGet });
        }

        public ActionResult Browse(string category)
        {
            if (category == null)
            {
                return HttpNotFound();               
            }

            var products = db.Products.Include(p => p.Category).Where(t => t.Category.Name == category);           

            if (products == null)
            {
                return HttpNotFound();
            }          

            Category currentCategory = db.Categories.FirstOrDefault(t => t.Name == category);

            if (currentCategory != null)
            {
                ViewBag.CategoryName = currentCategory.Description;
            }         

            return View(products);
        }       

        public ActionResult Details(string category, int? code)
        {
            if (category == null || code == null)
            {
                return HttpNotFound();
            }

            Product product = db.Products.Include(p => p.Category).Include(c => c.Comments).FirstOrDefault(t => t.ProductCode == code);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }
      
        [HttpPost]
        public JsonResult ProductInfo(int? id)
        {
            bool isGet = false;
            Product product = null;

            if (id == null)
            {
                isGet = false;
                product = null;
            }

            product = db.Products.FirstOrDefault(t => t.Id == id);

            if (product == null)
            {
                isGet = false;
                product = null;
            }
            else
            {
                isGet = true;
            }

            return Json(new { IsGet = isGet, ProductInfo = product, JsonRequestBehavior.AllowGet });
        }

        [HttpPost]
        public JsonResult Checkout(List<UserProducts> productsList, Order order)
        {
            bool isGet = false;
            string message = "Упс, щось пішло не так. Спробуйте пізніше...";

            string products = String.Empty;
            int productId;
            Product product;

            bool captcha = ReCaptcha.ValidateCaptcha(Request["g-recaptcha-response"]);

            for (int i = 0; i < productsList.Count; i++)
            {
                productId = productsList[i].Id;
                product = db.Products.Include(p => p.Category).FirstOrDefault(t => t.Id == productId);
                products += $"Товар - {Request.Url.Scheme}{Uri.SchemeDelimiter}{Request.Url.Authority}/{product.Category.Name}/{product.ProductCode}, кількість - {productsList[i].Quantity}\n";
            }

            if (String.IsNullOrWhiteSpace(products))
            {
                message = "Ваш кошик порожній!";
            }
            else
            {
                if (String.IsNullOrWhiteSpace(order.Name) || String.IsNullOrWhiteSpace(order.Phone) || String.IsNullOrWhiteSpace(order.Email))
                {
                    message = "Заповніть контактні дані!";
                }
                else
                {
                    if (order.Payment == "1")
                    {
                        if (order.Delivery == "1")
                        {
                            if (captcha)
                            {
                                try
                                {
                                    EmailService.SendEmail("denisukrvin@gmail.com", "Нове замовлення", $"--- Інформація по замовленню ---\n\nІм'я: {order.Name}\nТелефон: {order.Phone}\nПошта: {order.Email}\n\nСпосіб доставки: Самовивіз з магазину\nСпосіб оплати: Оплата при отриманні\n\nСписок товарів:\n{products}");
                                    EmailService.SendEmail($"{order.Email}", "Дякуємо за замовлення!", "Дякуємо за інтерес до товарів Online Store. Ваше замовлення отримано і надійде в обробку найближчим часом.");

                                    isGet = true;
                                    message = "Дякуємо за замовлення! Незабаром ми з вами зв'яжемось для підтвердження замовлення.";
                                }
                                catch (Exception ex)
                                {
                                    isGet = false;
                                    message = ex.ToString();
                                }
                            }                           
                            else
                            {
                                message = "Ви не пройшли перевірку ReCaptcha!";
                            }
                        }
                        else if (order.Delivery == "2")
                        {
                            if (String.IsNullOrWhiteSpace(order.PIB) || String.IsNullOrWhiteSpace(order.DeliveryCity) || String.IsNullOrWhiteSpace(order.DeliveryWarehouse))
                            {
                                message = "Заповніть дані для доставки";
                            }
                            else
                            {
                                if (captcha)
                                {
                                    try
                                    {
                                        EmailService.SendEmail("denisukrvin@gmail.com", "Нове замовлення", $"--- Інформація по замовленню ---\n\nІм'я: {order.Name}\nТелефон: {order.Phone}\nПошта: {order.Email}\n\nСпосіб доставки: Нова Пошта\nМісто: {order.DeliveryCity}\nНомер відділення: {order.DeliveryWarehouse}\nСпосіб оплати: Оплата при отриманні\n\nСписок товарів:\n{products}");
                                        EmailService.SendEmail($"{order.Email}", "Дякуємо за замовлення!", "Дякуємо за інтерес до товарів Online Store. Ваше замовлення отримано і надійде в обробку найближчим часом.");

                                        isGet = true;
                                        message = "Дякуємо за замовлення! Незабаром ми з вами зв'яжемось для підтвердження замовлення.";
                                    }
                                    catch (Exception ex)
                                    {
                                        message = $"Помилка: {ex.Message}";
                                    }
                                }
                                else
                                {
                                    message = "Ви не пройшли перевірку ReCaptcha!";
                                }
                            }                         
                        }
                        else
                        {
                            message = "Оберіть спосіб доставки!";
                        }
                    }
                    else if (order.Payment == "2")
                    {
                        if (order.Delivery == "1")
                        {
                            if (captcha)
                            {
                                try
                                {
                                    EmailService.SendEmail("denisukrvin@gmail.com", "Нове замовлення", $"--- Інформація по замовленню ---\n\nІм'я: {order.Name}\nТелефон: {order.Phone}\nПошта: {order.Email}\n\nСпосіб доставки: Самовивіз з магазину\nСпосіб оплати: Оплата на картку ПриватБанку\n\nСписок товарів:\n{products}");
                                    EmailService.SendEmail($"{order.Email}", "Дякуємо за замовлення!", "Дякуємо за інтерес до товарів Online Store. Ваше замовлення отримано і надійде в обробку найближчим часом.");

                                    isGet = true;
                                    message = "Дякуємо за замовлення! Незабаром ми з вами зв'яжемось для підтвердження замовлення.";
                                }
                                catch (Exception ex)
                                {
                                    isGet = false;
                                    message = ex.ToString();
                                }
                            }
                            else
                            {
                                message = "Ви не пройшли перевірку ReCaptcha!";
                            }                          
                        }
                        else if (order.Delivery == "2")
                        {
                            if (String.IsNullOrWhiteSpace(order.PIB) || String.IsNullOrWhiteSpace(order.DeliveryCity) || String.IsNullOrWhiteSpace(order.DeliveryWarehouse))
                            {
                                message = "Заповніть дані для доставки";
                            }
                            else
                            {
                                if (captcha)
                                {
                                    try
                                    {
                                        EmailService.SendEmail("denisukrvin@gmail.com", "Нове замовлення", $"--- Інформація по замовленню ---\n\nІм'я: {order.Name}\nТелефон: {order.Phone}\nПошта: {order.Email}\n\nСпосіб доставки: Нова Пошта\nМісто: {order.DeliveryCity}\nНомер відділення: {order.DeliveryWarehouse}\nСпосіб оплати: Оплата на картку ПриватБанку\n\nСписок товарів:\n{products}");
                                        EmailService.SendEmail($"{order.Email}", "Дякуємо за замовлення!", "Дякуємо за інтерес до товарів Online Store. Ваше замовлення отримано і надійде в обробку найближчим часом.");

                                        isGet = true;
                                        message = "Дякуємо за замовлення! Незабаром ми з вами зв'яжемось для підтвердження замовлення.";
                                    }
                                    catch (Exception ex)
                                    {
                                        message = $"Помилка: {ex.Message}";
                                    }
                                }
                                else
                                {
                                    message = "Ви не пройшли перевірку ReCaptcha!";
                                }                               
                            }
                        }
                        else
                        {
                            message = "Оберіть спосіб доставки!";
                        }
                    }
                    else
                    {
                        message = "Оберіть спосіб оплати!";
                    }
                }
            }

            return Json(new { IsGet = isGet, Message = message, JsonRequestBehavior.AllowGet });
        }

        [HttpGet]
        public ActionResult Checkout()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Delivery()
        {
            return View();
        }

        public ActionResult Payment()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}