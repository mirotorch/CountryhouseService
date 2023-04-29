using CountryhouseService.Data;
using Microsoft.AspNetCore.Mvc;
using CountryhouseService.Models;

namespace CountryhouseService.Controllers
{
    public class MediaController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        public MediaController(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }


        [HttpGet("Media/{id:int}")]
        public IActionResult Image(int? id)
        {
            Image? image = _dbcontext.Images.Find(id);
            if (image != null)
            {
                byte[] file = image.file;
                return File(file, "image/jpeg");
            }
            else return NotFound(); 
        }
    }
}
