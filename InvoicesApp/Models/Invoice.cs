using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace InvoicesApp.Models
{
    public class Invoice
    {
        
        [Display(Name = "Number")]
        public int Id { get; set; }

        
        //[Display(Name = "User")]
        //public int ApplicationUserId { get; set; }
        

        [DataType(DataType.Date), Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreationDate { get; set; }


        [Required]
        [DataType(DataType.Date), Display(Name = "Due Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }


        [StringLength(50, ErrorMessage = "Recipients name cannot be longer than 50 characters.")]
        public string Recipient { get; set; }

     
        [Display(Name = "Invoice Items")]
        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; }


        [Required, DataType(DataType.Currency), Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }


        [Required, DataType(DataType.Currency), Display(Name = "Total Cost W/ Tax")]
        public decimal TotalCostWithTax { get; set; }

        [Required]
        public string Taxes { get; set; }


        public decimal invoiceTotalCost()
        {
            decimal cost = 0;

            if(InvoiceItems != null)
            {
                foreach (InvoiceItem item in InvoiceItems)
                {
                    cost += item.Item.UnitPrice * item.Quantity;
                }
            }
            
            return cost;
        }
    }
}