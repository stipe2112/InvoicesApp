using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using InvoicesApp.Models;

namespace InvoicesApp.DAL
{
    public class WarehouseInitializer : DropCreateDatabaseIfModelChanges<WarehouseContext>
    {
        protected override void Seed(WarehouseContext context)
        {
            var items = new List<Item>
            {
            new Item{Name="Monitor",UnitPrice=99.99M},
            new Item{Name="Keyboard",UnitPrice=39.99M},
            new Item{Name="Mouse",UnitPrice=29.99M },
            new Item{Name="Speakers",UnitPrice=49.99M},
            new Item{Name="Office Chair",UnitPrice=199.99M},
            new Item{Name="Office Desk",UnitPrice=299.99M},
            new Item{Name="Lightbulb",UnitPrice=0.99M},
            new Item{Name="Coffee",UnitPrice=9.99M},
            new Item{Name="Power Cable",UnitPrice=4.99M}
            };

            items.ForEach(s => context.Items.Add(s));
            context.SaveChanges();
        }
    }
}