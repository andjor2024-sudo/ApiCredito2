using GestionIntApi.DTO;
using GestionIntApi.DTO.Admin;
using GestionIntApi.Models;
using GestionIntApi.Models.Admin;
using GestionIntApi.Repositorios.Interfaces;
using GestionIntApi.Repositorios.Interfaces.Admin;
using GestionIntApi.Utilidades;
using Microsoft.AspNetCore.Mvc;

namespace GestionIntApi.Controllers.Admin
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioAdminController : Controller
    {
        private readonly IUsuarioAdminRepository _UsuarioAdminServicios;
        private readonly ICodigoVerificacionService _codigoService;
        private readonly IEmailService _emailService;
        private readonly IUsuarioRepository _UsuarioServicios;
        private readonly IRegistroTemporalAdminService _registroTemporal;
        public UsuarioAdminController(IRegistroTemporalAdminService registroTemporal, IUsuarioRepository UsuarioServicios,IUsuarioAdminRepository usuarioServicios, ICodigoVerificacionService codigoService, IEmailService emailService)
        {
            _UsuarioAdminServicios = usuarioServicios;
            _codigoService = codigoService;
            _emailService = emailService;

            _UsuarioServicios = UsuarioServicios;


            _registroTemporal = registroTemporal;

        }

        [HttpPost]
        [Route("IniciarSesion")]
        public async Task<IActionResult> IniciarSesion([FromBody] LoginDTO login)
        {
            var rsp = new Response<SesionDTOAdmin>();
            try
            {
                rsp.status = true;
                rsp.value = await _UsuarioAdminServicios.ValidarCredenciales(login.Correo, login.Clave);

            }
            catch (Exception ex)
            {
                rsp.status = false;
                rsp.msg = ex.Message;
            }
            return Ok(rsp);
        }

        [HttpPost]
        [Route("Guardar")]
        public async Task<IActionResult> Guardar1([FromBody] UsuarioAdminDTO usuarioAdmin)
        {
            var rsp = new Response<UsuarioAdminDTO>();
            try
            {
                var existe = await _UsuarioServicios.ExisteCorreo(usuarioAdmin.Correo);
                if (existe)
                {
                    rsp.status = false;
                    rsp.msg = "El correo ya está registrado.";
                    return BadRequest(rsp);
                }

                //  var newUser = await _UsuarioServicios.crearUsuario(usuario);

                // 2. Generar Código
                var codigo = new Random().Next(100000, 999999).ToString();

                var datos = new RegistroTemporalAdmin
                {
                    UsuarioAdmin = usuarioAdmin,
                    Codigo = codigo
                };

                // 3. Guardar el código temporal asociado al correo del usuario
                //  _codigoService.GuardarCodigo(usuario.Correo, codigo);
                _registroTemporal.GuardarRegistro(usuarioAdmin.Correo, datos);
                // 4. Enviar correo
                await _emailService.SendEmailAsync(
                    usuarioAdmin.Correo,
                    "Código de verificación",
                    $"<h3>Tu código es: <b>{codigo}</b></h3>"
                );
                rsp.status = true;
                rsp.msg = "Código enviado. Verifique su correo.";
                // rsp.value = await _UsuarioServicios.crearUsuario(usuario);
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
