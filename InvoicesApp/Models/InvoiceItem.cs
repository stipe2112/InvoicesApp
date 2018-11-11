using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoicesApp.Models
{
    public class InvoiceItem
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int ItemId { get; set; }

        [Required]
        [Range(1, 99, ErrorMessage = "Maximum number of units is 99")]
        public int Quantity { get; set; }

        [DataType(DataType.Currency), Display(Name = "Price")]
        public decimal TotalPrice { get; set; }
        

        public virtual Invoice Invoice { get; set; }
        public virtual Item Item { get; set; }


    }
}