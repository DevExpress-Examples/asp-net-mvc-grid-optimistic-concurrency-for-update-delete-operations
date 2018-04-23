using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using GridViewOptimisticConcurrencyMvc.Models;

namespace GridViewOptimisticConcurrencyMvc.Controllers {
    public class HomeController : Controller {
        private CustomerDbContext db = new CustomerDbContext();

        public ActionResult Index() {
            return View();
        }

        public ActionResult GridViewPartial() {
            return PartialView(db.Customers.ToList());
        }

        public ActionResult GridViewPartialAddNew(Customer customer) {
            var model = db.Customers;

            if (ModelState.IsValid) {
                try {
                    db.Entry(customer).State = System.Data.Entity.EntityState.Added;
                    db.SaveChanges();
                }
                catch (Exception e) {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("GridViewPartial", db.Customers.ToList());
        }

        public ActionResult GridViewPartialUpdate(Customer customer) {
            var model = db.Customers;

            customer.RowVersion = CalculateOldRowVersion(customer.Id);

            if (ModelState.IsValid) {
                try {
                    db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception e) {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("GridViewPartial", db.Customers.ToList());
        }

        public ActionResult GridViewPartialDelete(Customer customer) {
            var model = db.Customers;

            customer.RowVersion = CalculateOldRowVersion(customer.Id);

            if (ModelState.IsValid) {
                try {
                    db.Entry(customer).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                catch (Exception e) {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("GridViewPartial", db.Customers.ToList());
        }

        private byte[] CalculateOldRowVersion(int id) {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string rowVersions = Request["RowVersions"];
            Dictionary<object, string> dictionary = (Dictionary<object, string>)serializer.Deserialize(rowVersions, typeof(Dictionary<object, string>));
            char[] rowVersion = dictionary[id.ToString()].ToCharArray();

            return Convert.FromBase64CharArray(rowVersion, 0, rowVersion.Length);
        }
    }
}