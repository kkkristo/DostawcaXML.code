using DostawcaXML.Models;
using Microsoft.AspNetCore.Http;

namespace DostawcaXML.Statics
{
    public static class SD
    {
        public const string uploadsFolder = "Uploads";
        public const string exportsFolder = "Exports";
        public const string exportedFileName = "dostawcy.xml";

        public static string uploadsFolderPath = Directory.GetCurrentDirectory() + "\\" + uploadsFolder;
        public static string exportsFolderPath = Directory.GetCurrentDirectory() + "\\" + SD.exportsFolder;

        public enum modelXml
        {
            Offered,
            Product,
            SourceFileName,
            Id,
            Name,
            Size,
            Categories,
            Category, 
            Description, 
            Quantity,
            Images,
            Image,
            Accepted
        }
    }

}
