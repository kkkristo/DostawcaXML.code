using DostawcaXML.Models;
using DostawcaXML.Services.Contracts;
using DostawcaXML.Statics;
using System.Xml.Linq;

namespace DostawcaXML.Services
{
    public class FileImporterService : IFileImporterService
    {
        private readonly IFileManagerService _fileManagerService;
        public FileImporterService(IFileManagerService fileManagerService)
        {
            _fileManagerService = fileManagerService;            
        }

        public IEnumerable<ProductDTO> ConvertFromSingleXml(string filePath)
        {
            string sourceFileName = Path.GetFileName(filePath);
            //XElement xElement = XElement.Load($"{SD.uploadsFolder}\\{sourceFileName}");
            XElement xElement = XElement.Load(filePath);

            string rootName = xElement.Name.ToString();
            var resultProducts = new List<ProductDTO>();
            List<XElement> products = new();

            switch (rootName.ToLower())
            {
                case "offer":  //dostawca 1, plik 1 lub plik 2

                    products = xElement.Descendants("product").ToList();
                    foreach (var product in products)
                    {
                        var result = new ProductDTO
                        {
                            Id = product.Attribute("id").Value,
                            SourceFileName = sourceFileName,
                            Description = string.Empty,
                            Categories = new(),
                            Accepted = false,
                            Size = product.Element("sizes").Element("size").Attribute("id").Value,
                            Images = new()
                        };
                        if (product.Descendants("name").Any())
                        {
                            var name = product.Element("description").Elements("name")
                                .FirstOrDefault(e => e.Attribute(XNamespace.Xml + "lang").Value == "pol").Value;
                            result.Name = name;
                        };
                        if (product.Descendants("image") != null)
                        {
                            foreach (var image in product.Descendants("image"))
                            {
                                var img = image.Attribute("url").Value;
                                result.Images.Add(img);
                            };
                        }

                        if (product.Descendants("description").Any())
                        {
                            var desc = product.Element("description").Elements("long_desc")
                                .FirstOrDefault(e => e.Attribute(XNamespace.Xml + "lang").Value == "pol").Value;

                            result.Description = desc;
                        };
                        resultProducts.Add(result);
                    }
                    return resultProducts;

                case "products":  //dostawca 2, plik 1 lub plik 2

                    products = xElement.Descendants("product").ToList();
                    foreach (var product in products)
                    {
                        var result = new ProductDTO
                        {
                            Id = product.Element("id").Value,
                            SourceFileName = sourceFileName,
                            Name = product.Element("name") != null ? product.Element("name").Value : string.Empty,
                            Description = string.Empty,
                            Accepted = false,
                            Size = string.Empty,
                            Quantity = product.Element("qty").Value,
                            Images = new(),
                            Categories = new()
                        };
                        if (product.Descendants("photo") != null)
                        {
                            foreach (var image in product.Descendants("photo"))
                            {
                                var img = image.Value;
                                result.Images.Add(img);
                            };
                        }

                        if (product.Descendants("desc").Any())
                        {
                            var desc = product.Element("desc").Value;

                            result.Description = desc;
                        };
                        if (product.Descendants("category") != null)
                        {
                            foreach (var cat in product.Descendants("category"))
                            {
                                var category = cat.Value;
                                result.Categories.Add(category);
                            };
                        }
                        resultProducts.Add(result);
                    }
                    return resultProducts;

                case "produkty":  //dostawca 3, plik 1

                    products = xElement.Descendants("produkt").ToList();
                    foreach (var product in products)
                    {
                        var result = new ProductDTO
                        {
                            Id = product.Element("id").Value,
                            SourceFileName = sourceFileName,
                            Name = product.Element("nazwa_pl") != null ? product.Element("nazwa_pl").Value : string.Empty,
                            Description = string.Empty,
                            Accepted = false,
                            Size = product.Element("rozmiar") != null ? product.Element("rozmiar").Value : string.Empty,
                            Quantity = string.Empty,
                            Images = new(),
                            Categories = new()
                        };
                        if (product.Descendants("zdjecie") != null)
                        {
                            foreach (var image in product.Descendants("zdjecie"))
                            {
                                var img = image.Attribute("url").Value;
                                result.Images.Add(img);
                            };
                        }

                        if (product.Element("dlugi_opis_pl") != null)
                        {
                            result.Description = product.Element("dlugi_opis_pl").Value;
                        };
                        if (product.Descendants("cat_pl") != null)
                        {
                            var category = product.Element("cat_pl").Value;
                            result.Categories.Add(category);
                        }

                        resultProducts.Add(result);
                    }

                    return resultProducts;

                case "offered":  //merged result file

                    products = xElement.Descendants("product").ToList();
                    foreach (var product in products)
                    {
                        var result = new ProductDTO
                        {
                            Id = product.Element(SD.modelXml.Id.ToString().ToLower()).Value,
                            SourceFileName = product.Element(SD.modelXml.SourceFileName.ToString().ToLower()).Value,
                            Name = product.Element(SD.modelXml.Name.ToString().ToLower()).Value,

                            Description = !product.Element(SD.modelXml.Description.ToString().ToLower()).IsEmpty ?
                            product.Element(SD.modelXml.Description.ToString().ToLower()).Value : string.Empty,

                            Accepted = product.Element(SD.modelXml.Accepted.ToString().ToLower()).Value.ToLower() == "true" ? true : false,

                            Size = !product.Element(SD.modelXml.Size.ToString().ToLower()).IsEmpty ?
                            product.Element(SD.modelXml.Size.ToString().ToLower()).Value : string.Empty,

                            Quantity = !product.Element(SD.modelXml.Quantity.ToString().ToLower()).IsEmpty ?
                            product.Element(SD.modelXml.Quantity.ToString().ToLower()).Value : string.Empty,

                            Images = new(),
                            Categories = new()
                        };

                        if (product.Descendants(SD.modelXml.Image.ToString().ToLower()).Any())
                        {
                            foreach (var image in product.Descendants(SD.modelXml.Image.ToString().ToLower()))
                            {
                                result.Images.Add(image.Value);
                            };
                        }

                        if (product.Descendants(SD.modelXml.Category.ToString().ToLower()).Any())
                        {
                            foreach (var cat in product.Descendants(SD.modelXml.Category.ToString().ToLower()))
                            {
                                result.Categories.Add(cat.Value);
                            }

                        }

                        resultProducts.Add(result);
                    }

                 return resultProducts;

                default: break;
            }

            return resultProducts;
        }

        public IEnumerable<ProductDTO> PrepareDTOCollection()
        {
            List<ProductDTO> products = new List<ProductDTO>();

            var enumeratedFolder = SD.uploadsFolderPath;

            var files = Directory.EnumerateFiles(enumeratedFolder).ToList();

            foreach (var file in files)
            {
                //var filePath = Path.Combine(uploadsFolderPath, file);
                List<ProductDTO> productsFromFile = ConvertFromSingleXml(file).ToList();
                products.AddRange(productsFromFile);
            }

            return products;
        }

        public IEnumerable<ProductDTO> PrepareDTOCollection(string filePath)
        {
            List<ProductDTO> products = new List<ProductDTO>();

            var enumeratedFolder = filePath != null ? filePath : SD.uploadsFolderPath;

            var files = Directory.EnumerateFiles(enumeratedFolder).ToList();

            foreach (var file in files)
            {                
                List<ProductDTO> productsFromFile = ConvertFromSingleXml(file).ToList();
                products.AddRange(productsFromFile);
            }

            return products;
        }
        
        public void MergeAndExportUploadedFiles()
        {
            var products = PrepareDTOCollection().ToList();
            _fileManagerService.ExportToFile(products);
        }


    }


}
