using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HouseRentManagementSystem.ViewModel
{
    public class BookStatusModel
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int Cus_Id { get; set; }
        public int Prop_Id { get; set; }
        public string BookStatus { get; set; }


    }
}