using IfsDashboardApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace IfsDashboardApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IIfsService _ifsService;

        public DashboardController(IIfsService ifsService)
        {
            _ifsService = ifsService;
        }

        // Test
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("API çalışıyor 🚀");
        }

        // PDKS Süreleri
        [HttpGet("pdks")]
        public async Task<IActionResult> GetPdks(
            [FromQuery] DateTime? baslangic,
            [FromQuery] DateTime? bitis)
        {
            // Parametre gelmezse default: son 7 gün
            var b = baslangic ?? DateTime.Today.AddDays(-7);
            var t = bitis ?? DateTime.Today;

            var data = await _ifsService.GetPdksSureleriAsync(b, t);
            return Ok(data);
        }
        [HttpGet("iscilik")]
        public async Task<IActionResult> GetIscilik([FromQuery] DateTime? baslangic, [FromQuery] DateTime? bitis)
        {
            var data = await _ifsService.GetIscilikSureleriAsync(baslangic, bitis);
            return Ok(data);
        }

        [HttpGet("uretim/sevkiyatlar")]
        public async Task<IActionResult> GetSevkiyatlar([FromQuery] DateTime? baslangic, [FromQuery] DateTime? bitis)
        {
            var data = await _ifsService.GetSevkiyatlarAsync(baslangic, bitis);
            return Ok(data);
        }
       


    }
}

