using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NewPharmacy.Data.DTOs;
using NewPharmacy.Services;
using Stripe;

namespace NewPharmacy.Endpoints.PaymentEndpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreatePaymentIntentEndpoint : ControllerBase
    {
        private readonly OrderPricingService _orderPricingService;
        private readonly StripeSettings _stripeSettings;
        private readonly MyAuthService _authService;

        public CreatePaymentIntentEndpoint(
            OrderPricingService orderPricingService,
            IOptions<StripeSettings> stripeOptions,
            MyAuthService authService)
        {
            _orderPricingService = orderPricingService;
            _stripeSettings = stripeOptions.Value;
            _authService = authService;
        }

        [HttpPost]
        [MyAuthorization(isAdmin: false, isPharmacist: false, isCustomer: true)]
        public async Task<IActionResult> Create([FromBody] CreatePaymentIntentDTO dto, CancellationToken cancellationToken)
        {
            var authInfo = _authService.GetAuthInfo();
            if (authInfo == null || !authInfo.IsLoggedIn)
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(_stripeSettings.SecretKey) || _stripeSettings.SecretKey == "sk_test_replace_me")
            {
                return StatusCode(500, "Stripe secret key is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_stripeSettings.PublishableKey) || _stripeSettings.PublishableKey == "pk_test_replace_me")
            {
                return StatusCode(500, "Stripe publishable key is not configured.");
            }

            try
            {
                var total = await _orderPricingService.CalculateTotalAsync(dto.Items, dto.DeliveryMethod, cancellationToken);
                var amount = _orderPricingService.ToMinorUnits(total);

                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
                {
                    Amount = amount,
                    Currency = _stripeSettings.Currency.ToLowerInvariant(),
                    PaymentMethodTypes = new List<string> { "card" },
                    Metadata = new Dictionary<string, string>
                    {
                        ["myAppUserId"] = authInfo.UserId.ToString(),
                        ["deliveryMethod"] = dto.DeliveryMethod ?? string.Empty,
                        ["calculatedTotal"] = total.ToString("0.00")
                    }
                }, cancellationToken: cancellationToken);

                return Ok(new
                {
                    clientSecret = paymentIntent.ClientSecret,
                    paymentIntentId = paymentIntent.Id,
                    publishableKey = _stripeSettings.PublishableKey,
                    amount = total
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (StripeException ex)
            {
                return StatusCode(502, ex.StripeError?.Message ?? ex.Message);
            }
        }
    }
}
