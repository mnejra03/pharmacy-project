using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.DTOs;

namespace NewPharmacy.Endpoints
{
    [Route("api/dashboard")]
    [ApiController]
    public class GetDashboardEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GetDashboardEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        [MyAuthorization(isAdmin: true, isPharmacist: false, isCustomer: false)]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var activeOrders = _context.Orders.Where(o => !o.IsDeleted && !o.IsSupplyOrder);

            var todaySales = await activeOrders
                .Where(o => o.OrderDate.Date == today)
                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;

            var monthlySales = await activeOrders
                .Where(o => o.OrderDate >= startOfMonth)
                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;

            var orderCount = await activeOrders.CountAsync();
            var userCount = await _context.MyAppUsers.CountAsync();
            var stockCount = await _context.Products.SumAsync(p => p.QuantityInStock);

            var past7Days = Enumerable.Range(0, 7)
                .Select(i => today.AddDays(-i))
                .ToList();

            var salesByDay = await activeOrders
                .Where(o => o.OrderDate >= past7Days.Last())
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Amount = g.Sum(o => o.TotalPrice)
                })
                .ToListAsync();

            var dailySalesData = past7Days
                .Select(date => new DailySaleDto
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    Amount = salesByDay.FirstOrDefault(d => d.Date == date)?.Amount ?? 0
                })
                .OrderBy(d => d.Date)
                .ToList();
           

            var stats = new DashboardStatsDto
            {
                TodaySales = todaySales,
                MonthlySales = monthlySales,
                OrderCount = orderCount,
                UserCount = userCount,
                StockCount = stockCount,
                DailySalesData = dailySalesData
            };

            return Ok(stats);
        }
    }
}
