using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoicesApp.Models
{
    public class Item
    { 
        public int Id { get; set; }

        [Required, StringLength(50, ErrorMessage = "Item name cannot be longer than 50 characters.")]
        public string Name { get; set; }

        [Required, DataType(DataType.Currency), Display(Name = "Unit Price")]
        //[Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }

    }
}