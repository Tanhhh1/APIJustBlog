using Microsoft.AspNetCore.Mvc;

namespace API_Blog.Controllers.Common
{
    [ApiController]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class ApiController : ControllerBase
    {

    }
}
