using AgriculturalWebsite.Models;

namespace AgriculturalWebsite.Models
{
    public class SearchViewModel
    {
        public string Query { get; set; } = string.Empty;
        public IEnumerable<Tractor> Tractors { get; set; } = new List<Tractor>();
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public IEnumerable<Trailer> Trailers { get; set; } = new List<Trailer>();
    }
}