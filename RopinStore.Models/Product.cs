﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RopinStore.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [ValidateNever]
        public string Gender { get; set; }
        [Required]
        public string Color { get; set; }
        [Required]
        public string Description { get; set; }       
        [Required]
        public string Specification { get; set; }
        [Required]
        [Range(1, 10000)]
        public double Price { get; set; }

        [ValidateNever]
        [DisplayName("Image URL")]
        public string ImageUrl { get; set; }

        [ValidateNever]
        [DisplayName("Second Image")]
        public string SecondImage { get; set; }

        [Required]
        [DisplayName("Category")]
        public int CategoryId { get; set; }

        [ValidateNever]
        public Category Category { get; set; }

        [Required]
        [DisplayName("Brand")]
        public int BrandId { get; set; }

        [ValidateNever]
        public Brand Brand { get; set; }

        [Required]
        [DisplayName("Collection")]
        public int CollectionId { get; set; }

        [ValidateNever]
        public Collection Collection { get; set; }


        [DisplayName("Product Gallery")]
        public List<ProductGallery> Gallery { get; set; } = new List<ProductGallery>();
    }
}
