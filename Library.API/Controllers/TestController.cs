using Library.Infrastructure.Datos;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    public class TestController : ControllerBase
    {
        private readonly ContextoBiblioteca _context;

        public TestController(ContextoBiblioteca context)
        {
            _context = context;
        }

        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                if (canConnect)
                {
                    return Ok("Connection successful");
                }
                else
                {
                    return StatusCode(500, "Connection failed");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Connection failed: {ex.Message}");
            }
        }
    }


}
