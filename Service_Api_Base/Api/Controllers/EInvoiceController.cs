using Application.GetMap;
using Application.GetMap.Invoice;
using Application.Request.Invoice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/e-invoice-holding/e-invoice")]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class EInvoiceController : ControllerBase
    {
        private readonly ILogger _logger;

        public EInvoiceController(ILogger<EInvoiceController> logger)
        {
            _logger = logger;
        }

        [HttpPost("create")]
        public Task<IActionResult> CreateInvoice(ParkingTicketRequest parkingTicketRequest)
        {
            var response = new InvoiceCreateResponse
            {
                Link_Get_Invoice = "https://hddt.pvi.com.vn/GetInvoice.aspx?id=8d00a248-fc1c-4f60-9328-91beccc8d15f"
            };

            return Task.FromResult<IActionResult>(Ok(new ResponseApi(response, true)));
        }
    }
}
