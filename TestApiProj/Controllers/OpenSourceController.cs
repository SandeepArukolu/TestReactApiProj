using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestApiProj.DataClasses;
using TestApiProj.MainEntity;
using TestApiProj.Models;
using TestApiProj.Services;

namespace TestApiProj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenSourceController : ControllerBase
    {

        private readonly IInvoice _invoice;
        private readonly MyDbContext _Context;
        private readonly IItemService _itemService;
        public OpenSourceController(IInvoice invoice, MyDbContext myDbContext, IItemService itemService)
        {
            _invoice = invoice;
            _Context = myDbContext;
            _itemService = itemService;
        }
        
        [HttpGet("CheckAdminAuthorization")]
        [Authorize(Policy = "Admin")]
        public IActionResult Get()
        {
            return Ok(new { message = "This is a Admin protected resource" });
        }

        [HttpGet("CheckUserAuthorization")]
        [Authorize(Policy = "User")]
        public IActionResult GetName()
        {
            return Ok(new { message = "This is a User protected resource" });
        }

        [HttpPost("DownloadInvoice")]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<IActionResult> DownloadInvoice([FromBody] Invoice formData)
        {
            try
            {
                // Generate PDF
                var pdfResult = await _invoice.GenerateInvoicePdf(formData);

                // Set headers for file download
                Response.ContentType = "application/pdf";
                Response.Headers.Add("Content-Disposition", $"attachment; filename={formData.CustomerName}Invoice.pdf");
                
                // Return the PDF file for download
                return pdfResult; 
            }
            catch (Exception ex)
            {
                // Handle any errors appropriately
                return BadRequest($"Error generating the invoice: {ex.Message}");
            }
        }

        [HttpGet("GetAllItems")]
        public async Task<IActionResult> GetAllItems()
        {
            var Items = ItemData.Items;
            return Ok(Items);
        }

        [HttpPost("AddItem")]
        public async Task<IActionResult> AddItem([FromBody] Items formData)
        {
           string response= _itemService.AddItem(formData);
            return Ok(response);
        }

    }
}
