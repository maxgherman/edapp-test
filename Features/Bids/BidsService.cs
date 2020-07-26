using System;
using System.Collections.Generic;
using System.Linq;
using EdAppTest.Database;
using EdAppTest.Domain;
using EdAppTest.Exceptions;

namespace EdAppTest.Features.Bids
{
    public interface IBidsService
    {
        Bid AddBid(Bid bid);

        IEnumerable<Auction> GetAuctions(Guid? productId);
    }

    public class BidsService : IBidsService
    {
        private readonly IDatabaseAdapter databaseAdapter;
       
        public BidsService(IDatabaseAdapter databaseAdapter)
        {
            this.databaseAdapter = databaseAdapter;
        }

        public Bid AddBid(Bid bid)
        {
            var user = databaseAdapter.GetUser(bid.UserId);

            if(user == null) {
                throw new NotFoundException("User", bid.UserId.ToString());
            }

            var (product, existingBid) = databaseAdapter.GetProductBid(bid.ProductId);

            if(product == null) {
                throw new NotFoundException("Product", bid.ProductId.ToString());
            }

            if(product.AuctionEnd <= DateTime.UtcNow) {
                throw new BadRequestException(product, "Auction expired");
            }

            if(existingBid != null && bid.Price <= existingBid.Price)
            {
                throw new BadRequestException(bid, $"Unsupported price. Should be greater then ${existingBid.Price}");
            }

            bid.Id = Guid.NewGuid();
            databaseAdapter.AddBid(bid);

            return bid;
        }

        public IEnumerable<Auction> GetAuctions(Guid? productId)
        {
            var result = databaseAdapter.GetProductBids(productId);

            return result.Select(item => new Auction
            {
                Product = item.Product,
                Bid = item.bid,
                Status = GetAuctionStatus(item.Product, item.bid)
            });   
        }

        private AuctionStatus GetAuctionStatus(Product product, Bid bid)
        {
            if(product.AuctionEnd >= DateTime.UtcNow) {
                return bid == null ? AuctionStatus.Empty : AuctionStatus.InProgress;
            }

            return AuctionStatus.Finished; 
        }
    }
}
