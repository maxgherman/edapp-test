using System;
using EdAppTest.Constants;
using EdAppTest.Database;
using EdAppTest.Domain;
using EdAppTest.Extensions;
using Microsoft.Extensions.Configuration;

namespace EdAppTest.Features.Products
{
    public interface IProductService
    {
        Product AddProduct(Product product);
    }

    public class ProductService: IProductService
    {
        private readonly IDatabaseAdapter databaseAdapter;
        private readonly AuctionOptions auctionOptions;

        public ProductService(
            IDatabaseAdapter databaseAdapter,
            IConfiguration configuration
            )
        {
            this.databaseAdapter = databaseAdapter;
            this.auctionOptions = configuration.ParseValue<AuctionOptions>("Auction");
        }

        public Product AddProduct(Product product)
        {
            product.Id = Guid.NewGuid();
            product.AuctionEnd = DateTime.UtcNow.AddMinutes(auctionOptions.DurationMinutes);
            databaseAdapter.AddProduct(product);

            return product;
        }
    }
}