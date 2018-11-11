using System.Linq;
using System.Web.Mvc;
using InvoicesApp.DAL;
using InvoicesApp.ViewModels;

namespace InvoicesApp.Controllers
{
    public class HomeController : Controller
    {
        private WarehouseContext db = new WarehouseContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            IQueryable<CreationDateGroup> data = from invoice in db.Invoices
                                                 group invoice by invoice.CreationDate into dateGroup
                                                 select new CreationDateGroup()
                                                 {
                                                     CreationDate = dateGroup.Key,
                                                     InvoicesCount = dateGroup.Count()
                                                 };


            return View(data.ToList());
        }

        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}