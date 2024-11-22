using PracticeWebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PracticeWebApp.ViewModels
{
    public class ProductEditModel
    {
        public int ProductId { get; set; }
        [Required,StringLength(50)]
        public string ProductName { get; set; }
        [Required,DataType(DataType.Date)]
        public DateTime MfgDate { get; set; }
        [Required, DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public HttpPostedFileBase Picture { get; set; }
        public bool InStock { get; set; }

        public virtual List<Sale> Sales { get; set; }=new List<Sale>();
    }
}