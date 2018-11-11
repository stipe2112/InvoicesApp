using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;

namespace InvoicesApp.Features
{
    [Export(typeof(ITax))]
    [ExportMetadata("Tax", "Croatian")]
    class CroatianTax : ITax
    {
        public decimal Operate(decimal price)
        {
            return decimal.Multiply(price, Taxes.CroatiaTax);
        }
    }
}
