using MediaService.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MediaService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> WriteFile([FromForm] MediaRequest mediaRequest)
        {
            try
            {
                var filepath = Path.Combine("c://socialMedia", mediaRequest.Username);

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                var exactpath = Path.Combine(filepath, mediaRequest.Filename);

                using (var stream = new FileStream(exactpath, FileMode.Create))
                {
                    await mediaRequest.File.CopyToAsync(stream);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
