using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetHello()
        {
            var result = new
            {
                Message = "Chào anh Thanh!",
                Project = "Course Management System",
                Version = "1.0",
                Status = "Online"
            };

            return Ok(result);
        }
    }
}
