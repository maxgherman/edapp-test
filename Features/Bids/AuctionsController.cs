using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace EdAppTest.Features.Bids
{
    [Route("api/[controller]")]
    public class AuctionsController : ControllerBase
    {
        private readonly IBidsService bidsService;
        
        public AuctionsController(IBidsService bidsService)
        {
            this.bidsService = bidsService; 
        }

        [HttpGet()]
        public IActionResult Get([FromQuery] string productId = null)
        {
            var productIdValue = string.IsNullOrWhiteSpace(productId) ?
                (Guid?)null : Guid.Parse(productId);
            
            var result = bidsService.GetAuctions(productIdValue)
                .Select(item => new AuctionViewModel(item));

            return Ok(result);
        }
    }
}
