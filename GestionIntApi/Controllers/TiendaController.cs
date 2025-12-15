using GestionIntApi.DTO;
using GestionIntApi.Repositorios.Interfaces;
using GestionIntApi.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionIntApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TiendaController : Controller
    {


        private readonly ITiendaService _TiendaServicios;
        private readonly ILogger<TiendaController> _logger;

        public TiendaController(ITiendaService TiendaServicios, ILogger<TiendaController> logger)
        {
            _TiendaServicios = TiendaServicios;
            _logger = logger;
        }



        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            var rsp = new Response<List<TiendaDTO>>();
            try
            {
                rsp.status = true;
                rsp.value = await _TiendaServicios.GetAllTiendas();
            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;
            }
            return Ok(rsp);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TiendaDTO>> GetById(int id)
        {
            try
            {
                var odontologo = await _TiendaServicios.GetTiendaById(id);
                if (odontologo == null)
                    return NotFound();
                return Ok(odontologo);
            }
            catch
            {
                return StatusCode(500, "Error al obtener la tienda por ID");
            }
        }



        [HttpGet("tiendaApp/{clienteId}")]
        public async Task<ActionResult<List<TiendaMostrarAppDTO>>> GetByIdApp(int clienteId)
        {
            try
            {
                var tiendas = await _TiendaServicios.GetTiendasApp(clienteId);
                if (tiendas == null || !tiendas.Any())
                    return NotFound();

                return Ok(tiendas);
            }
            catch (Exception ex)
            {
                // Log detallado del error
                _logger.LogError(ex, "Error al obtener las tiendas del clienteId {ClienteId}", clienteId);

                // También se puede devolver el mensaje en desarrollo
                return StatusCode(500, $"Error al obtener las tiendas del clienteId {clienteId}: {ex.Message}");
            }
        }


        [HttpGet("tiendasApp")]

        [Authorize] // 🔒 Protegido con JWT
        public async Task<ActionResult<List<TiendaMostrarAppDTO>>> GetCreditosPendientesApp()
        {
            try
            {

                var clienteIdClaim = User.FindFirst("ClienteId")?.Value;
                if (string.IsNullOrEmpty(clienteIdClaim))
                    return Unauthorized("Token inválido o ClienteId no encontrado");

                int clienteId = int.Parse(clienteIdClaim);



                var tienda = await _TiendaServicios.GetTiendasApp(clienteId);
                if (tienda == null)
                    return NotFound();
                return Ok(tienda);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); // o usar ILogger
                return StatusCode(500, $"Error al obtener los créditos: {ex.Message}");

            }
        }


        [HttpPost]
        [Route("Guardar")]
        public async Task<IActionResult> Guardar([FromBody] TiendaDTO tienda)
        {
            var rsp = new Response<TiendaDTO>();

            try
            {
                // 1. Validar correo

                // 2. Registrar usuario directamente
                var nuevoCredito = await _TiendaServicios.CreateTienda(tienda);

                rsp.status = true;
                rsp.msg = "Tienda registrada correctamente.";
                rsp.value = nuevoCredito;
            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;
            }

            return Ok(rsp);
        }

        [HttpPut]
        [Route("Editar")]
        public async Task<IActionResult> Editar([FromBody] TiendaDTO Detalle)
        {
            var rsp = new Response<bool>();
            try
            {
                rsp.status = true;
                rsp.value = await _TiendaServicios.UpdateTienda(Detalle);
            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;
            }
            return Ok(rsp);
        }

        [HttpDelete]
        [Route("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var rsp = new Response<bool>();
            try
            {
                rsp.status = true;
                rsp.value = await _TiendaServicios.DeleteTienda(id);
            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;
            }
            return Ok(rsp);
        }
    }
}
