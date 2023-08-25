using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RopinStore.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }

        [ValidateNever]
        [DisplayName("Product Gallery")]
        public List<ProductGallery> Gallery { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }


        [Range(1, 1000, ErrorMessage = "Enter between 1 and 1000")]
        public int Count { get; set; }
        [NotMapped]
        public double Price { get; set; }
        [ValidateNever]
        [DisplayName("Reviews")]
        public List<Review> Review { get; set; }
    }
}
