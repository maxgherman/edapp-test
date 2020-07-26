using System;

namespace EdAppTest.Domain
{
    public class Bid
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public Guid UserId { get; set; }

        public decimal Price { get; set; }
    }
}