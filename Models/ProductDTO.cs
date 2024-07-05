namespace DostawcaXML.Models
{
    public class ProductDTO
    {
        public string Id { get; set; }

        public string SourceFileName { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public List<string> Categories { get; set; }

        public List<string> Images {  get; set; }

        public string Size {  get; set; }

        public string Quantity { get; set; }

        public bool Accepted {  get; set; }

        public int? Index { get; set;}
    }
}
