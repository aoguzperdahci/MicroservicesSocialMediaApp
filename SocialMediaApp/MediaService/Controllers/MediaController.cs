using MediaService.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MediaService.Controllers
{
   // [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {


        //[HttpGet("{username}/{filename}")]
        [HttpPost]
        //[Route("media")]
        private async Task<IActionResult> WriteFile([FromBody] MediaRequest mediaRequest/*string username, string filename, IFormFile file*/)
        {
           // string filename = "";
            try
            {
                // var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                //filename = DateTime.Now.Ticks.ToString() + extension;
                string username = mediaRequest.Username;
                string filename = mediaRequest.Filename;

                var filepath = Path.Combine("c://socialMedia", username);

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                //var exactpath = Path.Combine("c://socialMedia", filename);

                //var exactpath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\MediaFiles", filename);
                using (var stream = new FileStream(filepath, FileMode.Create))
                {
                    //await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
            }
            return Ok("Image saved");
        }







        /* [HttpPost]
         [Route("UploadFile")]
         [ProducesResponseType(StatusCodes.Status200OK)]
         [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]

         public async Task<IActionResult> UploadFile(IFormFile file, CancellationToken cancellationtoken)
         {
             var result = await WriteFile(file);
             return Ok(result);
         }*/

        
      /*   private async Task<string> WriteFile(IFormFile file)
          {
              string filename = "";
              try
              {
                  var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                  filename = DateTime.Now.Ticks.ToString() + extension;

                  var filepath = Path.Combine("c://socialMedia", filename);

                  if(!Directory.Exists(filepath))
                  {
                      Directory.CreateDirectory(filepath);
                  }

                  var exactpath = Path.Combine("c://socialMedia", filename);

                  //var exactpath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\MediaFiles", filename);
                  using (var stream = new FileStream(exactpath, FileMode.Create))
                  {
                      await file.CopyToAsync(stream);
                  }
              }
              catch (Exception ex)
              {
              }
              return filename;
          }*/

      
    }
}
