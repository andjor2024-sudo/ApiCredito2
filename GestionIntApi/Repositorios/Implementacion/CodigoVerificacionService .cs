using GestionIntApi.Models;
using GestionIntApi.Repositorios.Interfaces;

namespace GestionIntApi.Repositorios.Implementacion
{
    public class CodigoVerificacionService : ICodigoVerificacionService
    {

        private readonly Dictionary<string, VerificationCode> _codigos = new();

        public void GuardarCodigo(string correo, string codigo)
        {
            _codigos[correo] = new VerificationCode
            {
                Correo = correo,
                Codigo = codigo,
                Expira = DateTime.Now.AddMinutes(5)
            };
        }

        public bool ValidarCodigo(string correo, string codigo)
        {
            if (!_codigos.ContainsKey(correo))
                return false;

            var data = _codigos[correo];

            if (data.Expira < DateTime.Now)
                return false;

            return data.Codigo == codigo;
        }
    }
}
