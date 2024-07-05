using DostawcaXML.Models;
using DostawcaXML.Statics;
using System.Xml;

namespace DostawcaXML.Services.Contracts
{
    public interface IFileManagerService
    {
        public void ExportToFile(List<ProductDTO> productsToExp);
        public void DeleteFile(string filePath);
    }
}
