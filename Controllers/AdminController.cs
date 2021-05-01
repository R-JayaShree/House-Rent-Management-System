using HouseRentManagementSystem.Models;
using HouseRentManagementSystem.ViewModel;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;
using System.Web.Helpers;
using System.Data.Entity;
using System.Drawing;
using System.IO;

namespace HouseRentManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult AIndex()
        {
            Database1Entities5 dc = new Database1Entities5();
            string Emailid = User.Identity.Name;
            var acc = dc.RegisterCustomers.FirstOrDefault(s => s.Email.Equals(Emailid));
            List<RegisterCustomer> register = new List<RegisterCustomer>();
            register.Add(acc);
            Session["Name"] = Convert.ToString(acc.FirstName+""+acc.LastName);
            Session["SEmail"] = Convert.ToString(acc.Email);
            Session["SPhone"] = Convert.ToString(acc.PhoneNumber);
            ViewBag.image = acc.ProfileImage;
            return View("AIndex","_LayoutAdmin");
        }
        
        public ActionResult TopSellers()

        {
            Database1Entities5 dc = new Database1Entities5();
            string Emailid = User.Identity.Name;
            var acc1 = dc.RegisterCustomers.FirstOrDefault(s => s.Email.Equals(Emailid));
            ViewBag.image = acc1.ProfileImage;
            List<RegisterCustomer> acc = dc.RegisterCustomers.OrderByDescending(s => s.BookCount).Where(s => s.BookCount != null).ToList();
            return View(acc);
        }

        public ActionResult AddProperty()
        {
            Database1Entities5 dc = new Database1Entities5();
            string Emailid = User.Identity.Name;
            var acc = dc.RegisterCustomers.FirstOrDefault(s => s.Email.Equals(Emailid));
            Session["Name"] = Convert.ToString(acc.FirstName + "" + acc.LastName);
            Session["SEmail"] = Convert.ToString(acc.Email);
            Session["SPhone"] = Convert.ToString(acc.PhoneNumber);
            ViewBag.image = acc.ProfileImage;
            return View();
        }
        public ActionResult ViewProperty()
        {
            Database1Entities5 dc = new Database1Entities5();
            string Emailid = User.Identity.Name;
            var acc1 = dc.RegisterCustomers.FirstOrDefault(s => s.Email.Equals(Emailid));
            string Sname = @Convert.ToString(Session["Name"]);
            var acc = dc.PropertyTables.Where(m => m.SEmail == Emailid).ToList();
            ViewBag.image = acc1.ProfileImage;
            return View(acc.AsEnumerable());
            
        }
        public ActionResult DeleteProperty(int? Id)
        {
                Database1Entities5 dc = new Database1Entities5();
                var itemToRemove = dc.PropertyTables.Where(a => a.Prop_id == Id).FirstOrDefault();
                List<Property_Images> property_Images = dc.Property_Images.Where(a => a.Property_Id == Id).ToList();
                foreach(var i in property_Images) { 
                    dc.Entry(i).State = EntityState.Deleted;
                }
                dc.Entry(itemToRemove).State = EntityState.Deleted;
                dc.SaveChanges();
                return RedirectToAction("ViewProperty");
        }
    
        public ActionResult PropertyEdit(int? Id)
        {
            Database1Entities5 dc = new Database1Entities5();
            string Emailid = User.Identity.Name;
            var acc = dc.RegisterCustomers.FirstOrDefault(s => s.Email.Equals(Emailid));
            var acc1 = dc.PropertyTables.Where(s => s.Id == Id).FirstOrDefault();
            Session["Name"] = Convert.ToString(acc.FirstName + "" + acc.LastName);
            Session["SEmail"] = Convert.ToString(acc.Email);
            Session["SPhone"] = Convert.ToString(acc.PhoneNumber);
            Session["P_Id"] = Convert.ToInt32(acc1.Prop_id);
            ViewBag.image = acc.ProfileImage;
            if (acc1 == null)
            {
                return HttpNotFound();
            }
            return View(acc1);
        }

        [HttpPost]
        public ActionResult PropertyEdit(PropertyTable Registers, HttpPostedFileBase image, HttpPostedFileBase[] images)
        {
            Database1Entities5 dc = new Database1Entities5();
            if (image != null)
            {
                Registers.PropertyImage = new byte[image.ContentLength];
                image.InputStream.Read(Registers.PropertyImage, 0, image.ContentLength);
            }
            if (ModelState.IsValid)
            {
                if (images != null)
                {
                    List<Property_Images> pi = dc.Property_Images.Where(a => a.Property_Id == Registers.Prop_id).ToList();
                    int v = 0;
                    foreach (var i in images)
                    {
                        BinaryReader binary = new BinaryReader(i.InputStream);
                        pi[v].Image = binary.ReadBytes((int)i.ContentLength);
                        //dc.Property_Images.Add(pi[a]);
                        v++;
                    }
                    dc.SaveChanges();
                }
                dc.Entry(Registers).State = EntityState.Modified;
                dc.SaveChanges();
                return RedirectToAction("ViewProperty");
            }
            return View(Registers);
        }
        [HttpPost]
        public ActionResult SavePropertyDetails(PropertyModel registerDetails, HttpPostedFileBase[] images, HttpPostedFileBase files)
        {
            if (ModelState.IsValid)
            {
                using (var databaseContext = new Database1Entities5())
                {
                    PropertyTable reglog = new PropertyTable();
                    var userWithSameEmail = databaseContext.PropertyTables.Where(m => m.SellerName == "Izas").FirstOrDefault();

                    if (userWithSameEmail == null )
                    {
                        reglog.PropertyName = registerDetails.PropertyName;
                        reglog.PropertyType = registerDetails.PropertyType;
                        reglog.Status = registerDetails.Status;
                        reglog.Rent = registerDetails.Rent;
                        reglog.PropertySize = registerDetails.PropertySize;
                        reglog.PropertyAmenities = registerDetails.PropertyAmenities;
                        reglog.PropertySpecifications = registerDetails.PropertySpecifications;
                        reglog.PropertyRooms = registerDetails.PropertyRooms;
                        reglog.Propertyblacony = registerDetails.Propertyblacony;
                        reglog.PropertyBathrooms = registerDetails.PropertyBathrooms;
                        reglog.PropertyAddress = registerDetails.PropertyAddress;
                        reglog.PropertyDescription = registerDetails.PropertyDescription;
                        reglog.SellerName = registerDetails.SellerName;
                        reglog.SEmail = registerDetails.SEmail;
                        reglog.SPhoneno = registerDetails.SPhoneno;
                        reglog.SImage = registerDetails.SImage;
                        int iii = (int)databaseContext.PropertyTables.Max(a => a.Prop_id);
                        //int iii = 0;
                        reglog.Prop_id = iii + 1;

                        if (files != null)
                        {
                            reglog.PropertyImage = new byte[files.ContentLength];
                            files.InputStream.Read(reglog.PropertyImage, 0, files.ContentLength);

                        }
                        if (ModelState.IsValid)
                        {
                            int id = (int)databaseContext.PropertyTables.Max(a => a.Prop_id);
                            //int id = 0;
                            id += 1;
                            if (images != null)
                            {
                                foreach (var image in images)
                                {
                                    BinaryReader binary = new BinaryReader(image.InputStream);
                                    Property_Images pi = new Property_Images
                                    {
                                        Property_Id = id,
                                        Image = binary.ReadBytes((int)image.ContentLength)
                                    };
                                    databaseContext.Property_Images.Add(pi);
                                }
                                databaseContext.SaveChanges();
                            }
                        }

                        try
                        {
                            databaseContext.PropertyTables.Add(reglog);
                            databaseContext.SaveChanges();


                            ViewBag.saved = "Registration Successful..!";
                            return RedirectToAction("AIndex");
                        }
                        catch (NullReferenceException ex)
                        {
                            ViewBag.Message = "Something Wrong Please Try Again";
                            return View("AddProperty");
                        }
                    
                    }
                    else { 

                        ViewBag.Message = "Email Already Exists";

                        return View("AddProperty");
                    }
                }
            }
            else
            {
                return View("AddProperty", registerDetails);
            }
        }
        public ActionResult SaveBookedDetails([Bind(Include = "Id,PropertyName,PropertyType,Status,Rent,PropertySize,PropertyAmenities,PropertySpecifications,PropertyRooms,Propertyblacony,PropertyBathrooms,PropertyAddress,PropertyDescription,PropertyImage,HouseImages,SellerName,SEmail,SPhoneno,SImage,HouseImage2,HouseImage3,HouseImage4,CustomerName,BookStatus")] PropertyTable Registers, HttpPostedFileBase image,string book)
        {
            Database1Entities5 dc = new Database1Entities5();
            int PropId = @Convert.ToInt32(Session["PropId"]);
            var reglog = dc.BookStatus.FirstOrDefault(s => s.Prop_Id.Equals(PropId));
            var reglog1 = dc.PropertyTables.FirstOrDefault(s => s.Prop_id == PropId);
            string mail = reglog1.CustomerName;
            Session["SPhoneno"] = Convert.ToString(reglog1.SPhoneno);
            var reg = dc.RegisterCustomers.FirstOrDefault(s => s.Email == reglog1.SEmail);
            int i;
            if (reg.BookCount == null)
            {
                i = 0 + 1;
            }
            else { 
                i = (int)reg.BookCount+1;
            }
            if (book == "ACCEPT")
            {
                reglog.BookStatus = "Accepted";
                reglog1.BookStatus = "Accepted";
                reglog1.CustomerName = reglog1.CustomerName;
                reg.BookCount = i;
                dc.SaveChanges();
                //string resetCode = Guid.NewGuid().ToString();
                SendVerificationLinkEmail(mail,"Accepted");
                dc.Configuration.ValidateOnSaveEnabled = false;
                dc.SaveChanges();
                return RedirectToAction("BookedList");
            }
            else if(book == "REJECT")
            {
                reglog.BookStatus = "Cancelled";
                reglog1.BookStatus = null;
                reglog1.CustomerName = null;
                reglog1.BookTime = null;
                dc.SaveChanges();
                SendVerificationLinkEmail(mail, "Cancelled");
                dc.Configuration.ValidateOnSaveEnabled = false;
                dc.SaveChanges();
                return RedirectToAction("AllRequestedDetails");
            }
            return View();
            
        }
        [NonAction]
        public void SendVerificationLinkEmail(string emailID,string emailFor )
        {
            var fromEmail = new MailAddress("jaisriravi4527@gmail.com", "support@HouseRentSystem");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "jaisriemail82199";

            string subject = "";
            string body = "";
            if (emailFor == "Accepted")
            {
                subject = "Success! Your HouseRent Booking is Confirmed";
                body = "<!DOCTYPE html><html lang='en' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'><head><meta charset='utf-8'><meta name='viewport' content='width=device-width'><meta http-equiv='X-UA-Compatible' content='IE=edge'><meta name='x-apple-disable-message-reformatting'><title></title><link href='https://fonts.googleapis.com/css?family=Poppins:200,300,400,500,600,700' rel='stylesheet'><style>html,body{margin: 0 auto !important;padding: 0 !important;height: 100% !important;width: 100% !important;background: #f1f1f1;}* {-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;}div[style*='margin: 16px 0'] {margin: 0 !important;}table,td {mso-table-lspace: 0pt !important;mso-table-rspace: 0pt !important;}table {border-spacing: 0 !important;border-collapse: collapse !important;table-layout: fixed !important;margin: 0 auto !important;}img {-ms-interpolation-mode:bicubic;}a {text-decoration: none;}*[x-apple-data-detectors],.unstyle-auto-detected-links *,.aBn {border-bottom: 0 !important;cursor: default !important;color: inherit !important;text-decoration: none !important;font-size: inherit !important;font-family: inherit !important;font-weight: inherit !important;line-height: inherit !important;}.a6S {display: none !important;opacity: 0.01 !important;}.im {color: inherit !important;}img.g-img + div {display: none !important;}@media only screen and (min-device-width: 320px) and (max-device-width: 374px) {u ~ div .email-container {min-width: 320px !important;}}@media only screen and (min-device-width: 375px) and (max-device-width: 413px) {u ~ div .email-container {min-width: 375px !important;}}@media only screen and (min-device-width: 414px) {u ~ div .email-container {min-width: 414px !important;}}</style><style>.primary{background: #17bebb;}.bg_white{background: #ffffff;}.bg_light{background: #f7fafa;}.bg_black{background: #000000;}.bg_dark{background: rgba(0,0,0,.8);}.email-section{padding:2.5em;}.btn{padding: 10px 15px;display: inline-block;}.btn.btn-primary{border-radius: 5px;background: #17bebb;color: #ffffff;}.btn.btn-white{border-radius: 5px;background: #ffffff;color: #000000;}.btn.btn-white-outline{border-radius: 5px;background: transparent;border: 1px solid #fff;color: #fff;}.btn.btn-black-outline{border-radius: 0px;background: transparent;border: 2px solid #000;color: #000;font-weight: 700;}.btn-custom{color: rgba(0,0,0,.3);text-decoration: underline;}h1,h2,h3,h4,h5,h6{font-family: 'Poppins', sans-serif;color: #000000;margin-top: 0;font-weight: 400;}body{font-family: 'Poppins', sans-serif;font-weight: 400;font-size: 15px;line-height: 1.8;color: rgba(0,0,0,.4);}a{color: #17bebb;}table{}.logo h1{margin: 0;}.logo h1 a{color: #17bebb;font-size: 24px;font-weight: 700;font-family: 'Poppins', sans-serif;}.hero{position: relative;z-index: 0;}.hero .text{color: rgba(0,0,0,.3);}.hero .text h2{color: #000;font-size: 34px;margin-bottom: 0;font-weight: 200;line-height: 1.4;}.hero .text h3{font-size: 24px;font-weight: 300;}.hero .text h2 span{font-weight: 600;color: #000;}.text-author{border: 1px solid rgba(0,0,0,.05);max-width: 50%;margin: 0 auto;padding: 2em;}.text-author img{border-radius: 50%;padding-bottom: 20px;}.text-author h3{margin-bottom: 0;}ul.social{padding: 0;}ul.social li{display: inline-block;margin-right: 10px;}.footer{border-top: 1px solid rgba(0,0,0,.05);color: rgba(0,0,0,.5);}.footer .heading{color: #000;font-size: 20px;}.footer ul{margin: 0;padding: 0;}.footer ul li{list-style: none;margin-bottom: 10px;}.footer ul li a{color: rgba(0,0,0,1);}.hov-link:hover {color: #17bebb;}@media screen and (max-width: 500px) {}</style></head><body width='100%' style='margin: 0; padding: 0 !important; mso-line-height-rule: exactly; background-color: #f1f1f1;'><center style='width: 100%; background-color: #f1f1f1;'><div style='display: none; font-size: 1px;max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden; mso-hide: all; font-family: sans-serif;'>&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;</div><div style='max-width: 600px; margin: 0 auto;' class='email-container'><table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='margin: auto;'><tr><td valign='top' class='bg_white' style='padding: 1em 2.5em 0 2.5em;'><table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'><tr><td class='logo' style='text-align: center;'><h1><a href='#'>Alert Message</a></h1></td></tr></table></td></tr><tr><td valign='middle' class='hero bg_white' style='padding: 2em 0 4em 0;'><table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'><tr><td style='padding: 0 2.5em; text-align: center; padding-bottom: 3em;'><div class='text'><h3>Thank You For your Booking We are pleased to inform you that your booking request is SUCCESSFULLY CONFIRMED by seller.For your more References see Booked list on your website or any further information needed call via </h3><h3>Seller Contact No:</h3><h3 style='color:Red;font-size:bold;'>" + Convert.ToString(Session["SPhoneno"])+ "</h3></div></td></tr></body></html>";

            }
            else if(emailFor== "Cancelled")
            {
                subject = "Sorry! Your HouseRent Booking is Cancelled";
                body = "<!DOCTYPE html><html lang='en' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'><head><meta charset='utf-8'><meta name='viewport' content='width=device-width'><meta http-equiv='X-UA-Compatible' content='IE=edge'><meta name='x-apple-disable-message-reformatting'><title></title><link href='https://fonts.googleapis.com/css?family=Poppins:200,300,400,500,600,700' rel='stylesheet'><style>html,body{margin: 0 auto !important;padding: 0 !important;height: 100% !important;width: 100% !important;background: #f1f1f1;}* {-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;}div[style*='margin: 16px 0'] {margin: 0 !important;}table,td {mso-table-lspace: 0pt !important;mso-table-rspace: 0pt !important;}table {border-spacing: 0 !important;border-collapse: collapse !important;table-layout: fixed !important;margin: 0 auto !important;}img {-ms-interpolation-mode:bicubic;}a {text-decoration: none;}*[x-apple-data-detectors],.unstyle-auto-detected-links *,.aBn {border-bottom: 0 !important;cursor: default !important;color: inherit !important;text-decoration: none !important;font-size: inherit !important;font-family: inherit !important;font-weight: inherit !important;line-height: inherit !important;}.a6S {display: none !important;opacity: 0.01 !important;}.im {color: inherit !important;}img.g-img + div {display: none !important;}@media only screen and (min-device-width: 320px) and (max-device-width: 374px) {u ~ div .email-container {min-width: 320px !important;}}@media only screen and (min-device-width: 375px) and (max-device-width: 413px) {u ~ div .email-container {min-width: 375px !important;}}@media only screen and (min-device-width: 414px) {u ~ div .email-container {min-width: 414px !important;}}</style><style>.primary{background: #17bebb;}.bg_white{background: #ffffff;}.bg_light{background: #f7fafa;}.bg_black{background: #000000;}.bg_dark{background: rgba(0,0,0,.8);}.email-section{padding:2.5em;}.btn{padding: 10px 15px;display: inline-block;}.btn.btn-primary{border-radius: 5px;background: #17bebb;color: #ffffff;}.btn.btn-white{border-radius: 5px;background: #ffffff;color: #000000;}.btn.btn-white-outline{border-radius: 5px;background: transparent;border: 1px solid #fff;color: #fff;}.btn.btn-black-outline{border-radius: 0px;background: transparent;border: 2px solid #000;color: #000;font-weight: 700;}.btn-custom{color: rgba(0,0,0,.3);text-decoration: underline;}h1,h2,h3,h4,h5,h6{font-family: 'Poppins', sans-serif;color: #000000;margin-top: 0;font-weight: 400;}body{font-family: 'Poppins', sans-serif;font-weight: 400;font-size: 15px;line-height: 1.8;color: rgba(0,0,0,.4);}a{color: #17bebb;}table{}.logo h1{margin: 0;}.logo h1 a{color: #17bebb;font-size: 24px;font-weight: 700;font-family: 'Poppins', sans-serif;}.hero{position: relative;z-index: 0;}.hero .text{color: rgba(0,0,0,.3);}.hero .text h2{color: #000;font-size: 34px;margin-bottom: 0;font-weight: 200;line-height: 1.4;}.hero .text h3{font-size: 24px;font-weight: 300;}.hero .text h2 span{font-weight: 600;color: #000;}.text-author{border: 1px solid rgba(0,0,0,.05);max-width: 50%;margin: 0 auto;padding: 2em;}.text-author img{border-radius: 50%;padding-bottom: 20px;}.text-author h3{margin-bottom: 0;}ul.social{padding: 0;}ul.social li{display: inline-block;margin-right: 10px;}.footer{border-top: 1px solid rgba(0,0,0,.05);color: rgba(0,0,0,.5);}.footer .heading{color: #000;font-size: 20px;}.footer ul{margin: 0;padding: 0;}.footer ul li{list-style: none;margin-bottom: 10px;}.footer ul li a{color: rgba(0,0,0,1);}.hov-link:hover {color: #17bebb;}@media screen and (max-width: 500px) {}</style></head><body width='100%' style='margin: 0; padding: 0 !important; mso-line-height-rule: exactly; background-color: #f1f1f1;'><center style='width: 100%; background-color: #f1f1f1;'><div style='display: none; font-size: 1px;max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden; mso-hide: all; font-family: sans-serif;'>&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;</div><div style='max-width: 600px; margin: 0 auto;' class='email-container'><table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='margin: auto;'><tr><td valign='top' class='bg_white' style='padding: 1em 2.5em 0 2.5em;'><table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'><tr><td class='logo' style='text-align: center;'><h1><a href='#' style='color:red'>Alert Message</a></h1></td></tr></table></td></tr><tr><td valign='middle' class='hero bg_white' style='padding: 2em 0 4em 0;'><table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'><tr><td style='padding: 0 2.5em; text-align: center; padding-bottom: 3em;'><div class='text'><h3>Sorry ! For your Booking We are inform you that your booking request is CANCELLED by seller.For any further information needed call via </h3><h3>Seller Contact No:</h3><h3 style='color:Red;font-size:bold;'>" + Convert.ToString(Session["SPhoneno"]) + "</h3></div></td></tr></body></html>";
            }
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        public ActionResult UserData()
        {
            Database1Entities5 dc = new Database1Entities5();
            string Emailid = User.Identity.Name;
            var acc = dc.RegisterCustomers.FirstOrDefault(s => s.Email.Equals(Emailid));
            List<RegisterCustomer> register = new List<RegisterCustomer>();
            register.Add(acc);
            Session["Name"] = Convert.ToString(acc.FirstName+"" +acc.LastName);
            ViewBag.image = acc.ProfileImage;

            return View(register.AsEnumerable());
        }
        public ActionResult Edit(int? Id)
        {
            Database1Entities5 dc = new Database1Entities5();
            var acc = dc.RegisterCustomers.Where(s => s.Id == Id).FirstOrDefault();

            if (acc == null)
            {
                return HttpNotFound();
            }
            return View(acc);
        }
        [HttpPost]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Email,Password,PhoneNumber,Aadharno,ResetPasswordCode")] RegisterCustomer Registers, HttpPostedFileBase image)
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

        public ActionResult BookingDetails(int? Id)
        {
            Database1Entities5 dc = new Database1Entities5();
            string Emailid = User.Identity.Name;
            var acc1 = dc.RegisterCustomers.FirstOrDefault(s => s.Email.Equals(Emailid));
            ViewBag.image = acc1.ProfileImage;
            Session["PropId"] = Convert.ToString(Id);
            var tables = new AllTable
            {
                RegisterUser = dc.RegisterUsers.ToList(),
                Property_Images=dc.Property_Images.Where(m=>m.Property_Id==Id).ToList(),
                PropertyTable = dc.PropertyTables.Where(m => m.Prop_id == Id).ToList()
        };
            return View(tables);
        }
        public ActionResult BookedList()
        {
            Database1Entities5 dc = new Database1Entities5();
            string Emailid = User.Identity.Name;
            var acc1 = dc.RegisterCustomers.FirstOrDefault(s => s.Email.Equals(Emailid));
            ViewBag.image = acc1.ProfileImage;
            var acc = dc.PropertyTables.FirstOrDefault(s => s.SEmail == Emailid);
            var tables = new AllTable
            {
                RegisterUser = dc.RegisterUsers.Where(m => m.Email == acc.CustomerName).ToList(),
                PropertyTable = dc.PropertyTables.Where(m => m.BookStatus == "Accepted" && m.CustomerName == acc.CustomerName && m.SEmail == Emailid).ToList()
            };
            return View(tables);
        }
        public ActionResult AllRequestedDetails()
        {
            Database1Entities5 dc = new Database1Entities5();
            string Emailid = User.Identity.Name;
            var acc1 = dc.RegisterCustomers.FirstOrDefault(s => s.Email.Equals(Emailid));
            ViewBag.image = acc1.ProfileImage;
            //var acc = dc.PropertyTables.Where(s => s.SEmail == Emailid).ToList();
            var tables = new AllTable
            {

                RegisterUser = dc.RegisterUsers.ToList(),
                PropertyTable = dc.PropertyTables.Where(m => m.BookStatus == "Waiting" && m.SEmail == Emailid).ToList()
            };
            return View(tables);
        }
    public ActionResult Logout()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Login","Account");
        }

    }
}