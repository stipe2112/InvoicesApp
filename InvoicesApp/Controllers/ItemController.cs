using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using InvoicesApp.DAL;
using InvoicesApp.Models;
using PagedList;

namespace InvoicesApp.Controllers
{
    public class ItemController : Controller
    {
        private WarehouseContext db = new WarehouseContext();


        // GET: Item
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "NameDesc" : "Name";
            ViewBag.PriceSortParam = sortOrder == "Price" ? "PriceDesc" : "Price";
            ViewBag.PriceSortParam = sortOrder == "Id" ? "IdDesc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var items = from i in db.Items select i;

            if (!String.IsNullOrEmpty(searchString))
            {
                items = items.Where(i => i.Name.Contains(searchString) || i.UnitPrice.ToString().StartsWith(searchString));
            }

            switch (sortOrder)
            {
                case "NameDesc":
                    items = items.OrderByDescending(i => i.Name);
                    break;
                case "Name":
                    items = items.OrderBy(i => i.Name);
                    break;
                case "Price":
                    items = items.OrderBy(i => i.UnitPrice);
                    break;
                case "PriceDesc":
                    items = items.OrderByDescending(i => i.UnitPrice);
                    break;
                case "IdDesc":
                    items = items.OrderByDescending(i => i.Id);
                    break;
                default:
                    items = items.OrderBy(i => i.Id);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(items.ToPagedList(pageNumber, pageSize));
        }

        // GET: Item/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Item item = db.Items.Find(id);

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // GET: Item/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Item/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,UnitPrice")] Item item)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Items.Add(item);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            
            return View(item);
        }

        // GET: Item/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Item/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost([Bind(Include = "Name,UnitPrice")] int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var itemToUpdate = db.Items.Find(id);

            if(TryUpdateModel(itemToUpdate, "", new string[] { "Name", "UnitPrice" })) {

                try
                {
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            return View(itemToUpdate);
        }

        // GET: Item/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }

            Item item = db.Items.Find(id);

            if (item == null)
            {
                return HttpNotFound();
            }

            return View(item);
        }

        // POST: Item/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Item item = db.Items.Find(id);
                db.Items.Remove(item);
                db.SaveChanges();
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
