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
    public class Order
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        public double TotalPrice { get; set; }
        public string? PaymentType { get; set; }
        public string? OrderStatus { get; set; }
        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        [DisplayName("Zip Code")]
        public string ZipCode { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string State { get; set; }
    }
}
