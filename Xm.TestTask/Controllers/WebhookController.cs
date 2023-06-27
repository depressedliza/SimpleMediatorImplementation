using Microsoft.AspNetCore.Mvc;
using Xm.TestTask.Interfaces;

namespace Xm.TestTask.Controllers
{
    [Route("test")]
    public class WebhookController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public WebhookController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> HandleAsync()
        {
            var dataType = "SetAction"; 
                //Request.Headers["x-xdt"].First();
            var dataBody = await ReadRequestBodyAsync();
            
            return Ok(await _mediator.Dispatch(dataType, dataBody));
        }

        private async Task<byte[]> ReadRequestBodyAsync()
        {
            using (var memoryStream = new MemoryStream())
            {
                await Request.Body.CopyToAsync(memoryStream);

                return memoryStream.ToArray();
            }
        }
    }
}
