using Microsoft.Extensions.Options;
using PremiumPlace_API.Infrastructure.Payments.PayPal;
using System.Globalization;

namespace PremiumPlace_API.Services.PayPal
{
    public class PayPalPaymentVerifier : IPayPalPaymentVerifier
    {
        private readonly IPayPalOrdersClient _ordersClient;
        private readonly PayPalOptions _options;

        public PayPalPaymentVerifier(IPayPalOrdersClient ordersClient, IOptions<PayPalOptions> options)
        {
            _ordersClient = ordersClient;
            _options = options.Value;
        }

        public async Task VerifyOrThrowAsync(
            string payPalOrderId,
            decimal expectedAmount,
            string expectedCurrency,
            string idempotencyKey,
            CancellationToken ct = default)
        {
            if (!string.IsNullOrWhiteSpace(_options.ExpectedCurrency))
                expectedCurrency = _options.ExpectedCurrency!;

            // 1) Show order details
            var order = await _ordersClient.GetOrderAsync(payPalOrderId, ct);

            // Allowed statuses before capture
            var status = (order.Status ?? "").ToUpperInvariant();
            if (status is not "APPROVED" and not "COMPLETED")
                throw new InvalidOperationException($"PayPal order status invalid: {order.Status}");

            var pu = order.PurchaseUnits.FirstOrDefault()
                     ?? throw new InvalidOperationException("PayPal order has no purchase_units.");

            // 2) Validate amount/currency (from order)
            var currency = (pu.Amount.CurrencyCode ?? "").ToUpperInvariant();
            if (!string.Equals(currency, expectedCurrency, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Currency mismatch. Expected {expectedCurrency}, got {currency}.");

            if (!TryParseMoney(pu.Amount.Value, out var actualAmount))
                throw new InvalidOperationException("PayPal amount value invalid.");

            if (decimal.Round(actualAmount, 2) != decimal.Round(expectedAmount, 2))
                throw new InvalidOperationException($"Amount mismatch. Expected {expectedAmount:0.00}, got {actualAmount:0.00}.");

            // 3) Optional capture (recommended for “real confirmation”)
            if (_options.CaptureOnConfirm)
            {
                var capture = await _ordersClient.CaptureOrderAsync(payPalOrderId, idempotencyKey, ct);

                var capStatus = (capture.Status ?? "").ToUpperInvariant();
                if (capStatus != "COMPLETED")
                    throw new InvalidOperationException($"Capture status invalid: {capture.Status}");

                // Extra: validate capture amount if present
                var capPu = capture.PurchaseUnits.FirstOrDefault();
                var cap = capPu?.Payments?.Captures?.FirstOrDefault();
                if (cap is not null)
                {
                    var capCur = (cap.Amount.CurrencyCode ?? "").ToUpperInvariant();
                    if (!string.Equals(capCur, expectedCurrency, StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException($"Capture currency mismatch. Expected {expectedCurrency}, got {capCur}.");

                    if (TryParseMoney(cap.Amount.Value, out var capAmount))
                    {
                        if (decimal.Round(capAmount, 2) != decimal.Round(expectedAmount, 2))
                            throw new InvalidOperationException($"Capture amount mismatch. Expected {expectedAmount:0.00}, got {capAmount:0.00}.");
                    }
                }
            }
        }

        private static bool TryParseMoney(string? s, out decimal value)
            => decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
    }
}
