using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;


namespace HouseRentManagementSystem.ViewModel
{
    public class PropertyModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string PropertyName { get; set; }

        [Required]
        public string PropertyType { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public long Rent { get; set; }

        [Required]
        public string PropertySize { get; set; }

        [Required]
        public string PropertyAmenities { get; set; }

        [Required]
        public string PropertySpecifications { get; set; }

        [Required]
        public string PropertyRooms { get; set; }

        [Required]
        public string Propertyblacony { get; set; }

        [Required]
        public string PropertyBathrooms { get; set; }

        [Required]
        public string PropertyAddress { get; set; }

        [Required]
        public string PropertyDescription { get; set; }

        
        public byte[] PropertyImage { get; set; }

        public string SellerName { get; set; }
        public string SEmail { get; set; }
        public long SPhoneno { get; set; }

        public byte[] SImage { get; set; }

        public DateTime BookTime { get; set; }

        public int Prop_Id { get; set; }

    }
}