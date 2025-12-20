using GestionIntApi.DTO;
using GestionIntApi.Repositorios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestionIntApi.Controllers
{
    public class UbicacionController : Controller
    {
        private readonly IUbicacionService _ubicacionService;

        public UbicacionController(IUbicacionService ubicacionService)
        {
            _ubicacionService = ubicacionService;
        }


        [HttpPost]

        [Authorize]
        public IActionResult Registrar([FromBody] UbicacionDTO dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            _ubicacionService.Registrar(userId, dto);

            return Ok(new { mensaje = "Ubicación registrada" });
        }
    }
}
