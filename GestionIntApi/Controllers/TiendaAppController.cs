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
        public async Task<IActionResult> AsociarTienda([FromBody] AsociarTiendaClienteDTO dto)
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



        [HttpPost]
        [Route("GuardarTiendaJWT")]
        [Authorize] // Requiere JWT válido
        public async Task<IActionResult> GuardarTinedaConJWT1([FromBody] AsociarTiendaClienteDTO tienda)
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
                    CedulaEncargado = tienda.CedulaEncargado,
                    ClienteId = tienda.ClienteId,
                    FechaRegistro = DateTime.Now
                };


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
