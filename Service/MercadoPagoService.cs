using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using TiendaUT.Context;

namespace TiendaUT.Service
{
    public class MercadoPagoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public MercadoPagoService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            var accessToken = _configuration["MercadoPago:AccessToken"];
        }

        public async Task<Preference> CreatePreferenceAsync(List<PreferenceResponse> items)
        {
            var preferenceRequest = new PreferenceRequest
            {
                Items = items.Select(item => new PreferenceItemRequest
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    PictureUrl = item.PictureUrl,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    CurrencyId = "MXN",
                }).ToList(),
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = "http://localhost:5173/",
                    Failure = "http://localhost:5173/",
                    Pending = "http://localhost:5173/"
                },
                AutoReturn = "approved",
                //NotificationUrl = "" PRUEBA
            };

            var client = new PreferenceClient();
            var preference = await client.CreateAsync(preferenceRequest);
            return preference;
        }



    }
}
