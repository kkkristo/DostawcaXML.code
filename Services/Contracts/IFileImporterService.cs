using DostawcaXML.Models;

namespace DostawcaXML.Services.Contracts
{
    public interface IFileImporterService
    {
        public IEnumerable<ProductDTO> ConvertFromSingleXml(string filePath);

        public IEnumerable<ProductDTO> PrepareDTOCollection();

        public IEnumerable<ProductDTO> PrepareDTOCollection(string filePath);

        public void MergeAndExportUploadedFiles();
    }
}
