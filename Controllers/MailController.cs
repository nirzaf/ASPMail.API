using System.Threading.Tasks;
using MailService;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASPMail.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly ISender _mail;

        public MailController(ISender mail)
        {
            _mail = mail;
        }

        // POST api/<MailController>
        [HttpPost]
        public async Task Post([FromBody] Message message)
        {
            await _mail.SendEmailAsync(message);
        }
    }
}
