using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using OnlineStore.Models;

namespace OnlineStore.Controllers
{
    public class AdminController : Controller
    {
        private DBContext db = new DBContext();

        [Authorize]
        public ActionResult Index()
        {
            return View();      
        }
             
        [HttpGet]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }             
            else
            {
                return View();
            }              
        }

        [HttpPost]
        public JsonResult Login(Admin model)
        {
            bool isLogin = false;
            Admin admin = null;
            string message = "Упс, щось пішло не так. Спробуйте пізніше...";
            bool captcha = ReCaptcha.ValidateCaptcha(Request["g-recaptcha-response"]);

            if (String.IsNullOrWhiteSpace(model.Login) || String.IsNullOrWhiteSpace(model.Password) || !captcha)
            {
                message = "Заповніть усі поля!";
            }
            else
            {              
                admin = db.Admins.FirstOrDefault(u => u.Login == model.Login);

                if (admin != null)
                {
                    var verifyUserPass = SecurePasswordHasher.Verify(model.Password, admin.Password);

                    if (verifyUserPass)
                    {
                        FormsAuthentication.SetAuthCookie(model.Login, true);
                        isLogin = true;
                    }
                    else
                    {
                        message = "Невірно вказаний пароль";
                    }
                }
                else
                {
                    message = "Користувача з таким логіном не існує";
                }
            }
                                         
            return Json(new { IsLogin = isLogin, Message = message, JsonRequestBehavior.AllowGet });
        }

        [Authorize]
        public ActionResult Newsletter()
        {
            var emails = db.UserEmails;

            return View(emails);
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateNewsletter()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public JsonResult CreateNewsletter(string subject, string text)
        {
            bool isCreate = false;
            string message = "Упс, щось пішло не так. Спробуйте пізніше...";

            if (String.IsNullOrWhiteSpace(subject) || String.IsNullOrWhiteSpace(text))
            {
                message = "Заповніть усі поля!";
            }
            else
            {
                try
                {
                    var emailList = db.UserEmails;

                    foreach (var user in emailList)
                    {
                        EmailService.SendEmail(user.Email, subject, text);
                    }

                    isCreate = true;
                    message = "Розсилка успішно завершена!";
                }
                catch (Exception ex)
                {
                    message = $"Помилка: {ex.Message}";
                }
            }

            return Json(new { IsCreate = isCreate, Message = message, JsonRequestBehavior.AllowGet });
        }

        [Authorize]
        public ActionResult Products()
        {
            var products = db.Products;

            return View(products);
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateProduct()
        {
            var categories = db.Categories;
            ViewBag.Categories = categories;

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateProduct(Product product)
        {
            if (product.CategoryId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            db.Products.Add(product);
            db.SaveChanges();

            return RedirectToAction("Products");
        }

        [Authorize]
        public ActionResult DetailsProduct(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = db.Products.Include(t => t.Category).FirstOrDefault(t => t.Id == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditProduct(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = db.Products.Find(id);
            var categories = db.Categories;            

            if (product == null || categories == null)
            {
                return HttpNotFound();
            }

            ViewBag.Categories = categories;

            return View(product);
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditProduct(Product product)
        {
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Products");
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteProduct(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = db.Products.Include(t => t.Category).FirstOrDefault(t => t.Id == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("DeleteProduct")]
        [Authorize]
        public ActionResult DeleteProduct(int id)
        {
            Product b = db.Products.Find(id);

            if (b == null)
            {
                return HttpNotFound();
            }

            db.Products.Remove(b);
            db.SaveChanges();

            return RedirectToAction("Products");
        }

        [Authorize]
        public ActionResult Categories()
        {
            var categories = db.Categories;

            return View(categories);
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateCategory(Category category)
        {
            db.Categories.Add(category);
            db.SaveChanges();

            return RedirectToAction("Categories");
        }

        [Authorize]
        public ActionResult DetailsCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = db.Categories.Find(id);

            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = db.Categories.Find(id);

            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditCategory(Category category)
        {         
            db.Entry(category).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Categories");
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = db.Categories.Find(id);

            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("DeleteCategory")]
        [Authorize]
        public ActionResult DeleteCategory(int id)
        {
            Category b = db.Categories.Find(id);

            if (b == null)
            {
                return HttpNotFound();
            }           

            db.Categories.Remove(b);
            db.SaveChanges();

            return RedirectToAction("Categories");
        }       

        [Authorize]
        public ActionResult Comments()
        {
            var comments = db.Comments;

            return View(comments);
        }

        [HttpGet]
        [Authorize]      
        public ActionResult CreateComment()
        {
            var products = db.Products;
            ViewBag.Products = products;

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateComment(Comment comment)
        {
            if (comment.ProductId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            db.Comments.Add(comment);
            db.SaveChanges();

            return RedirectToAction("Comments");
        }

        [Authorize]
        public ActionResult DetailsComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment comment = db.Comments.Include(t => t.Product).FirstOrDefault(t => t.Id == id);

            if (comment == null)
            {
                return HttpNotFound();
            }

            return View(comment);
        }

        [HttpGet]
        [Authorize]       
        public ActionResult EditComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment comment = db.Comments.Find(id);
            var products = db.Products;

            if (comment == null || products == null)
            {
                return HttpNotFound();
            }

            ViewBag.Products = products;

            return View(comment);
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditComment(Comment comment)
        {
            db.Entry(comment).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Comments");
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment comment = db.Comments.Include(t => t.Product).FirstOrDefault(t => t.Id == id);

            if (comment == null)
            {
                return HttpNotFound();
            }

            return View(comment);
        }

        [HttpPost, ActionName("DeleteComment")]
        [Authorize]
        public ActionResult DeleteComment(int id)
        {
            Comment b = db.Comments.Find(id);

            if (b == null)
            {
                return HttpNotFound();
            }

            db.Comments.Remove(b);           
            db.SaveChanges();

            return RedirectToAction("Comments");
        }

        [Authorize]
        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Admin");
        }
    }
}