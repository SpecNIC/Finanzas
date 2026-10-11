using Microsoft.VisualStudio.TestTools.UnitTesting;
using Software.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Software.Tests
{
    //Luego, en el archivo ".aspx.cs" se deberían reemplazar las validaciones por las de este archivo
    [TestClass]
    public class TestAsientoContables_2
    {
        [TestMethod]
        public void ValidarLinea_SinCuenta_RetornaError()
        {
            var service = new ServicioAsientoContable();

            var resultado = service.ValidarLinea(0, 100, 0);

            Assert.IsFalse(resultado.EsValido);
            Assert.AreEqual("Debe seleccionar una cuenta contable.", resultado.Mensaje);
        }

        [TestMethod]
        public void ValidarLinea_MontoNegativo_RetornaError()
        {
            var service = new ServicioAsientoContable();

            var resultado = service.ValidarLinea(1, -100, 0);

            Assert.IsFalse(resultado.EsValido);
            Assert.AreEqual("Los montos no pueden ser negativos.", resultado.Mensaje);
        }

        [TestMethod]
        public void ValidarLinea_DebeYHaberEnLaMismaLinea_RetornaError()
        {
            var service = new ServicioAsientoContable();

            var resultado = service.ValidarLinea(1, 100, 100);

            Assert.IsFalse(resultado.EsValido);
            Assert.AreEqual("No puede ingresar un monto en Debe y Haber en la misma línea.", resultado.Mensaje);
        }

        [TestMethod]
        public void CalcularTotales_LineasValidas_RetornaTotalesCorrectos()
        {
            var service = new ServicioAsientoContable();

            var lineas = new List<LineaAsiento>
            {
                new LineaAsiento { CuentaID = 1, Debe = 500, Haber = 0 },
                new LineaAsiento { CuentaID = 2, Debe = 0, Haber = 500 }
            };

            Assert.AreEqual(500, service.CalcularTotalDebe(lineas));
            Assert.AreEqual(500, service.CalcularTotalHaber(lineas));
        }

        [TestMethod]
        public void ValidarAsientoCompleto_AsientoCuadrado_RetornaValido()
        {
            var service = new ServicioAsientoContable();

            var lineas = new List<LineaAsiento>
            {
                new LineaAsiento { CuentaID = 1, Debe = 1000, Haber = 0 },
                new LineaAsiento { CuentaID = 2, Debe = 0, Haber = 1000 }
            };

            var resultado = service.ValidarAsientoCompleto(
                DateTime.Today,
                "Compra de equipo",
                lineas
            );

            Assert.IsTrue(resultado.EsValido);
        }

        [TestMethod]
        public void ValidarAsientoCompleto_AsientoDescuadrado_RetornaError()
        {
            var service = new ServicioAsientoContable();

            var lineas = new List<LineaAsiento>
            {
                new LineaAsiento { CuentaID = 1, Debe = 1000, Haber = 0 },
                new LineaAsiento { CuentaID = 2, Debe = 0, Haber = 800 }
            };

            var resultado = service.ValidarAsientoCompleto(
                DateTime.Today,
                "Compra de equipo",
                lineas
            );

            Assert.IsFalse(resultado.EsValido);
            Assert.AreEqual("El asiento no está cuadrado.", resultado.Mensaje);
        }

        [TestMethod]
        public void ValidarAsientoCompleto_AsientoSinDescripcion_RetornaError()
        {
            var service = new ServicioAsientoContable();

            var lineas = new List<LineaAsiento>
            {
                new LineaAsiento { CuentaID = 1, Debe = 1000, Haber = 0 },
                new LineaAsiento { CuentaID = 2, Debe = 0, Haber = 1000 }
            };

            var resultado = service.ValidarAsientoCompleto(
                DateTime.Today,
                "",
                lineas
            );

            Assert.IsFalse(resultado.EsValido);
        }

        [TestMethod]
        public void ValidarAsientoCompleto_AsientoSinLineas_RetornaError()
        {
            var service = new ServicioAsientoContable();

            var lineas = new List<LineaAsiento>
            {
                new LineaAsiento { CuentaID = 1, Debe = 1000, Haber = 0 }
            };

            var resultado = service.ValidarAsientoCompleto(
                DateTime.Today,
                "Compra de equipo",
                lineas
            );

            Assert.IsFalse(resultado.EsValido);
        }
    }
}
