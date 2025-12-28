using GestionIntApi.DTO;
using GestionIntApi.Repositorios.Implementacion;
using GestionIntApi.Repositorios.Interfaces;
using GestionIntApi.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionIntApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TiendaAppController : Controller
    {


        private readonly ITiendaService _TiendaServicios;
        private readonly ILogger<TiendaAppController> _logger;

        public TiendaAppController(ITiendaService TiendaServicios, ILogger<TiendaAppController> logger)
        {
            _TiendaServicios = TiendaServicios;
            _logger = logger;
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



                var tienda = await _TiendaServicios.GetTiendasCliente(clienteId);
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

        [HttpPost("asociar")]
        public async Task<IActionResult> AsociarTienda([FromBody] TiendaAppDTO dto)
        {


         
            var rsp = new Response<bool>();

            try
            {
                dto.ClienteId = int.Parse(User.FindFirst("ClienteId")!.Value);

                rsp.status = true;
                rsp.value = await _TiendaServicios.AsociarTiendaCliente(dto);
                rsp.msg = "Tienda asociada correctamente";
            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;
            }

            return Ok(rsp);
        }

        [HttpPost("asociar1")]
        public async Task<IActionResult> AsociarTienda1([FromBody] TiendaAppDTO dto)
        {
            var rsp = new Response<bool>();

            try
            {
                // 🔥 Validar que el DTO no sea null
                if (dto == null)
                {
                    rsp.status = false;
                    rsp.msg = "Los datos de la tienda son requeridos";
                    return BadRequest(rsp);
                }

                // 🔥 Validar que ClienteId venga en el DTO
                if (dto.ClienteId == 0)
                {
                    rsp.status = false;
                    rsp.msg = "El ClienteId es requerido";
                    return BadRequest(rsp);
                }

                rsp.value = await _TiendaServicios.AsociarTiendaCliente(dto);
                rsp.status = true;
                rsp.msg = "Tienda asociada correctamente";
            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;
            }

            return Ok(rsp);
        }

        [HttpPost]
        [Route("GuardarTiendaJWT")]
        [Authorize] // Requiere JWT válido
        public async Task<IActionResult> GuardarTinedaConJWT1([FromBody] TiendaAppDTO tienda)
        {
            var rsp = new Response<TiendaAppDTO>();

            try
            {
                // 1️⃣ Obtener ClienteId desde el JWT
                var clienteIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ClienteId");
                if (clienteIdClaim == null)
                {
                    rsp.status = false;
                    rsp.msg = "Cliente no identificado en el token.";
                    return Unauthorized(rsp);
                }

                tienda.ClienteId = int.Parse(clienteIdClaim.Value);

                // 2️⃣ Crear el crédito usando el servicio
                var tiendaNueva = await _TiendaServicios.AsociarTiendaCliente(tienda);

                rsp.status = true;
                rsp.msg = "Crédito actulizada correctamente.";
                rsp.value = new TiendaAppDTO

                {
                    Id = tienda.Id,

                    CedulaEncargado = tienda.CedulaEncargado,
                    ClienteId = tienda.ClienteId,
                    EstadoDeComision= tienda.EstadoDeComision,
                    FechaRegistro = DateTime.UtcNow
                };


            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;
            }

            return Ok(rsp);
        }

        [HttpGet("tiendasAppFechaV")]

        [Authorize] // 🔒 Protegido con JWT
        public async Task<ActionResult<List<TiendaMostrarAppDTO>>> GetFechaVenta()
        {
            try
            {

                var clienteIdClaim = User.FindFirst("ClienteId")?.Value;
                if (string.IsNullOrEmpty(clienteIdClaim))
                    return Unauthorized("Token inválido o ClienteId no encontrado");

                int clienteId = int.Parse(clienteIdClaim);



                var tienda = await _TiendaServicios.GetFechaVenta(clienteId);
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
        [HttpGet("tiendasAppFechaVSinJWT/{id}")]

    // 🔒 Protegido con JWT
        public async Task<ActionResult<List<TiendaMostrarAppDTO>>> GetFechaVentaSINJWT(int id)
        {
            try
            {

 
                var tienda = await _TiendaServicios.GetFechaVenta(id);
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


    }
}
