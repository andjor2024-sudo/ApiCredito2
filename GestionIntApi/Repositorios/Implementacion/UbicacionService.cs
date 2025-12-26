using AutoMapper;
using GestionIntApi.DTO;
using GestionIntApi.Models;
using GestionIntApi.Repositorios.Contrato;
using GestionIntApi.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;

namespace GestionIntApi.Servicios.Implementacion
{
    public class UbicacionService : IUbicacionService
    {
        private readonly IUbicacionService _ubicacionRepository;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Ubicacion> _clienteRepository2;

        public UbicacionService( IGenericRepository<Ubicacion> clienteRepository2, IMapper mapper)
        {
            
            _mapper = mapper;
            _clienteRepository2 = clienteRepository2;

        }

        public async Task<UbicacionDTO> Registrar(double latitud, double longitud, int usuarioId)
        {
            try
            {
                var ubicacion = new Ubicacion
                {
                    Latitud = latitud,
                    Longitud = longitud,
                    UsuarioId = usuarioId,
                    Fecha = DateTime.UtcNow
                };

                var ubicacionCreada = await _clienteRepository2.Crear(ubicacion);

                if (ubicacionCreada.Id == 0)
                    throw new TaskCanceledException("No se pudo registrar la ubicación");

                return _mapper.Map<UbicacionDTO>(ubicacionCreada);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar ubicación: {ex.Message}");
            }
        }

        public async Task<List<UbicacionDTO>> ObtenerPorUsuario(int usuarioId)
        {
            try
            {
                var query = await _clienteRepository2.Consultar(n => n.UsuarioId == usuarioId);
                var lista = await query.OrderByDescending(n => n.Fecha).ToListAsync();

                // Mapear a DTO
                return _mapper.Map<List<UbicacionDTO>>(lista);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener ubicaciones del usuario {usuarioId}: {ex.Message}");
            }
        }

        public async Task<UbicacionDTO> ObtenerUltima(int usuarioId)
        {
            try
            {
                var query = await _clienteRepository2.Consultar(n => n.UsuarioId == usuarioId);
                var ultima = await query.OrderByDescending(n => n.Fecha).FirstOrDefaultAsync();

                return _mapper.Map<UbicacionDTO>(ultima);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la última ubicación del usuario {usuarioId}: {ex.Message}");
            }
        }
    }
}