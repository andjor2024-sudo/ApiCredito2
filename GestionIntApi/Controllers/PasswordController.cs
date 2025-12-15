using GestionIntApi.DTO;
using GestionIntApi.Models;
using GestionIntApi.Repositorios;
using GestionIntApi.Repositorios.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using BCrypt.Net;

namespace GestionIntApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : Controller
    {

        private readonly SistemaGestionDBcontext _db;
        private readonly IEmailService _emailService;
        public PasswordController(SistemaGestionDBcontext db, IEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Correo == dto.Correo);
            // Generar un código numérico de 6 dígitos
            var random = new Random();
            var codigo = random.Next(100000, 999999).ToString(); // Ej: "124325"


            if (usuario == null)
                return BadRequest("El correo no está registrado.");

            // Crear token
            var token = Guid.NewGuid().ToString();

            usuario.PasswordResetToken = codigo;
            usuario.ResetTokenExpires = DateTime.UtcNow.AddHours(1);

            await _db.SaveChangesAsync();

            var link = $"https://tu-frontend.com/reset-password?token={token}";

            string body = $@"
    <h3>Restablecer contraseña</h3>
    <p>Tu token para restablecer contraseña es:</p>
    <h2>{codigo}</h2>
    <p>Utiliza este token en el formulario de restablecimiento.</p>
    <p>Expira en 1 hora.</p>
";

            await _emailService.SendEmailAsync(usuario.Correo, "Restablecer contraseña", body);

            return Ok("Se envió un correo con el enlace para restablecer la contraseña.");
        }


        // ---------------------------
        // 2. Restablecer contraseña
        // ---------------------------
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            var usuario = await _db.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.PasswordResetToken == dto.Token &&
                    u.ResetTokenExpires > DateTime.UtcNow);

            if (usuario == null)
                return BadRequest("Token inválido o expirado.");
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.NuevaClave);
            // Actualizar contraseña
            //  usuario.Clave = ; // Aquí puedes encriptar si quieres

            usuario.Clave = BCrypt.Net.BCrypt.HashPassword(dto.NuevaClave);
            usuario.PasswordResetToken = null;
            usuario.ResetTokenExpires = null;

            await _db.SaveChangesAsync();

            return Ok("Contraseña actualizada correctamente.");
        }
    }
}

