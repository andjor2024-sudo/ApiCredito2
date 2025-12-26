using GestionIntApi.DTO;
using GestionIntApi.Repositorios.Interfaces;
using ClosedXML.Excel;
namespace GestionIntApi.Repositorios.Implementacion
{
    public class ReporteService : IReporteService
    {
        private readonly IClienteService _clienteRepository;

        public ReporteService(IClienteService clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        // =============================
        // ESTE YA LO TIENES
        // =============================
        public async Task<List<ReporteDTO>> ObtenerReporte(string fechaInicio,
    string fechaFin)
        {
            return await _clienteRepository.Reporte(fechaInicio, fechaFin);
            // ← aquí ya tienes tus joins cliente + tienda + crédito
        }

        // =============================
        // NUEVO: EXCEL
        // =============================
        public async Task<byte[]> ExportarReporteExcel(
    string fechaInicio,
    string fechaFin
)
        {
            var reporte = await ObtenerReporte(fechaInicio, fechaFin);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Reporte Créditos");

            int col = 1;


            ws.Cell(1, col++).Value = "ClienteId";
            ws.Cell(1, col++).Value = "Nombre Cliente";
            ws.Cell(1, col++).Value = "Cédula";
            ws.Cell(1, col++).Value = "Teléfono Cliente";
            ws.Cell(1, col++).Value = "Dirección Cliente";
           

            // TIENDA
            ws.Cell(1, col++).Value = "Tienda Id";
            ws.Cell(1, col++).Value = "Nombre Tienda";
            ws.Cell(1, col++).Value = "Encargado Tienda";
            ws.Cell(1, col++).Value = "Teléfono Tienda";

            // CRÉDITO
            ws.Cell(1, col++).Value = "Crédito Id";
            ws.Cell(1, col++).Value = "Entrada";
            ws.Cell(1, col++).Value = "Marca";
            ws.Cell(1, col++).Value = "Modelo";
            ws.Cell(1, col++).Value = "Monto Total";
            ws.Cell(1, col++).Value = "Monto Pendiente";
            ws.Cell(1, col++).Value = "Plazo Cuotas";
            ws.Cell(1, col++).Value = "Frecuencia Pago";
            ws.Cell(1, col++).Value = "Valor por Cuota";
            ws.Cell(1, col++).Value = "Estado Crédito";
            ws.Cell(1, col++).Value = "Estado de cuota";

            ws.Cell(1, col++).Value = "Abonado de cuota";
            ws.Cell(1, col++).Value = "Abonado Total";
            ws.Cell(1, col++).Value = "Próxima Cuota";
            // ARCHIVOS
            ws.Cell(1, col++).Value = "Foto Contrato";
            ws.Cell(1, col++).Value = "Foto Celular Entregado";
            ws.Cell(1, col++).Value = "Foto Cliente";
            // FECHAS
            ws.Cell(1, col++).Value = "Fecha Crédito";

            int fila = 2;

            foreach (var r in reporte)
            {
                col = 1;

                ws.Cell(fila, col++).Value = r.ClienteId;
                ws.Cell(fila, col++).Value = r.NombreCliente;
                ws.Cell(fila, col++).Value = r.Cedula;
                ws.Cell(fila, col++).Value = r.TelefonoCliente;
                ws.Cell(fila, col++).Value = r.DireccionCliente;
             

                // TIENDA
                ws.Cell(fila, col++).Value = r.TiendaId;
                ws.Cell(fila, col++).Value = r.NombreTienda;
                ws.Cell(fila, col++).Value = r.EncargadoTienda;
                ws.Cell(fila, col++).Value = r.TelefonoTienda;

                // CRÉDITO
                ws.Cell(fila, col++).Value = r.CreditoId;
                ws.Cell(fila, col++).Value = r.Entrada;
                ws.Cell(fila, col++).Value = r.Marca;
                ws.Cell(fila, col++).Value = r.Modelo;
                ws.Cell(fila, col++).Value = r.MontoTotal;
                ws.Cell(fila, col++).Value = r.MontoPendiente;
                ws.Cell(fila, col++).Value = r.PlazoCuotas;
                ws.Cell(fila, col++).Value = r.FrecuenciaPago;
                ws.Cell(fila, col++).Value = r.ValorPorCuota;
                ws.Cell(fila, col++).Value = r.EstadoCredito;
                ws.Cell(fila, col++).Value = r.EstadoCuota;
                ws.Cell(fila, col++).Value = r.AbonadoCuota;
                ws.Cell(fila, col++).Value = r.AbonadoTotal;

                ws.Cell(fila, col++).Value = r.ProximaCuota.ToString("dd/MM/yyyy");
                // ARCHIVOS
                ws.Cell(fila, col++).Value = r.FotoContrato;
                ws.Cell(fila, col++).Value = r.FotoCelularEntregadoUrl;
                ws.Cell(fila, col++).Value = r.FotoClienteUrl;
                // FECHA
                ws.Cell(fila, col++).Value = r.FechaCreditoStr;

                fila++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }



    }
}
