//Account Controller
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

namespace HouseRentManagementSystem.Controllers
{
    public class AccountController : Controller
    {

        //Return Register view
        public ActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/Account/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var fromEmail = new MailAddress("jaisriravi4527@gmail.com", "support@HouseRentSystem");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "jaisriemail82199";

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";
                body = "<br/><br/>We are excited to tell you that your House Rent account is" +
                    " successfully created. Please click on the below link to verify your account" +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Your Password";
                body = "Hi,<br/><br/>We have received a request to reset your HouseRent Management System account password associated with this email address. If you have not placed this request, you can safely ignore this email and we assure you that your account is completely secure." +
                    "If you do need to change your HouseRent Password, you can use the link given below." +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>";
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

        [HttpPost]
        public ActionResult ForgotPassword(string EmailID)
        {
            //Verify Email ID
            //Generate Reset password link
            //Send Email
            string message = "";
            bool status = false;

            using (Database1Entities5 dc = new Database1Entities5())
            {
                var account = dc.RegisterUsers.Where(a => a.Email == EmailID).FirstOrDefault();
                var account1 = dc.RegisterCustomers.Where(a => a.Email == EmailID).FirstOrDefault();
                if (account != null)
                {
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account.Email, resetCode, "ResetPassword");
                    account.ResetPasswordCode = resetCode;
                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();
                    message = "Reset password link has been sent to your email id.";
                }
                else if(account1!= null)
                {
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account1.Email, resetCode, "ResetPassword");
                    account.ResetPasswordCode = resetCode;
                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();
                    message = "Reset password link has been sent to your email id.";
                }
                else
                {
                    message = "Account not found";
                }
            }
            ViewBag.Message = message;
            return View();
        }



        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page

            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            using (Database1Entities5 dc = new Database1Entities5())
            {
                var user = dc.RegisterUsers.Where(a => a.ResetPasswordCode == id).SingleOrDefault();
                if (user != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetCode = id;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using (Database1Entities5 dc = new Database1Entities5())
                {
                    var user = dc.RegisterUsers.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if (user != null)
                    {
                        user.Password = model.NewPassword;
                        user.ResetPasswordCode = "";
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        dc.SaveChanges();
                        message = "New password updated successfully";
                    }
                }
            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View(model);
        }


        [HttpPost]
        public ActionResult SaveRegisterDetails(Register registerDetails,HttpPostedFileBase image)
        {
            //We check if the model state is valid or not. We have used DataAnnotation attributes.
            //If any form value fails the DataAnnotation validation the model state becomes invalid.
            if (ModelState.IsValid)
            {
                //create database context using Entity framework
                using (var databaseContext = new Database1Entities5())
                {
                    RegisterUser reglog = new RegisterUser();
                    var userWithSameEmail = databaseContext.RegisterUsers.Where(m => m.Email == registerDetails.Email).FirstOrDefault(); 
                    if (userWithSameEmail == null)
                    {
                        reglog.FirstName = registerDetails.FirstName;
                        reglog.LastName = registerDetails.LastName;
                        reglog.Email = registerDetails.Email;
                        reglog.PhoneNumber = registerDetails.PhoneNumber;
                        reglog.Password = registerDetails.Password;
                        reglog.Aadharno = registerDetails.Aadharno;
                        if (image != null)
                        {
                            reglog.ProfileImage = new byte[image.ContentLength];
                            image.InputStream.Read(reglog.ProfileImage, 0, image.ContentLength);
                        }

                        try
                        {
                            databaseContext.RegisterUsers.Add(reglog);
                            databaseContext.SaveChanges();

                            TempData["Message"] = "Registration Is Success ! Use your username and password here to Login..";
                            return RedirectToAction("Login");
                        }
                        catch (NullReferenceException ex)
                        {
                            ViewBag.Message = "Something Wrong Please Try Again";
                            return View("Register");
                        }

                        TempData["ErrorMessage"] = "Registration Is Success";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = "Email Already Exists";
                        return View("Register");
                    }
                }

            }
            else
            {

                //If the validation fails, we are returning the model object with errors to the view, which will display the error messages.
                return View("Register", registerDetails);
            }
        }


        [HttpPost]
        public ActionResult SaveRegisterUser(Register registerDetails, HttpPostedFileBase image)
        {
            //We check if the model state is valid or not. We have used DataAnnotation attributes.
            //If any form value fails the DataAnnotation validation the model state becomes invalid.
            if (ModelState.IsValid)
            {
                //create database context using Entity framework
                using (var databaseContext = new Database1Entities5())
                {

                    //If the model state is valid i.e. the form values passed the validation then we are storing the User's details in DB.
                    RegisterCustomer reglog = new RegisterCustomer();
                    var userWithSameEmail = databaseContext.RegisterCustomers.Where(m => m.Email == registerDetails.Email).FirstOrDefault(); //checking if the emailid already exits for any user
                    //var user = databaseContext.RegisterUsers.Where(query => query.Email.Equals(registerDetails.Email)).SingleOrDefault();
                    if (userWithSameEmail == null)
                    {
                        reglog.FirstName = registerDetails.FirstName;
                        reglog.LastName = registerDetails.LastName;
                        reglog.Email = registerDetails.Email;
                        reglog.PhoneNumber = registerDetails.PhoneNumber;
                        reglog.Password = registerDetails.Password;
                        reglog.Aadharno = registerDetails.Aadharno;
                        if (image != null)
                        {
                            reglog.ProfileImage = new byte[image.ContentLength];
                            image.InputStream.Read(reglog.ProfileImage, 0, image.ContentLength);
                        }

                        try
                        {
                            databaseContext.RegisterCustomers.Add(reglog);
                            databaseContext.SaveChanges();
                            TempData["Message1"] = "Your Registration Is Success ! Use your username and password here to Login..";
                            return RedirectToAction("Login");
                        }
                        catch (NullReferenceException ex)
                        {
                            ViewBag.Message = "Something Wrong Please Try Again";
                            return View("Register");
                        }
                    }
                    else
                    {
                        ViewBag.Message1 = "Email Already Exists";
                        return View("Register");
                    }
                }

            }
            else
            {

                //If the validation fails, we are returning the model object with errors to the view, which will display the error messages.
                return View("Register", registerDetails);
            }
        }




        public ActionResult Login()
        {
            return View();
        }

        //The login form is posted to this method.
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            //Checking the state of model passed as parameter.
            if (ModelState.IsValid)
            {
                //Validating the user, whether the user is valid or not.
                var isValidUser = IsValidUser(model);
                 if (isValidUser != null)
                {
                    System.Web.Security.FormsAuthentication.SetAuthCookie(model.Email, false);
                    return RedirectToAction("UIndex", "User");
                }
                else
                {
                    //If the username and password combination is not present in DB then error message is shown.
                    TempData["Login"] = "Wrong Username and password combination !";
                    return RedirectToAction("Login");
                }
            }
            else
            {
                //If model state is not valid, the model with error message is returned to the View.
                return View(model);
            }
        }

        //function to check if User is valid or not
        public RegisterUser IsValidUser(LoginViewModel model)
        {
            using (var dataContext = new Database1Entities5())
            {
                //Retireving the user details from DB based on username and password enetered by user.
                RegisterUser user = dataContext.RegisterUsers.Where(query => query.Email.Equals(model.Email) && query.Password.Equals(model.Password)).SingleOrDefault();
                //If user is present, then true is returned.
                if (user == null)
                    return null;
                //If user is not present false is returned.
                else
                    return user;
            }
        }

        [HttpPost]
        public ActionResult Login1(LoginViewModel model)
        {
            //Checking the state of model passed as parameter.
            if (ModelState.IsValid)
            {

                //Validating the user, whether the user is valid or not.
                var isValidUser = IsValidUser1(model);
                if (isValidUser != null)
                {
                    System.Web.Security.FormsAuthentication.SetAuthCookie(model.Email, false);
                    return RedirectToAction("AIndex", "Admin");
                }
                else
                {
                    //If the username and password combination is not present in DB then error message is shown.
                    TempData["Login1"] = "Wrong Username and password combination !";
                    return RedirectToAction("Login");
                }
            }
            else
            {
                //If model state is not valid, the model with error message is returned to the View.
                return View(model);
            }
        }

        //function to check if User is valid or not
        public RegisterCustomer IsValidUser1(LoginViewModel model)
        {
            using (var dataContext = new Database1Entities5())
            {
                //Retireving the user details from DB based on username and password enetered by user.
                RegisterCustomer user = dataContext.RegisterCustomers.Where(query => query.Email.Equals(model.Email) && query.Password.Equals(model.Password)).SingleOrDefault();
                //If user is present, then true is returned.
                if (user == null)
                    return null;
                //If user is not present false is returned.
                else
                    return user;
            }
        }

        public ActionResult Logout()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Login", "Account");
        }




    }
}



