using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Software.Services
{
    public class LineaAsiento
    {
        public int CuentaID { get; set; }
        public string NombreCuenta { get; set; }
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
    }

    public class ResultadoValidacionAsiento
    {
        public bool EsValido { get; set; }
        public string Mensaje { get; set; }
    }

    public class ServicioAsientoContable
    {
        public ResultadoValidacionAsiento ValidarLinea(int cuentaId, decimal debe, decimal haber)
        {
            if (cuentaId <= 0)
                return new ResultadoValidacionAsiento { EsValido = false, Mensaje = "Debe seleccionar una cuenta contable." };

            if (debe < 0 || haber < 0)
                return new ResultadoValidacionAsiento { EsValido = false, Mensaje = "Los montos no pueden ser negativos." };

            if (debe == 0 && haber == 0)
                return new ResultadoValidacionAsiento { EsValido = false, Mensaje = "Debe ingresar un monto en Debe o en Haber." };

            if (debe > 0 && haber > 0)
                return new ResultadoValidacionAsiento { EsValido = false, Mensaje = "No puede ingresar un monto en Debe y Haber en la misma línea." };

            return new ResultadoValidacionAsiento { EsValido = true };
        }

        public decimal CalcularTotalDebe(List<LineaAsiento> lineas)
        {
            return lineas.Sum(x => x.Debe);
        }

        public decimal CalcularTotalHaber(List<LineaAsiento> lineas)
        {
            return lineas.Sum(x => x.Haber);
        }

        public ResultadoValidacionAsiento ValidarAsientoCompleto(DateTime? fecha, string descripcion, List<LineaAsiento> lineas)
        {
            if (fecha == null)
                return new ResultadoValidacionAsiento { EsValido = false, Mensaje = "La fecha del asiento es obligatoria." };

            if (string.IsNullOrWhiteSpace(descripcion))
                return new ResultadoValidacionAsiento { EsValido = false, Mensaje = "La descripción del asiento es obligatoria." };

            if (lineas == null || lineas.Count < 2)
                return new ResultadoValidacionAsiento { EsValido = false, Mensaje = "Un asiento debe tener al menos dos líneas (Debe y Haber)." };

            decimal totalDebe = CalcularTotalDebe(lineas);
            decimal totalHaber = CalcularTotalHaber(lineas);

            if (totalDebe != totalHaber)
                return new ResultadoValidacionAsiento { EsValido = false, Mensaje = "El asiento no está cuadrado." };

            if (totalDebe <= 0)
                return new ResultadoValidacionAsiento { EsValido = false, Mensaje = "El asiento debe tener monto mayor que cero." };

            return new ResultadoValidacionAsiento { EsValido = true };
        }
    }
}
