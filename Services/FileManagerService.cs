using DostawcaXML.Models;
using DostawcaXML.Services.Contracts;
using DostawcaXML.Statics;
using System.Xml;

namespace DostawcaXML.Services
{
    public class FileManagerService : IFileManagerService
    {
        public void ExportToFile(List<ProductDTO> productsToExp)
        {
            var exportedFiles = Directory.EnumerateFiles(SD.exportsFolderPath);
            List<ProductDTO> productsToExport = productsToExp;
            foreach (var file in exportedFiles)
            {
                DeleteFile(file);
            }

            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = ("    "),
                Encoding = System.Text.Encoding.UTF8,
                OmitXmlDeclaration = false,
                CloseOutput = true
            };

            using (XmlWriter writer = XmlWriter.Create($"{SD.exportsFolderPath}\\{SD.exportedFileName}", settings))
            {
                writer.WriteStartDocument();
                writer.WriteComment("Delivered by KrzySzko");
                writer.WriteStartElement("offered");
                foreach (var product in productsToExport)
                {
                    writer.WriteStartElement("product");
                    writer.WriteElementString("sourcefilename", product.SourceFileName);
                    writer.WriteElementString("id", product.Id);
                    writer.WriteElementString("name", product.Name);
                    writer.WriteElementString("size", product.Size);
                    writer.WriteStartElement("categories");
                    if (product.Categories.Count > 0)
                    {
                        foreach (var category in product.Categories)
                        {
                            writer.WriteElementString("category", category);
                        }
                    }
                    writer.WriteEndElement();
                    writer.WriteElementString("description", product.Description);
                    writer.WriteElementString("quantity", product.Quantity);
                    writer.WriteStartElement("images");
                    if (product.Images.Count > 0)
                    {
                        foreach (var image in product.Images)
                        {
                            writer.WriteElementString("image", image);
                        }
                    }
                    writer.WriteEndElement();
                    writer.WriteElementString("accepted", product.Accepted.ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
            }
        }
        public void DeleteFile(string filePath)
        {
            System.IO.File.Delete(filePath);
        }
    }
}
