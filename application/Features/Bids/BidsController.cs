using System;
using System.ComponentModel.DataAnnotations;
using EdAppTest.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace EdAppTest.Features.Bids
{
    [Route("api/[controller]")]
    public class BidsController : ControllerBase
    {
        private readonly IBidsService bidsService;
        
        public BidsController(IBidsService bidsService)
        {
            this.bidsService = bidsService; 
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateBidRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = bidsService.AddBid(new EdAppTest.Domain.Bid
            {
                Price = request.Price.Value,
                ProductId = Guid.Parse(request.ProductId),
                UserId = HttpContext.UserId()
            });

            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            // TODO: implememt get bid
            return Ok();
        }

        public class CreateBidRequest
        {
            [Required]
            [MinLength(1)]
            public string ProductId { get; set; }

            [Required]
            public decimal? Price { get; set; }
        }
    }
}
