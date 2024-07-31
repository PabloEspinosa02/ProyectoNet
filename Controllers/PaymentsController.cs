using SystemHttpClient = System.Net.Http.HttpClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using BraintreeHttp;
using Stripe;
using TiendaUT.Service;
using MercadoPago.Client.Preference;

namespace TiendaUT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly SystemHttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly MercadoPagoService _mercadoPagoService;

        public PaymentsController(SystemHttpClient httpClient, IConfiguration configuration, MercadoPagoService mercadoPagoService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _mercadoPagoService = mercadoPagoService;
        }

        [HttpPost("create-paypal-order")]
        public async Task<IActionResult> CreatePayPalOrder(PaymentCreateRequest request)
        {
            var environment = new SandboxEnvironment(
                _configuration["PayPal:ClientId"],
                _configuration["PayPal:ClientSecret"]
            );
            var client = new PayPalHttpClient(environment);

            var orderRequest = new OrdersCreateRequest();
            orderRequest.Prefer("return=representation");
            orderRequest.RequestBody(new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>()
        {
            new PurchaseUnitRequest()
            {
                AmountWithBreakdown = new AmountWithBreakdown()
                {
                    CurrencyCode = "MXN",  // Moneda configurada a pesos mexicanos
                    Value = request.TransactionAmount.ToString()  // Reemplaza esto con el total real del carrito
                }
            }
        },
                ApplicationContext = new ApplicationContext()
                {
                    BrandName = "TiendaUT",
                    LandingPage = "BILLING",
                    UserAction = "PAY_NOW",
                    ReturnUrl = "https://example.com/payment-success", // Reemplaza con tu URL de retorno
                    CancelUrl = "https://example.com/payment-cancel"  // Reemplaza con tu URL de cancelación
                }
            });

            var response = await client.Execute(orderRequest);
            var result = response.Result<Order>();

            return Ok(result);
        }


        [HttpPost("create-stripe-payment-intent")]
        public async Task<IActionResult> CreateStripePaymentIntent(PaymentCreateRequest request)
        {
            var stripeSecretKey = _configuration["Stripe:SecretKey"];
            StripeConfiguration.ApiKey = stripeSecretKey;

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(request.TransactionAmount * 100),
                Currency = "mxn",  // Moneda configurada a pesos mexicanos
                PaymentMethodTypes = new List<string> { "card" },
            };
            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            return Ok(new { clientSecret = paymentIntent.ClientSecret });
        }

        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment(PaymentCreateRequest request)
        {
            var accessToken = _configuration["MercadoPago:AccessToken"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var paymentRequest = new
            {
                transaction_amount = request.TransactionAmount,
                token = request.Token,
                description = request.Description,
                installments = request.Installments,
                payment_method_id = request.PaymentMethodId,
                payer = new
                {
                    email = request.Payer.Email,
                }
            };

            var jsonContent = System.Text.Json.JsonSerializer.Serialize(paymentRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.mercadopago.com/v1/payments", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Ok(System.Text.Json.JsonSerializer.Deserialize<JsonElement>(responseString));
            }
            else
            {
                return BadRequest(responseString);
            }
        }

        [HttpPost("mercado-pago-preference")]
        public async Task<IActionResult> CreatePreference(List<PreferenceRequest> request)
        {
            await _mercadoPagoService.CreatePreferenceAsync(request);
            return Ok();
        }


    }

    public class PaymentCreateRequest
    {
        public decimal TransactionAmount { get; set; }  // Total del carrito
        public string Token { get; set; }
        public string Description { get; set; }
        public int Installments { get; set; }
        public string PaymentMethodId { get; set; }
        public PayerRequest Payer { get; set; }
    }

    public class PayerRequest
    {
        public string Email { get; set; }
    }
}
