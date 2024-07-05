using DostawcaXML.Models;
using DostawcaXML.Services.Contracts;
using DostawcaXML.Statics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace DostawcaXML.Controllers
{
    public class HomeController : Controller
    {
        private string uploadsFolderPath = SD.uploadsFolderPath;
        private string exportsFolderPath = SD.exportsFolderPath;
        private IFileManagerService _fileManagerService;
        private IFileImporterService _fileImporterService;

        public HomeController(IFileManagerService fileManager,IFileImporterService fileImporter)
        {
            _fileManagerService = fileManager;
            _fileImporterService = fileImporter;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region UPLOAD FILE from user

        [HttpGet]
        public IActionResult Import()
        {
            var files = Directory.EnumerateFiles(uploadsFolderPath);
            List<string> fileNames = new();
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                fileNames.Add(fileName);
            }

            ViewBag.UploadedFiles = fileNames;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            string filePath;
            if (file != null && file.Length > 0)
            {
                try
                {                    
                    var uploadedfileName = Path.GetFileName(file.FileName);
                    var uploadedExtension = Path.GetExtension(uploadedfileName);

                    if (uploadedExtension == ".xml")
                    {
                        filePath = Path.Combine(uploadsFolderPath, uploadedfileName);

                        if (!Directory.Exists(uploadsFolderPath))
                        { Directory.CreateDirectory(uploadsFolderPath); }

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                    }
                    else
                    {
                        throw new Exception("File must be \".xml\" type");
                    }

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home", new { message = ex.Message });
                }
            }
            else
            {
                return RedirectToAction("Error", "Home", new { message = "File not attached or empty." });
            }

            return RedirectToAction("Import", "Home");
        }

        #endregion

        public IActionResult DisplayUploaded(bool? afterChange)
        {
            if (afterChange == null || (bool)!afterChange)
            {
                _fileImporterService.MergeAndExportUploadedFiles();
            }
            var mergedXml = exportsFolderPath + "\\" + SD.exportedFileName;

            var resultsDTO = _fileImporterService.ConvertFromSingleXml(mergedXml);

            return View(resultsDTO);
        }

        public IActionResult RemoveFile(string fileName)
        {
            var filePath = uploadsFolderPath + "\\" + fileName;
            _fileManagerService.DeleteFile(filePath);

            return RedirectToAction("Import");
        }

        public IActionResult DownloadMerged()
        {
            string filePath = exportsFolderPath + "\\" + SD.exportedFileName;
            FileStream stream = new FileStream(filePath, FileMode.Open);
            return File(stream, "application/octet-stream", SD.exportedFileName);
        }

        public IActionResult DeleteEntryFromDisplay(int index)
        {
            var collection = _fileImporterService.PrepareDTOCollection(exportsFolderPath).ToList();
            collection.RemoveAt(index);
            _fileManagerService.ExportToFile(collection);

            return RedirectToAction("DisplayUploaded", new {afterChange = true}); ;
        }

        public IActionResult ChangeAcceptation(int index)
        {
            var collection = _fileImporterService.PrepareDTOCollection(exportsFolderPath).ToList();
            var product = collection[index];
            product.Accepted = !product.Accepted;
            _fileManagerService.ExportToFile(collection);
            return RedirectToAction("DisplayUploaded", new { afterChange = true }); 
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string? message)
        {

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Message = !string.IsNullOrEmpty(message) ? message : ""
            });
        }
       
    }
}