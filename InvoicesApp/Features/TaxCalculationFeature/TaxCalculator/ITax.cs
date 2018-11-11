using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoicesApp.Features
{
    public interface ITax
    {
        decimal Operate(decimal price);
    }
}
