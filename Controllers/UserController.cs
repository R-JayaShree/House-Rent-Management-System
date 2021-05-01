using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HouseRentManagementSystem.Models;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web;
using System.Dynamic;
using System.Data.Entity.Migrations.Model;
using HouseRentManagementSystem.ViewModel;

namespace HouseRentManagementSystem.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        // Return Home page.
        public ActionResult UIndex()
        {
            Database1Entities5 dc = new Database1Entities5();
            string Emailid = User.Identity.Name;
            var acc = dc.RegisterUsers.FirstOrDefault(s => s.Email.Equals(Emailid));
            List<RegisterUser> register = new List<RegisterUser>();
            register.Add(acc);
            Session["Name"] = Convert.ToString(acc.FirstName+""+acc.LastName);
            ViewBag.image = acc.ProfileImage;
            return View("UIndex", "_LayoutUser");
        }

        public ActionResult AboutUs()
        {
            Database1Entities5 dc = new Database1Entities5();
            string Emailid = User.Identity.Name;
            var acc = dc.RegisterUsers.FirstOrDefault(s => s.Email.Equals(Emailid));
            Session["Name"] = Convert.ToString(acc.FirstName + "" + acc.LastName);
            ViewBag.image = acc.ProfileImage;
            return View();
        }

            public ActionResult BookingCancelView()
        {
            Database1Entities5 dc = new Database1Entities5();
            string Emailid = User.Identity.Name;
            var acc = dc.RegisterUsers.FirstOrDefault(s => s.Email.Equals(Emailid));
            Session["Name"] = Convert.ToString(acc.FirstName + "" + acc.LastName);
            ViewBag.image = acc.ProfileImage;
            return View();
        }
        
        public ActionResult SaveBookingDetails([Bind(Include = "Id,PropertyName,PropertyType,Status,Rent,PropertySize,PropertyAmenities,PropertySpecifications,PropertyRooms,Propertyblacony,PropertyBathrooms,PropertyAddress,PropertyDescription,PropertyImage,HouseImages,SellerName,SEmail,SPhoneno,SImage,HouseImage2,HouseImage3,HouseImage4,CustomerName,BookStatus")] PropertyTable Registers, HttpPostedFileBase image, string book)
        {
            Database1Entities5 dc = new Database1Entities5();
            string Emailid = User.Identity.Name;
            int PropId = @Convert.ToInt32(Session["PropId"]);
            BookStatu reglog = new BookStatu(); 
            var reglog1 = dc.PropertyTables.FirstOrDefault(s => s.Prop_id == PropId);
            var acc = dc.RegisterUsers.FirstOrDefault(s => s.Email == Emailid);
            int CusId = acc.Id;
            if (book != null && book== "OK")
            {
                DateTime now = DateTime.Now;
                reglog1.BookTime = now;
                reglog.Prop_Id = PropId;
                reglog.Cus_Id = CusId;
                reglog.BookStatus = "Waiting";
                reglog1.BookStatus = "Waiting";
                reglog1.CustomerName = Emailid;
                dc.BookStatus.Add(reglog);
                dc.SaveChanges();
                return RedirectToAction("BookedDetails");
            }
            else if(book== "Cancel")
            {
                return RedirectToAction("ViewFullDetails", new { Id = PropId });
            }
            return View();
        }
        public ActionResult SaveBookingDetails1([Bind(Include = "Id,PropertyName,PropertyType,Status,Rent,PropertySize,PropertyAmenities,PropertySpecifications,PropertyRooms,Propertyblacony,PropertyBathrooms,PropertyAddress,PropertyDescription,PropertyImage,HouseImages,SellerName,SEmail,SPhoneno,SImage,HouseImage2,HouseImage3,HouseImage4,CustomerName,BookStatus")] PropertyTable Registers, HttpPostedFileBase image, string book)
        {
            Database1Entities5 dc = new Database1Entities5();
            string Emailid = User.Identity.Name;
            int PropId = @Convert.ToInt32(Session["PropId"]);
            var reglog = dc.BookStatus.FirstOrDefault(s => s.Prop_Id == PropId);
            var reglog1 = dc.PropertyTables.FirstOrDefault(s => s.Prop_id == PropId);
            if (book != null && book == "Cancel" || book=="Yes")
            {
                reglog.BookStatus = "Cancelled";
                reglog1.BookStatus = null;
                reglog1.CustomerName = null;
                reglog1.BookTime = null;
                dc.SaveChanges();
                return RedirectToAction("BookingCancelView");
            }
            else if (book == "Wait" || book=="No")
            {
                return RedirectToAction("ViewFullDetails1", new { Id = PropId });
            }
            return View();
        }
        public ActionResult Cancelbook([Bind(Include = "Id,PropertyName,PropertyType,Status,Rent,PropertySize,PropertyAmenities,PropertySpecifications,PropertyRooms,Propertyblacony,PropertyBathrooms,PropertyAddress,PropertyDescription,PropertyImage,HouseImages,SellerName,SEmail,SPhoneno,SImage,HouseImage2,HouseImage3,HouseImage4,CustomerName,BookStatus")] PropertyTable Registers, HttpPostedFileBase image, int? Id)
        {
            Database1Entities5 dc = new Database1Entities5();
            int PropId = @Convert.ToInt32(Session["PropId"]);
            var reglog = dc.BookStatus.FirstOrDefault(s => s.Prop_Id == Id);
            var reglog1 = dc.PropertyTables.FirstOrDefault(s => s.Prop_id == Id);
            reglog.BookStatus = "Cancelled";
            reglog1.BookStatus = null;
            reglog1.CustomerName = null;
            reglog1.BookTime = null;
            dc.SaveChanges();
            return RedirectToAction("BookingCancelView");
        }
        public ActionResult BookedDetails()
        {
            Database1Entities5 dc = new Database1Entities5();
            string Emailid = User.Identity.Name;
            var acc1 = dc.RegisterUsers.FirstOrDefault(s => s.Email.Equals(Emailid));
            ViewBag.image = acc1.ProfileImage;
            List<PropertyTable> acc = dc.PropertyTables.Where(x => (x.BookStatus == "Waiting" || x.BookStatus != "Accepted" || x.BookStatus != "Cancelled") && (x.CustomerName==Emailid)).ToList();
            return View(acc);
        }

        public ActionResult AllProperties(string search)
        {
            Database1Entities5 dc = new Database1Entities5();
            string Emailid = User.Identity.Name;
            var acc1 = dc.RegisterUsers.FirstOrDefault(s => s.Email.Equals(Emailid));
            ViewBag.image = acc1.ProfileImage;
            List<PropertyTable> acc = dc.PropertyTables.Where(x => x.BookStatus != "Waiting" && x.BookStatus != "Accepted" && x.BookStatus!= "Rejected").ToList();
            //long rent = long.Parse(search);
            var acc2 = dc.RegisterCustomers.FirstOrDefault(s => s.Email.Equals(Emailid));
            if (search != null || search == "")
            {
                return View(dc.PropertyTables.Where(x => (x.BookStatus != "Waiting" || x.BookStatus != "Accepted") &&  (x.PropertyAddress == search || x.PropertyName == search || x.PropertyType == search || x.Status == search)).ToList());
            }
            else
            {
                return View(acc);
            }
        }
        
        public ActionResult ViewFullDetails(int? Id)
        {
            Database1Entities5 dc = new Database1Entities5();
            string Emailid = User.Identity.Name;
            var acc1 = dc.RegisterUsers.FirstOrDefault(s => s.Email.Equals(Emailid));
            ViewBag.image = acc1.ProfileImage;
            Session["Name"] = Convert.ToString(acc1.FirstName + "" + acc1.LastName);
            Session["PropId"] = Convert.ToString(Id);
            var tables = new AllTable
            {
                Property_Images = dc.Property_Images.Where(m => m.Property_Id == Id).ToList(),
                PropertyTable = dc.PropertyTables.Where(m => m.Prop_id == Id).ToList()
            };
            return View(tables);
        }
        public ActionResult ViewFullDetails1(int? Id)
        {
            Database1Entities5 dc = new Database1Entities5();
            string Emailid = User.Identity.Name;
            var acc1 = dc.RegisterUsers.FirstOrDefault(s => s.Email.Equals(Emailid));
            ViewBag.image = acc1.ProfileImage;
            Session["Name"] = Convert.ToString(acc1.FirstName + "" + acc1.LastName);
            Session["PropId"] = Convert.ToString(Id);
            var tables = new AllTable
            {
                Property_Images = dc.Property_Images.Where(m => m.Property_Id == Id).ToList(),
                PropertyTable = dc.PropertyTables.Where(m => m.Prop_id == Id).ToList()
            };
            return View(tables);
        }


        public ActionResult Logout()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Login", "Account");
        }
        public ActionResult UserData()
        {
            Database1Entities5 dc = new Database1Entities5();
            string Emailid = User.Identity.Name;
            var acc = dc.RegisterUsers.FirstOrDefault(s => s.Email.Equals(Emailid));
            List<RegisterUser> register = new List<RegisterUser>();
            register.Add(acc);
            Session["Name"] = Convert.ToString(acc.FirstName + " " + acc.LastName);
            ViewBag.image = acc.ProfileImage;

            return View(register.AsEnumerable());
        }
        public ActionResult Edit(int? Id)
        {
            Database1Entities5 dc = new Database1Entities5();
            var acc = dc.RegisterUsers.Where(s => s.Id == Id).FirstOrDefault();

            if (acc == null)
            {
                return HttpNotFound();
            }
            return View(acc);
        }
        [HttpPost]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Email,Password,PhoneNumber,Aadharno,ResetPasswordCode")] RegisterUser Registers, HttpPostedFileBase image)
        {
            Database1Entities5 dc = new Database1Entities5();
            if (image != null)
            {
                Registers.ProfileImage = new byte[image.ContentLength];
                image.InputStream.Read(Registers.ProfileImage, 0, image.ContentLength);
            }
            if (ModelState.IsValid)

            {
                dc.Entry(Registers).State = EntityState.Modified;
                dc.SaveChanges();
                return RedirectToAction("UserData");
            }
            return View(Registers);
        }
    }
}