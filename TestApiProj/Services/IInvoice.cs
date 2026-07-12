using Microsoft.AspNetCore.Mvc;
using TestApiProj.Models;

namespace TestApiProj.Services
{
    public interface IInvoice
    {
        Task<FileStreamResult> GenerateInvoicePdf(Invoice invoice);
    }
}
