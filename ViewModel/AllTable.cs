using HouseRentManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseRentManagementSystem.ViewModel
{
    public class AllTable
    {
        public IEnumerable<RegisterUser> RegisterUser { get; set; }
        public IEnumerable<RegisterCustomer> RegisterCustomer { get; set; }
        public IEnumerable<PropertyTable> PropertyTable { get; set; }

        public IEnumerable<Property_Images> Property_Images { get; set; }

        public IEnumerable<BookStatu> BookStatus { get; set; }
    }
}