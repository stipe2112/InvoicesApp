using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using InvoicesApp.DAL;
using InvoicesApp.Models;
using PagedList;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using InvoicesApp.Features;

namespace InvoicesApp.Controllers
{
    public class InvoiceController : Controller
    {
        private WarehouseContext db = new WarehouseContext();

        private CompositionContainer _container;

        [Import(typeof(ITaxCalculator))]
        public ITaxCalculator calculator;


        // GET: Invoice
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            //Sortiranje i pamćenje odabira pri listanju stranica
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParam = String.IsNullOrEmpty(sortOrder) ? "IdDesc" : "";
            ViewBag.CreationDateSortParam = sortOrder == "CreationDate" ? "CreationDateDesc" : "CreationDate";
            ViewBag.DueDateSortParam = sortOrder == "DueDate" ? "DueDateDesc" : "DueDate";
            ViewBag.RecipientSortParam = sortOrder == "Recipient" ? "RecipientDesc" : "Recipient";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var invoices = from i in db.Invoices select i;

            if (!String.IsNullOrEmpty(searchString))
            {
                invoices = invoices.Where(i => i.Recipient.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "RecipientDesc":
                    invoices = invoices.OrderByDescending(i => i.Recipient);
                    break;
                case "Recipient":
                    invoices = invoices.OrderBy(i => i.Recipient);
                    break;
                case "CreationDate":
                    invoices = invoices.OrderBy(i => i.CreationDate);
                    break;
                case "CreationDateDesc":
                    invoices = invoices.OrderByDescending(i => i.CreationDate);
                    break;
                case "DueDate":
                    invoices = invoices.OrderBy(i => i.DueDate);
                    break;
                case "DueDateDesc":
                    invoices = invoices.OrderByDescending(i => i.DueDate);
                    break;
                case "IdDesc":
                    invoices = invoices.OrderByDescending(i => i.Id);
                    break;
                default:
                    invoices = invoices.OrderBy(i => i.Id);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(invoices.ToPagedList(pageNumber, pageSize));
        }



        // GET: Invoice/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Invoice invoice = db.Invoices.Find(id);

            if (invoice == null)
            {
                return HttpNotFound();
            }

            return View(invoice);
        }



        // GET: Invoice/Create
        public ActionResult Create()
        {
            /* sastavljanje kalkulatora za dohvaćanje podataka svih dostupnih kalkulatora poreza za prikaz 
             * dropdown liste (vjerojatno postoji elegantije rjesenje)*/

            ComposeTaxCalculator();
            ViewBag.Taxes = new SelectList(calculator.TaxesList());

            return View();
        }

        public void ComposeTaxCalculator()
        {
            //An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();
            //Adds all the parts found in the same assembly as the Program class
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(InvoiceController).Assembly));

            //Create the CompositionContainer with the parts in the catalog
            _container = new CompositionContainer(catalog);

            //Fill the imports of this object
            try
            {
                this._container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }



        // POST: Invoice/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DueDate,Recipient, Taxes")] Invoice invoice)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ComposeTaxCalculator();

                    invoice.CreationDate = DateTime.Now;
                    invoice.TotalCost = invoice.invoiceTotalCost();
                    invoice.TotalCostWithTax = calculator.CalculateTax(invoice.Taxes, invoice.TotalCost);

                    db.Invoices.Add(invoice);
                    db.SaveChanges();

                    return RedirectToAction("Edit", new { id = invoice.Id });
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }


            return View(invoice);
        }



        // GET: Invoice/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Invoice invoice = db.Invoices.Where(i => i.Id == id).Single();

            ViewBag.itemId = new SelectList(db.Items, "Id", "Name");

            ComposeTaxCalculator();
            ViewBag.Taxes = new SelectList(calculator.TaxesList(), invoice.Taxes);

            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }



        // POST: Invoice/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DueDate, Recipient")] int? id, string taxes, string itemId, int quantity = 0)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (quantity == 0)
            {
                return RedirectToAction("Edit", new { id });
            }

            var invoiceToUpdate = db.Invoices.Where(i => i.Id == id).Single();

            if (TryUpdateModel(invoiceToUpdate, "", new string[] { "DueDate", "Recipient", "InvoiceItems", "Taxes" }))
            {

                try
                {
                    int itemIdParsed = int.Parse(itemId);
                    Item item = db.Items.Where(i => i.Id == itemIdParsed).Single();

                    InvoiceItem itemToAdd = new InvoiceItem
                    {
                        InvoiceId = invoiceToUpdate.Id,
                        ItemId = itemIdParsed,
                        Quantity = (int)quantity,
                        Item = item,
                        TotalPrice = decimal.Multiply(item.UnitPrice, quantity)
                    };

                    invoiceToUpdate.InvoiceItems.Add(itemToAdd);

                    ComposeTaxCalculator();
                    invoiceToUpdate.TotalCost = invoiceToUpdate.invoiceTotalCost();
                    invoiceToUpdate.TotalCostWithTax = calculator.CalculateTax(taxes, invoiceToUpdate.TotalCost);

                    db.SaveChanges();

                    return RedirectToAction("Edit");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            return View(invoiceToUpdate);
        }

        // POST: Invoice/RemoveInvoiceItem/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveInvoiceItem(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var invoiceToUpdate = db.InvoiceItems.Find(id).Invoice;

            try
            {
                db.InvoiceItems.Remove(db.InvoiceItems.Where(i => i.Id == id).Single());

                ComposeTaxCalculator();
                invoiceToUpdate.TotalCost = invoiceToUpdate.invoiceTotalCost();
                invoiceToUpdate.TotalCostWithTax = calculator.CalculateTax(invoiceToUpdate.Taxes, invoiceToUpdate.TotalCost);

                db.SaveChanges();
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return RedirectToAction("Edit", new { id = invoiceToUpdate.Id });

        }


        // GET: Invoice/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }

            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // POST: Invoice/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Invoice invoice = db.Invoices.Find(id);
                db.Invoices.Remove(invoice);
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
