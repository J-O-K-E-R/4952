﻿using _4952.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace _4952.Controllers
{
    public class MyAccountController : Controller
    {
        private byte[] salt = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        private int itterations = 57;

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User account)
        {
            if (ModelState.IsValid)
            {
                azureEntities db = new azureEntities();
                if(db.Users.Any(user => user.email == account.email))
                {
                    ViewBag.Message = "Account already exists";
                    return View();
                }
                if(account.email == null || account.password == null)
                {
                    ViewBag.Message = "Null Account";
                    return View();
                }
                string s = Encoding.Default.GetString(new Rfc2898DeriveBytes(account.password, salt, itterations).GetBytes(255));
                account.password = s;

                db.Users.Add(account);
                db.SaveChanges();
                
                ModelState.Clear();
                ViewBag.Message = "Successfully Registered";
                return RedirectToAction("Login");
            }
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserAccount user)
        {
            using (azureEntities db = new azureEntities())
            {
                if(user.email == null || user.password == null)
                { 
                    ModelState.AddModelError("", "Enter a username / Password");
                    return View();
                }
                string s = Encoding.Default.GetString(new Rfc2898DeriveBytes(user.password, salt, itterations).GetBytes(255));
                user.password = s;
                if(db.Users.All(u => u.email != user.email || u.password != user.password))
                {
                    ModelState.AddModelError("", "Username or Password is Wrong");
                    return View();
                }
                var usr = db.Users.Single(u => u.email == user.email && u.password == user.password);
                if (usr != null)
                {
                    Session["userID"] = usr.userID;
                    Session["email"] = usr.email.ToString();
                    Session["userID"] = usr.userID;
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session["userID"] = null;
            Session["email"] = null;
            return RedirectToAction("Login");
        }


        public ActionResult LoggedIn()
        {
            {
                if(Session["UserID"] != null)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
        }

    }
}