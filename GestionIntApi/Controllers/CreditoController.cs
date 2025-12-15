using GestionIntApi.DTO;
using GestionIntApi.Repositorios.Interfaces;
using GestionIntApi.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionIntApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CreditoController : Controller
    {

        private readonly ICreditoService _CreditoServicios;
        private readonly ICodigoVerificacionService _codigoService;
        private readonly IEmailService _emailService;
        private readonly IRegistroTemporalService _registroTemporal;


        public CreditoController( ICreditoService CreditoServicios)
        {
            _CreditoServicios = CreditoServicios;

        }



        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista()
        {
            var rsp = new Response<List<CreditoDTO>>();
            try
            {
                rsp.status = true;
                rsp.value = await _CreditoServicios.GetAllTiendas();
            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;
            }
            return Ok(rsp);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CreditoDTO>> GetById(int id)
        {
            try
            {
                var odontologo = await _CreditoServicios.GetTiendaById(id);
                if (odontologo == null)
                    return NotFound();
                return Ok(odontologo);
            }
            catch
            {
                return StatusCode(500, "Error al obtener el Odontólogo por ID");
            }
        }


        [HttpGet("pendientes/{Id}")]
        public async Task<ActionResult<CreditoDTO>> GetByIdCreditoActivo(int id)
        {
            try
            {
                var credito = await _CreditoServicios.GetCreditosPendientesPorCliente(id);
                if (credito == null)
                    return NotFound();
                return Ok(credito);
            }
            catch
            {
                return StatusCode(500, "Error al obtener el credito por ID");
            }
        }
        [HttpGet("pendientes1/{clienteId}")]
        public async Task<ActionResult<CreditoDTO>> GetByIdCreditoActivo1(int clienteId)
        {
            try
            {
                var credito = await _CreditoServicios.GetCreditosPendientesPorCliente1(clienteId);
                if (credito == null)
                    return NotFound();
                return Ok(credito);
            }
            catch
            {
                return StatusCode(500, "Error al obtener el credito por ID");
            }
        }
        [HttpGet("pendientesApp")]

        [Authorize] // 🔒 Protegido con JWT
        public async Task<ActionResult<List<CreditoMostrarDTO>>> GetByIdCreditoActivoApp()
        {
            try
            {

                var clienteIdClaim = User.FindFirst("ClienteId")?.Value;
                if (string.IsNullOrEmpty(clienteIdClaim))
                    return Unauthorized("Token inválido o ClienteId no encontrado");

                int clienteId = int.Parse(clienteIdClaim);



                var credito = await _CreditoServicios.GetCreditosClienteApp(clienteId);
                if (credito == null)
                    return NotFound();
                return Ok(credito);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); // o usar ILogger
                return StatusCode(500, $"Error al obtener los créditos: {ex.Message}");

            }
        }

        [HttpPost]
        [Route("Guardar")]
        public async Task<IActionResult> Guardar([FromBody] CreditoDTO credito)
        {
            var rsp = new Response<CreditoDTO>();

            try
            {
                // 1. Validar correo

                // 2. Registrar usuario directamente
                var nuevoCredito = await _CreditoServicios.CreateCredito(credito);

                rsp.status = true;
                rsp.msg = "Usuario registrado correctamente.";
                rsp.value = nuevoCredito;
            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;
            }

            return Ok(rsp);
        }



        [HttpPost]
        [Route("RegistrarPago")]
        public async Task<IActionResult> GuardarPago([FromBody] PagoCreditoDTO credito)
        {
            var rsp = new Response<CreditoDTO>();

            try
            {
                // 1. Validar correo

                // 2. Registrar usuario directamente
                var nuevoCredito = await _CreditoServicios.RegistrarPagoAsync(credito);

                rsp.status = true;
                rsp.msg = "Usuario registrado correctamente.";
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
        public async Task<IActionResult> Editar([FromBody] CreditoDTO Credito)
        {
            var rsp = new Response<bool>();
            try
            {
                rsp.status = true;
                rsp.value = await _CreditoServicios.UpdateCredito(Credito);
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
                rsp.value = await _CreditoServicios.DeleteCredito(id);
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
