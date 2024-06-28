using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class BarcodeController : ControllerBase
    {
    }
}