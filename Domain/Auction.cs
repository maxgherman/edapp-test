namespace EdAppTest.Domain
{
    public class Auction
    {
        public Auction()
        {
            Status = AuctionStatus.Empty;
        }

        public Product Product { get; set; }

        public Bid Bid { get; set; }

        public AuctionStatus Status { get; set; }
    }

    public enum AuctionStatus
    {
        Empty,

        InProgress,

        Finished
    }
}