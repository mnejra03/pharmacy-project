using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;
using NewPharmacy.Data.DTOs;

namespace NewPharmacy.Services
{
    public class OrderPricingService
    {
        private readonly ApplicationDbContext _context;

        public OrderPricingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> CalculateTotalAsync(List<OrderItemDTO> items, string? deliveryMethod, CancellationToken cancellationToken = default)
        {
            if (items == null || items.Count == 0)
            {
                throw new InvalidOperationException("Order must contain at least one item.");
            }

            var productIds = items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, cancellationToken);

            decimal total = 0m;

            foreach (var item in items)
            {
                if (item.Qty <= 0)
                {
                    throw new InvalidOperationException("Item quantity must be greater than zero.");
                }

                if (!products.TryGetValue(item.ProductId, out var product))
                {
                    throw new InvalidOperationException($"Product with ID {item.ProductId} does not exist.");
                }

                if (item.Qty > product.QuantityInStock)
                {
                    throw new InvalidOperationException($"Requested quantity for product ID {item.ProductId} exceeds available stock.");
                }

                var unitPrice = product.IsDiscounted && product.DiscountedPrice > 0
                    ? product.DiscountedPrice
                    : product.Price;

                total += unitPrice * item.Qty;
            }

            total += GetDeliveryCost(deliveryMethod);

            return decimal.Round(total, 2, MidpointRounding.AwayFromZero);
        }

        public decimal GetDeliveryCost(string? deliveryMethod)
        {
            return deliveryMethod?.ToLowerInvariant() switch
            {
                "standard" => 6m,
                "express" => 10m,
                _ => 0m
            };
        }

        public long ToMinorUnits(decimal total)
        {
            return (long)decimal.Round(total * 100m, 0, MidpointRounding.AwayFromZero);
        }
    }
}
