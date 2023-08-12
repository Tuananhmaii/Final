using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RopinStore.Models
{
    public class ProductGallery
    {
        [Key]
        public int Id { get; set; }

        [ValidateNever]
        [DisplayName("Product")]
        public int ProductId { get; set; }

        [ValidateNever]
        public Product Product { get; set; }
        public string? URL { get; set; }
    }
}
