using System;
using System.Globalization;
using EdAppTest.Domain;

namespace EdAppTest.Features.Products
{
    public class ProductViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string AuctionEnd { get; set; }

        public ProductViewModel()
        { }

        public ProductViewModel(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            AuctionEnd = product.AuctionEnd.ToString("o", CultureInfo.InvariantCulture);
        }
    }
}