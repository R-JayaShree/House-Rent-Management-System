using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HouseRentManagementSystem.ViewModel
{
    public class Imagemodel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int Property_Id { get; set; }
        public Byte[] Image { get; set; }
        

    }
}