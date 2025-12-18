using AutoMapper;
using GestionIntApi.DTO;
using GestionIntApi.Models;
using GestionIntApi.Repositorios.Contrato;
using GestionIntApi.Repositorios.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace GestionIntApi.Repositorios.Implementacion
{
    public class CreditoService: ICreditoService
    {


        private readonly IGenericRepository<Credito> _creditoRepository;

        private readonly IMapper _mapper;
        private readonly SistemaGestionDBcontext _context;
        private readonly IHubContext<AdminHub> _hubContext;


        public CreditoService(IHubContext<AdminHub> hubContext, IGenericRepository<Credito> creditoRepository, IMapper mapper, SistemaGestionDBcontext context)
        {

            _creditoRepository = creditoRepository;
            _mapper = mapper;
            _context = context;
            _hubContext = hubContext;

        }



        public async Task<List<CreditoDTO>> GetAllTiendas()
        {
            try
            {
                var queryCredito = await _creditoRepository.Consultar();

                var CreditoCliente = queryCredito.ToList();
                // Recorremos la lista de usuarios y reemplazamos el hash de la contraseña por el texto plano
                return _mapper.Map<List<CreditoDTO>>(CreditoCliente);
            }
            catch
            {

                throw;
            }



        }
        public async Task<CreditoDTO> GetTiendaById(int id)
        {
            try
            {
                var creditoEncontrado = await _creditoRepository.Obtener(u => u.Id == id);


                if (creditoEncontrado == null)
                    throw new TaskCanceledException("Credito de cliente no encontrado");
                return _mapper.Map<CreditoDTO>(creditoEncontrado);
            }
            catch
            {
                throw;
            }
        }



        public async Task<CreditoDTO> CreateCredito(CreditoDTO modelo)
        {
            try
            {

                // =============================
                // 1. VALIDAR SI CLIENTE TIENE CRÉDITO ACTIVO
                // =============================

                var creditoActual = await _creditoRepository.Obtener(
                    c => c.ClienteId == modelo.ClienteId && c.MontoPendiente > 0
                );

                if (creditoActual != null)
                {
                    throw new Exception("El cliente aún tiene un crédito activo pendiente. Debe saldarlo antes de crear uno nuevo.");
                }




                // Validaciones básicas
                if (modelo.MontoTotal <= 0)
                    throw new ArgumentException("El monto total debe ser mayor a cero.");

                if (modelo.Entrada < 0)
                    throw new ArgumentException("La entrada no puede ser negativa.");

                if (modelo.Entrada > modelo.MontoTotal)
                    throw new ArgumentException("La entrada no puede ser mayor al monto total del celular.");

                if (modelo.PlazoCuotas <= 0)
                    throw new ArgumentException("El plazo de cuotas debe ser mayor a cero.");

                // Cálculo del TotalPagar (sin intereses)
                modelo.MontoPendiente = modelo.MontoTotal-modelo.Entrada;

                int totalCuotas = modelo.FrecuenciaPago.ToLower() switch
                {
                    "semanal" => modelo.PlazoCuotas * 4,
                    "quincenal" => modelo.PlazoCuotas * 2,
                    "mensual" => modelo.PlazoCuotas,
                    _ => throw new Exception("Frecuencia de pago inválida")
                };

                // =============================
                // 5. VALOR POR CUOTA REAL
                // =============================
                modelo.ValorPorCuota = Math.Round(
                    modelo.MontoPendiente / totalCuotas, 2
                );
                // Cálculo de ValorPorCuota
              //  modelo.ValorPorCuota = modelo.MontoPendiente / modelo.PlazoCuotas;

                // Cálculo de PróximaCuota según frecuencia
                modelo.ProximaCuota = modelo.FrecuenciaPago.ToLower() switch
                {
                    "semanal" => modelo.DiaPago.AddDays(7),
                    "quincenal" => modelo.DiaPago.AddDays(15),
                    "mensual" => modelo.DiaPago.AddMonths(1),
                    _ => modelo.DiaPago
                };

                // =============================
              

                // =============================
                // 5. Actualizar estado
                // =============================
                modelo.Estado = modelo.MontoPendiente <= 0 ? "Pagado" : "Pendiente";


                var UsuarioCreado = await _creditoRepository.Crear(_mapper.Map<Credito>(modelo));

                if (UsuarioCreado.Id == 0)
                    throw new TaskCanceledException("No se pudo crear la tienda");

                return _mapper.Map<CreditoDTO>(UsuarioCreado);
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> UpdateCredito(CreditoDTO modelo)
        {
            try
            {


                var TiendaModelo = _mapper.Map<CreditoDTO>(modelo);

                var TiendaEncontrado = await _creditoRepository.Obtener(u => u.Id == TiendaModelo.Id);
                if (TiendaEncontrado == null)
                    throw new TaskCanceledException("El credito no existe");

                // Recalcular saldo pendiente y valor por cuota
                TiendaEncontrado.MontoPendiente = modelo.MontoTotal - modelo.Entrada;
                TiendaEncontrado.ValorPorCuota = TiendaEncontrado.MontoPendiente / modelo.PlazoCuotas;

                modelo.ProximaCuota = modelo.FrecuenciaPago.ToLower() switch
                {
                    "semanal" => modelo.DiaPago.AddDays(7),
                    "quincenal" => modelo.DiaPago.AddDays(15),
                    "mensual" => modelo.DiaPago.AddMonths(1),
                    _ => modelo.DiaPago
                };

                // Actualizar los campos editables
                TiendaEncontrado.MontoTotal = modelo.MontoTotal;
                TiendaEncontrado.Entrada = modelo.Entrada;
                TiendaEncontrado.PlazoCuotas = modelo.PlazoCuotas;
                TiendaEncontrado.FrecuenciaPago = modelo.FrecuenciaPago;
                TiendaEncontrado.DiaPago = modelo.DiaPago;

                // 🔥 Recalcular correctamente
                TiendaEncontrado.MontoPendiente = TiendaEncontrado.MontoTotal - TiendaEncontrado.Entrada;

                TiendaEncontrado.ValorPorCuota = TiendaEncontrado.PlazoCuotas > 0
                    ? TiendaEncontrado.MontoPendiente / TiendaEncontrado.PlazoCuotas
                    : 0;

                TiendaEncontrado.ProximaCuota = modelo.FrecuenciaPago.ToLower() switch
                {
                    "semanal" => TiendaEncontrado.DiaPago.AddDays(7),
                    "quincenal" => TiendaEncontrado.DiaPago.AddDays(15),
                    "mensual" => TiendaEncontrado.DiaPago.AddMonths(1),
                    _ => TiendaEncontrado.DiaPago
                };

                bool respuesta = await _creditoRepository.Editar(TiendaEncontrado);
                return respuesta;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DeleteCredito(int id)
        {
            try
            {
                var tiendaEncontrado = await _creditoRepository.Obtener(u => u.Id == id);
                if (tiendaEncontrado == null)
                    throw new TaskCanceledException("Tienda no existe");
                bool respuesta = await _creditoRepository.Eliminar(tiendaEncontrado);
                return respuesta;
            }
            catch
            {
                throw;
            }



        }
        public async Task<CreditoDTO> RegistrarPagoAsync(PagoCreditoDTO pago)
        {
            // 1. Buscar el crédito por Id
            var credito = await _creditoRepository.Obtener(u => u.Id == pago.CreditoId);

            if (credito == null)
                throw new Exception("Crédito no encontrado");

            // =============================
            // 2. Normalizar fechas a UTC
            // =============================
            credito.ProximaCuota = DateTime.SpecifyKind(credito.ProximaCuota, DateTimeKind.Utc);
            credito.DiaPago = DateTime.SpecifyKind(credito.DiaPago, DateTimeKind.Utc);
            credito.FechaCreacion = DateTime.SpecifyKind(credito.FechaCreacion, DateTimeKind.Utc);

            // =============================
            // 3. Restar monto pagado 
            // =============================
            if (pago.MontoPagado <= 0)
                throw new Exception("El monto pagado debe ser mayor a 0");

            credito.MontoPendiente -= pago.MontoPagado;

            // if (credito.MontoPendiente < 0)
            //   credito.MontoPendiente = 0; // evitar negativo


            // =============================
            // Ajustes si ya se pagó completo
            // =============================
            if (credito.MontoPendiente <= 0)
            {
                credito.MontoPendiente = 0;
                credito.ValorPorCuota = 0;
                credito.ProximaCuota = credito.DiaPago; // fecha del último pago
                credito.Estado = "Pagado";
            }
            else {



                // =============================
                // 4. Calcular Próxima Cuota
                // =============================

                // Si la fecha viene vacía o inválida → corregimos
                if (credito.ProximaCuota.Year < 2000)
                    credito.ProximaCuota = DateTime.UtcNow;



                switch (credito.FrecuenciaPago.ToLower())
                {
                    case "semanal":
                        credito.ProximaCuota = credito.ProximaCuota.AddDays(7);
                        break;

                    case "quincenal":
                        credito.ProximaCuota = credito.ProximaCuota.AddDays(15);
                        break;

                    case "mensual":
                        credito.ProximaCuota = credito.ProximaCuota.AddMonths(1);
                        break;
                }

                // Asegurar Kind = UTC
                credito.ProximaCuota = DateTime.SpecifyKind(credito.ProximaCuota, DateTimeKind.Utc);





            }

            // =============================
            // 5. Actualizar estado
            // =============================
            credito.Estado = credito.MontoPendiente <= 0 ? "Pagado" : "Pendiente";


            // =============================
            // 6. Guardar cambios en BD
            // =============================
            await _creditoRepository.Editar(credito);
            var dto = _mapper.Map<CreditoDTO>(credito);

            await _hubContext.Clients.All.SendAsync("CreditoActualizado", dto);
            Console.WriteLine("✅ SignalR SendAsync ejecutado");
            // =============================
            // 7. Mapear y devolver DTO
            // =============================
            return dto;
        }




        public async Task<List<CreditoDTO>> GetCreditosPendientesPorCliente(int clienteId)
        {
            try
            {
                // Consultamos solo los créditos del cliente
                var query = await _creditoRepository.Consultar(c => c.ClienteId == clienteId);

                // Filtramos solamente los pendientes
                var pendientes = query
                    .Where(c => c.MontoPendiente > 0)
                    .ToList();

                // Retornamos en DTO
                return _mapper.Map<List<CreditoDTO>>(pendientes);
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<CreditoMostrarDTO>> GetCreditosPendientesPorCliente1(int clienteId)
        {
            try
            {
                // Consultamos solo los créditos del cliente
                var query = await _creditoRepository.Consultar(c => c.ClienteId == clienteId);

                // Filtramos solamente los pendientes
                var pendientes = query
                    .Where(c => c.MontoPendiente > 0)
                    .ToList();

                // Retornamos en DTO
                return _mapper.Map<List<CreditoMostrarDTO>>(pendientes);
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<CreditoMostrarDTO>> GetCreditosClienteApp(int clienteId)
        {
            try
            {
                // Consultamos solo los créditos del cliente
                var query = await _creditoRepository.Consultar(c => c.ClienteId == clienteId);

                // Filtramos solamente los pendientes
                var pendientes = query
                    .Where(c => c.MontoPendiente > 0)
                    .ToList();

                // Retornamos en DTO
                return _mapper.Map<List<CreditoMostrarDTO>>(pendientes);
            }
            catch
            {
                throw;
            }
        }
    }



}

