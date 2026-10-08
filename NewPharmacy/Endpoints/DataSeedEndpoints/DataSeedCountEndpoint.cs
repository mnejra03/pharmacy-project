using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Helper.Api;
using NewPharmacy.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NewPharmacy.Endpoints.DataSeedEndpoints
{
    [Route("data-seed")]
    public class DataSeedCountEndpoint(ApplicationDbContext db)
        : MyEndpointBaseAsync
        .WithoutRequest
        .WithResult<Dictionary<string, int>>
    {
        [HttpGet]
        [MyAuthorization(isAdmin: true, isPharmacist: false, isCustomer: false)]
        public override async Task<Dictionary<string, int>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var myAppUserCount = await db.MyAppUsers.CountAsync(cancellationToken);
            var productCount = await db.Products.CountAsync(cancellationToken);
            var categoryCount = await db.Categories.CountAsync(cancellationToken);
            var supplierCount = await db.Suppliers.CountAsync(cancellationToken);

            Dictionary<string, int> dataCounts = new()
            {
                { "MyAppUser", myAppUserCount },
                { "Product", productCount },
                { "Category", categoryCount },
                { "Supplier", supplierCount }
            };

            return dataCounts;
        }
    }
}
