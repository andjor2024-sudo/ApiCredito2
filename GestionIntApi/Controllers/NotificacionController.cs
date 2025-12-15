using GestionIntApi.DTO;
using GestionIntApi.Repositorios.Implementacion;
using GestionIntApi.Repositorios.Interfaces;
using GestionIntApi.Utilidades;
using Microsoft.AspNetCore.Mvc;

namespace GestionIntApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificacionController : Controller
    {

        private readonly INotificacionServicio _NotificacionServicios;
        private readonly ICodigoVerificacionService _codigoService;
        private readonly IEmailService _emailService;
        private readonly IRegistroTemporalService _registroTemporal;
        public NotificacionController(INotificacionServicio NotificacionServicios)
        {
            _NotificacionServicios = NotificacionServicios;

        }

        [HttpPost("Generar")]
        public async Task<IActionResult> GenerarNotificaciones()
        {
            try
            {
                await _NotificacionServicios.GenerarNotificaciones();
                return Ok(new { mensaje = "Notificaciones generadas correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }


        // Endpoint para obtener todas las notificaciones
        [HttpGet("GetAll")]
        public async Task<ActionResult<List<NotificacionDTO>>> GetNotificaciones()
        {
            try
            {
                var notificaciones = await _NotificacionServicios.GetNotificaciones();
                return Ok(notificaciones);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // Opcional: obtener notificaciones de un cliente específico
        [HttpGet("{id}")]
        public async Task<ActionResult<List<NotificacionDTO>>> GetNotificacionesPorCliente(int clienteId)
        {
            try
            {
                var allNotificaciones = await _NotificacionServicios.GetNotificaciones();
                var notificacionesCliente = allNotificaciones.FindAll(n => n.ClienteId == clienteId);
                return Ok(notificacionesCliente);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }




    }
}
