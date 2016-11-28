﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _4952.Models;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Data.Entity.Validation;

namespace _4952.Controllers
{
    public class HomeController : Controller
    {
        azureEntities db = new azureEntities();
        static int fixedUserIDForTestingPurposesOnly = 2;

        public ActionResult Index(string searchString)
        {
            var model = new FileViewModel();
            model.fileMetadataList = (from file in db.Files
                                      where file.userID == fixedUserIDForTestingPurposesOnly
                                      select new FileMetadata()
                                      {
                                          fileID = file.FileID,
                                          fileName = file.fileName,
                                          fileSize = file.fileSize,
                                          fileDateCreated = file.fileDateCreated,
                                      }).ToList();

            if (!String.IsNullOrEmpty(searchString))
                model.fileMetadataList = model.fileMetadataList.Where(s => s.fileName.Contains(searchString)).ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult Search(string searchString)
        {
            Debug.Write("Works");
            var model = new FileViewModel();
            model.fileMetadataList = (from file in db.Files
                                      where file.userID == fixedUserIDForTestingPurposesOnly
                                      && file.fileName.Contains(searchString)
                                      select new FileMetadata()
                                      {
                                          fileID = file.FileID,
                                          fileName = file.fileName,
                                          fileSize = file.fileSize,
                                          fileDateCreated = file.fileDateCreated,
                                      }).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index()
        {
            byte[] fileBytes = Encoding.UTF8.GetBytes(Request["fileData"]);
            db.Files.Add(new Models.File()
            {
                userID = fixedUserIDForTestingPurposesOnly,
                data = fileBytes,
                fileName = Request["fileName"],
                fileSize = fileBytes.Length,
                fileDateCreated = DateTime.Now
            });
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult FileSelected(string submitButton)
        {
            int selectedFile;
            if (int.TryParse(Request.Form["rGroup"], out selectedFile))
            {
                switch (submitButton)
                {
                    case "Download":
                        return DownloadFile(selectedFile);
                    case "Delete":
                        return DeleteFile(selectedFile);
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult DownloadFile(int id)
        {
            Models.File file = db.Files.Find(id);
            if (file != null && file.userID == fixedUserIDForTestingPurposesOnly)
            {
                return File(file.data, System.Net.Mime.MediaTypeNames.Application.Octet, file.fileName.Trim());
            }
            return RedirectToAction("Index");
        }

        public ActionResult DeleteFile(int id)
        {
            Models.File file = db.Files.Find(id);
            if (file != null && file.userID == fixedUserIDForTestingPurposesOnly)
            {
                db.Files.Remove(file);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult fileBox()
        {
            return PartialView();
        }
    }
}