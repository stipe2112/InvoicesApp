using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.Composition;

namespace InvoicesApp.Features.TaxCalculationFeature.TaxCalculatorExtensions
{
    [Export(typeof(ITax))]
    [ExportMetadata("Tax", "Bosnian")]
    public class BosnianTax : ITax
    {
        public decimal Operate(decimal price)
        {
            return decimal.Multiply(price, Taxes.BosniaTax);
        }
    }
}