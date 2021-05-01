//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HouseRentManagementSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PropertyTable
    {
        public int Id { get; set; }
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public string Status { get; set; }
        public long Rent { get; set; }
        public string PropertySize { get; set; }
        public string PropertyAmenities { get; set; }
        public string PropertySpecifications { get; set; }
        public string PropertyRooms { get; set; }
        public string Propertyblacony { get; set; }
        public string PropertyBathrooms { get; set; }
        public string PropertyAddress { get; set; }
        public string PropertyDescription { get; set; }
        public string SellerName { get; set; }
        public string SEmail { get; set; }
        public Nullable<long> SPhoneno { get; set; }
        public byte[] SImage { get; set; }
        public string CustomerName { get; set; }
        public string BookStatus { get; set; }
        public byte[] PropertyImage { get; set; }
        public Nullable<System.DateTime> BookTime { get; set; }
        public Nullable<int> Prop_id { get; set; }
    }
}