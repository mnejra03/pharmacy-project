using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.Models;
using NewPharmacy.Endpoints;
using NewPharmacy.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewPharmacy.Endpoints.ProductEndpoints
{
    [Route("api/products/search")]
    [ApiController]
    public class SearchProductsEndpoint : ControllerBase
    {

        private readonly ProductService _productService;

        public SearchProductsEndpoint(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchProducts([FromQuery] string query)
        {
            var products = await _productService.SearchProductsAsync(query);
            return Ok(products);
        }
    }

}
