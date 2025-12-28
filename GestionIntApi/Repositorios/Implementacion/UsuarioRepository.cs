using AutoMapper;
using BCrypt.Net;
using GestionIntApi.DTO;
using GestionIntApi.Models;
using GestionIntApi.Repositorios;
using GestionIntApi.Repositorios.Contrato;
using GestionIntApi.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;


namespace GestionIntApi.Repositorios.Implementacion
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IGenericRepository<Usuario> _UsuarioRepositorio;
        private readonly IGenericRepository<Cliente> _ClienteRepositorio;
        private readonly IGenericRepository<DetalleCliente> _DetalleRepositorio;
        private readonly IGenericRepository<Credito> _CreditoRepositorio;
        private readonly IGenericRepository<Tienda> _TiendaRepositorio;
        private readonly IMapper _mapper;
        private readonly SistemaGestionDBcontext _context;
        private readonly ICreditoService _creditoService;

        private readonly IConfiguration _configuration;
        public UsuarioRepository(IConfiguration configuration,ICreditoService creditoService,IGenericRepository<DetalleCliente> DetalleRepositorio, IGenericRepository<Tienda> TiendaRepositorio, IGenericRepository<Credito> CreditoRepositorio, IGenericRepository<Cliente> ClienteRepositorio, IGenericRepository<Usuario> usuarioRepositorio, IMapper mapper, SistemaGestionDBcontext sistemaGestionDBcontext)
        {
            _UsuarioRepositorio = usuarioRepositorio;
            _mapper = mapper;
            _context = sistemaGestionDBcontext;
            _ClienteRepositorio = ClienteRepositorio;
            _DetalleRepositorio = DetalleRepositorio;
            _CreditoRepositorio = CreditoRepositorio;
            _TiendaRepositorio = TiendaRepositorio;
            _creditoService= creditoService;
            _configuration = configuration;

        }

        public async Task<List<UsuarioDTO>> listaUsuarios()
        {
            try
            {
                var queryUsuario = await _UsuarioRepositorio.Consultar();
                Console.WriteLine("Usuarios encontrados: " + queryUsuario.Count());
                var listaUsuario = queryUsuario.Include(rol => rol.Rol).ToList();
                // Recorremos la lista de usuarios y reemplazamos el hash de la contraseña por el texto plano
                return _mapper.Map<List<UsuarioDTO>>(listaUsuario);
            }
            catch
            {

                throw;
            }
        }

        public async Task<UsuarioDTO> obtenerPorIdUsuario(int id)
        {
            try
            {
                var odontologoEncontrado = await _UsuarioRepositorio
                    .Obtenerid(u => u.Id == id);
                var listaUsuario = odontologoEncontrado.Include(rol => rol.Rol).ToList();
                var odontologo = listaUsuario.FirstOrDefault();
                if (odontologo == null)
                    throw new TaskCanceledException("Usuario no encontrado");
                return _mapper.Map<UsuarioDTO>(odontologo);
            }
            catch
            {
                throw;
            }
        }


     


        public async Task<SesionDTO> ValidarCredenciales(string correo, string clave)
        {
            try
            {
                var queryUsuario = await _UsuarioRepositorio.Consultar(
                u => u.Correo == correo
               );

                if (queryUsuario.FirstOrDefault() == null)
                    throw new TaskCanceledException("El usuario no existe");

                Usuario devolverUsuario = queryUsuario.Include(rol => rol.Rol)
                    .Include(u => u.Cliente) 
                    .FirstOrDefault();

                if (devolverUsuario.EsActivo == false) // Verificar el estado del usuario
                    throw new TaskCanceledException("El usuario está inactivo");
                
                if (!BCrypt.Net.BCrypt.Verify(clave, devolverUsuario.Clave))
                    throw new TaskCanceledException("La contraseña es incorrecta");




                var token = GenerarToken(devolverUsuario);


                var sesionDTO = _mapper.Map<SesionDTO>(devolverUsuario);
                sesionDTO.Token = token;
                return sesionDTO;
            }
            catch
            {
                throw;
            }
        }





        public string GenerarToken1(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim("ClienteId", usuario.Cliente.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.NombreApellidos)
        }),
                Expires = DateTime.UtcNow.AddHours(int.Parse(jwtSettings["ExpiryHours"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public string GenerarToken(Usuario usuario)
        {
            // Intentamos obtener la clave desde la variable de entorno
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                // Si no está la variable, usamos la que está en appsettings.json
                var jwtSettings = _configuration.GetSection("JwtSettings");
                secretKey = jwtSettings["SecretKey"];
            }

            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim("ClienteId", usuario.Cliente.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.NombreApellidos)
        }),
              //  Expires = DateTime.UtcNow.AddHours(int.Parse(jwtSettings["ExpiryHours"])),
                Expires = DateTime.UtcNow.AddHours(int.Parse(_configuration.GetSection("JwtSettings")["ExpiryHours"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<UsuarioDTO> crearUsuario1(UsuarioDTO modelo)
        {
            try
            {

                // Encripta la contraseña del modelo
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(modelo.Clave);
                // Actualiza la propiedad 'Clave' del modelo con la contraseña encriptada
                modelo.Clave = hashedPassword;

                var UsuarioCreado = await _UsuarioRepositorio.Crear(_mapper.Map<Usuario>(modelo));
                
                /////////////////////
                
                
   
                // 2. Guardar DetalleCliente
                var detalle = await _DetalleRepositorio.Crear(
                    _mapper.Map<DetalleCliente>(modelo.Cliente.DetalleCliente)
                );

                // 3. Guardar Cliente
                var cliente = new Cliente
                {
                    UsuarioId = UsuarioCreado.Id,
                    DetalleClienteID = detalle.Id
                };

                cliente = await _ClienteRepositorio.Crear(cliente);

                var tiendasCreadas = new List<TiendaApp>();

                if (modelo.Cliente.TiendaApps != null)
                {
                    foreach (var t in modelo.Cliente.TiendaApps)
                    {
                        // 🔥 CAMBIO: Ahora TiendaApp necesita TiendaId
                        // Primero debes buscar o crear la Tienda real

                        var tiendaExistente = await _context.Tiendas
                            .FirstOrDefaultAsync(x => x.CedulaEncargado == t.CedulaEncargado);

                        if (tiendaExistente == null)
                        {
                            throw new TaskCanceledException(
                                $"La tienda con cédula {t.CedulaEncargado} no existe. " +
                                "Primero debe registrarse la tienda."
                            );
                        }

                        // Crear TiendaApp (la relación intermedia)
                        var TiendaApps = new TiendaApp
                        {
                            TiendaId = tiendaExistente.Id,  // ✅ FK a Tiendas
                            ClienteId = cliente.Id,          // ✅ FK a Clientes
                            CedulaEncargado = t.CedulaEncargado,
                            EstadoDeComision = "Pendiente",  // O el valor que necesites
                            FechaRegistro = DateTime.UtcNow
                        };

                        var tiendaCreada = await _context.TiendaApps.AddAsync(TiendaApps);
                        await _context.SaveChangesAsync();

                        tiendasCreadas.Add(TiendaApps);
                    }
                }

                // 5. Guardar créditos
                foreach (var c in modelo.Cliente.Creditos)
                {
                    c.ClienteId = cliente.Id;
                    if (tiendasCreadas.Any())
                        c.TiendaAppId = tiendasCreadas.First().Id;
                    else
                        c.TiendaAppId = null;
                    await _creditoService.CreateCredito(c);
                }

                ////////

                if (UsuarioCreado.Id == 0)
                    throw new TaskCanceledException("No se pudo Crear");
                var query = await _UsuarioRepositorio.Consultar(u => u.Id == UsuarioCreado.Id);
                UsuarioCreado = query.Include(rol => rol.Rol).First();
                return _mapper.Map<UsuarioDTO>(UsuarioCreado);
            }
            catch
            {
                throw;
            }
        }
        public async Task<UsuarioDTO> crearUsuario(UsuarioDTO modelo)
        {
            try
            {
                // 🔥 PASO 1: VALIDAR TODO ANTES DE GUARDAR
                if (modelo.Cliente.TiendaApps != null && modelo.Cliente.TiendaApps.Any())
                {
                    // Validar que TODAS las tiendas existan
                    foreach (var t in modelo.Cliente.TiendaApps)
                    {
                        var tiendaExistente = await _context.Tiendas
                            .FirstOrDefaultAsync(x => x.CedulaEncargado == t.CedulaEncargado);

                        if (tiendaExistente == null)
                        {
                            // 🔥 Lanza error ANTES de guardar nada
                            throw new TaskCanceledException(
                                $"La tienda con cédula {t.CedulaEncargado} no existe. " +
                                "Primero debe registrarse la tienda."
                            );
                        }
                    }
                }

                // ✅ PASO 2: Si pasó todas las validaciones, ahora sí guardamos

                // 1. Encripta la contraseña
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(modelo.Clave);
                modelo.Clave = hashedPassword;
                var UsuarioCreado = await _UsuarioRepositorio.Crear(_mapper.Map<Usuario>(modelo));

                // 2. Guardar DetalleCliente
                var detalle = await _DetalleRepositorio.Crear(
                    _mapper.Map<DetalleCliente>(modelo.Cliente.DetalleCliente)
                );

                // 3. Guardar Cliente
                var cliente = new Cliente
                {
                    UsuarioId = UsuarioCreado.Id,
                    DetalleClienteID = detalle.Id
                };
                cliente = await _ClienteRepositorio.Crear(cliente);

                // 4. Crear TiendaApps (ya validamos que todas existen)
                var tiendasCreadas = new List<TiendaApp>();
                if (modelo.Cliente.TiendaApps != null)
                {
                    foreach (var t in modelo.Cliente.TiendaApps)
                    {
                        // Ya sabemos que existe, la buscamos de nuevo
                        var tiendaExistente = await _context.Tiendas
                            .FirstOrDefaultAsync(x => x.CedulaEncargado == t.CedulaEncargado);

                        var TiendaApps = new TiendaApp
                        {
                            TiendaId = tiendaExistente.Id,
                            ClienteId = cliente.Id,
                            CedulaEncargado = t.CedulaEncargado,
                            EstadoDeComision = "Pendiente",
                            FechaRegistro = DateTime.UtcNow
                        };

                        await _context.TiendaApps.AddAsync(TiendaApps);
                        await _context.SaveChangesAsync();
                        tiendasCreadas.Add(TiendaApps);
                    }
                }

                foreach (var c in modelo.Cliente.Creditos)
                {
                    c.ClienteId = cliente.Id;
                    if (tiendasCreadas.Any())
                        c.TiendaAppId = tiendasCreadas.First().Id;
                    else
                        c.TiendaAppId = null;
                    await _creditoService.CreateCredito(c);
                }


                return _mapper.Map<UsuarioDTO>(UsuarioCreado);
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> editarUsuario(UsuarioDTO modelo)
        {
            try
            {
                // Encripta la contraseña del modelo
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(modelo.Clave);
                // Actualiza la propiedad 'Clave' del modelo con la contraseña encriptada
                modelo.Clave = hashedPassword;

                var UsuarioModelo = _mapper.Map<Usuario>(modelo);

                var UsuarioEncontrado = await _UsuarioRepositorio.Obtener(u => u.Id == UsuarioModelo.Id);
                if (UsuarioEncontrado == null)
                    throw new TaskCanceledException("El usuario no existe");
                UsuarioEncontrado.NombreApellidos = UsuarioModelo.NombreApellidos;
                UsuarioEncontrado.Correo = UsuarioModelo.Correo;
                UsuarioEncontrado.RolId = UsuarioModelo.RolId;
                UsuarioEncontrado.Clave = UsuarioModelo.Clave;
                UsuarioEncontrado.EsActivo = UsuarioModelo.EsActivo;
                bool respuesta = await _UsuarioRepositorio.Editar(UsuarioEncontrado);
                return respuesta;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> eliminarUsuario(int id)
        {
            var usuario = await _context.Usuarios
        .Include(u => u.Cliente)
            .ThenInclude(c => c.DetalleCliente)
        .Include(u => u.Cliente)
            .ThenInclude(c => c.Creditos)
        .Include(u => u.Cliente)
            .ThenInclude(c => c.TiendaApps)
        .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return false;

            _context.Usuarios.Remove(usuario);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExisteCorreo(string correo)
        {
            return await _context.Usuarios
                                 .AnyAsync(u => u.Correo.ToLower() == correo.ToLower());
        }
    }
}
