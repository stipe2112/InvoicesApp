using System;
using System.ComponentModel.DataAnnotations;

namespace InvoicesApp.ViewModels
{
    public class CreationDateGroup
    {
        [DataType(DataType.Date)]
        public DateTime? CreationDate { get; set; }

        public int InvoicesCount { get; set; }
    }
}