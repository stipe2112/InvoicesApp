using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Web.Mvc;



namespace InvoicesApp.Features
{
    [Export(typeof(ITaxCalculator))]
    class TaxCalculator : ITaxCalculator
    {
        [ImportMany]
        IEnumerable<Lazy<ITax, ITaxData>> operations;

        public decimal CalculateTax(string country, decimal price)
        { 
            foreach (Lazy<ITax, ITaxData> i in operations)
            {
                if (i.Metadata.Tax.Equals(country)) return i.Value.Operate(price);
            }

            return -1;
        }

        public List<string> TaxesList()
        {
            List<string> list = new List<string>();
            foreach (Lazy<ITax, ITaxData> o in operations)
            {
                list.Add(o.Metadata.Tax);
            }

            return list;
        }

    }
}
