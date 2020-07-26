using System;
using System.Collections.Generic;
using System.Linq;
using EdAppTest.Domain;

namespace EdAppTest.Database
{
    public interface IDatabaseAdapter
    {
        void UpsertUser(User user);

        User GetUser(Guid id);

        User GetUserByUserName(string userName);

        void AddProduct(Product product);

        Product GetProduct(Guid id);

        void AddBid(Bid bid);

        (Product Product, Bid bid) GetProductBid(Guid productId);

        IEnumerable<(Product Product, Bid bid)> GetProductBids(Guid? productId);
    }

    public class DatabaseAdapter : IDatabaseAdapter
    {
        private readonly Dictionary<Guid, User> users = new Dictionary<Guid, User>();
        private readonly  Dictionary<Guid, Product> products = new Dictionary<Guid, Product>();
        private readonly  Dictionary<Guid, Bid> bids = new Dictionary<Guid, Bid>();

        public void UpsertUser(User user)
        {
            var entity = GetUser(user.Id);

            if (entity == null)
            {
                users.Add(user.Id, user);
            }
            else
            {
                entity.RefreshToken = user.RefreshToken;
                entity.RefreshTokenExpiresAt = user.RefreshTokenExpiresAt;
            }
        }

        public User GetUser(Guid id)
        {
            users.TryGetValue(id, out User user);
            return user;
        }

        public User GetUserByUserName(string userName)
        {
            var item = users.FirstOrDefault(item =>
                string.Equals(item.Value.UserName, userName, StringComparison.Ordinal));

            return item.Value;
        }

        public void AddProduct(Product product)
        {
            products.Add(product.Id, product);
        }

        public Product GetProduct(Guid id)
        {
            products.TryGetValue(id, out Product product);
            return product;
        }
    
        public void AddBid(Bid bid)
        {
            bids.Add(bid.Id, bid);
        }

        public (Product Product, Bid bid) GetProductBid(Guid productId)
        {
            var product = GetProduct(productId);
            
            if(product == null)
            {
                return (null, null);
            }
            
            var bid =  bids.Values
                .Where(item => item.ProductId == product.Id)
                .OrderByDescending(item => item.Price)
                .FirstOrDefault();

            return (product, bid);
        }

        public IEnumerable<(Product Product, Bid bid)> GetProductBids(Guid? productId)
        {
            var join = from product in products.Values
                       join bid in bids.Values on product.Id equals bid.ProductId into bidJoin
                       from bid in bidJoin.DefaultIfEmpty()
                       select new { Product = product, Bid = bid };

            if(productId.HasValue) {
                join = join.Where(item => item.Product.Id == productId.Value);
            }

            return join
                .GroupBy(item => item.Product.Id)
                .Select(item => {
                    var product = item.First().Product;
                    var bid =
                        item
                        .Select(item => item.Bid)
                        .Where(item => item != null)
                        .OrderByDescending(item => item.Price)
                        .FirstOrDefault();

                    return (product, bid);
                })
                .ToArray();
        }
    }
}
