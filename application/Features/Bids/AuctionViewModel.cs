using System;
using EdAppTest.Domain;
using EdAppTest.Features.Products;

namespace EdAppTest.Features.Bids
{
    public class AuctionViewModel
    {
        public ProductViewModel Product { get; set; }

        public BidViewModel Bid { get; set; }

        public AuctionStatus Status { get; set;}

        public AuctionViewModel()
        { }

        public AuctionViewModel(Auction auction)
        {
            Product = new ProductViewModel(auction.Product);
            Bid = auction.Bid == null ? null : new BidViewModel
                {
                    Id = auction.Bid.Id,
                    UserId = auction.Bid.UserId,
                    Price = auction.Bid.Price
                };
                
            Status = auction.Status;
        }

        public class BidViewModel
        {
            public Guid Id { get; set; }

            public Guid UserId { get; set; }

            public decimal Price { get; set; }

        }
    }
}