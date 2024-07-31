using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace TiendaUT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceiptController : ControllerBase
    {
        private readonly IConverter _converter;

        public ReceiptController(IConverter converter)
        {
            _converter = converter;
        }

        [HttpGet("generate-receipt")]
        public IActionResult GenerateReceipt([FromQuery] string orderId)
        {
            // Lógica para obtener los detalles del pedido usando orderId
            var orderDetails = GetOrderDetails(orderId);

            var htmlContent = $@"
                <html>
                <head>
                    <title>Recibo de Compra</title>
                </head>
                <body>
                    <h1>Recibo de Compra</h1>
                    <p>Gracias por tu compra. Aquí están los detalles de tu pedido:</p>
                    <p>Orden ID: {orderDetails.Id}</p>
                    <p>Total: {orderDetails.Total}</p>
                    <!-- Agregar más detalles según sea necesario -->
                </body>
                </html>";

            var pdfDocument = new HtmlToPdfDocument
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4
                },
                Objects = {
                    new ObjectSettings
                    {
                        PagesCount = true,
                        HtmlContent = htmlContent,
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };

            var file = _converter.Convert(pdfDocument);
            return File(file, "application/pdf", "recibo.pdf");
        }

        private dynamic GetOrderDetails(string orderId)
        {
            // Aquí iría la lógica para obtener los detalles del pedido.
            return new { Id = orderId, Total = "100.00" }; // Ejemplo
        }
    }
}
