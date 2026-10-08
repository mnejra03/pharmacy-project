using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewPharmacy.Data;
using NewPharmacy.Data.Models;
using NewPharmacy.Services;
using System.ComponentModel.DataAnnotations;

namespace NewPharmacy.Endpoints.OrderEndpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderMedicineEndpoint : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MyAuthService _authService;

        public OrderMedicineEndpoint(ApplicationDbContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> OrderMedicine([FromBody] OrderMedicineDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || !authInfo.IsLoggedIn)
            {
                return Unauthorized();
            }

            if (!authInfo.IsPharmacist)
            {
                return Forbid();
            }

            var medicine = await _context.Products.FindAsync(request.MedicineId);
            if (medicine == null)
            {
                return NotFound(new { message = "Lijek nije pronađen." });
            }

            if (request.Quantity <= 0)
            {
                return BadRequest(new { message = "Količina mora biti veća od 0." });
            }

            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                MyAppUserId = authInfo.UserId,
                Status = "Pending",
                IsSupplyOrder = true,
                PaymentMethod = "Internal Transfer",
                ShippingAddress = "Apoteka Sarajevo",
                TotalPrice = request.Quantity * medicine.Price
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderDetail = new OrderDetail
            {
                OrderId = order.Id,
                ProductId = medicine.Id,
                Qty = request.Quantity,
                PricePerUnit = medicine.Price
            };
            _context.OrderDetails.Add(orderDetail);

            var notification = new Notification
            {
                MyAppUserId = authInfo.UserId,
                Title = "Nova narudžba zaliha",
                Message = $"Uspješno ste naručili lijek: {medicine.Name} ({request.Quantity} komada)",
                Time = DateTime.UtcNow,
                Read = false
            };
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Narudžba za lijek uspješno kreirana." });
        }
    }

    public class OrderMedicineDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "MedicineId must be greater than 0.")]
        public int MedicineId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }
    }
}
