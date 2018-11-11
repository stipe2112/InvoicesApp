using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace InvoicesApp.Features
{
    public interface ITaxCalculator
    {
        decimal CalculateTax(string country, decimal price);
        List<string> TaxesList();
    }
}
