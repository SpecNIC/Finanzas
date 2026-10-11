using Microsoft.VisualStudio.TestTools.UnitTesting;
using Software.Services;
using System;

namespace Software.Tests
{
    [TestClass]
    public class TestLogin_1
    {
        private readonly string connectionString =
            @"Data Source=localhost;Initial Catalog=software;Integrated Security=True;";
        

        [TestMethod]
        public void ValidarLogin_UsuarioRealYPasswordCorrecta_RetornaExito()
        {
            var service = new ServicioLogin(connectionString);

            var resultado = service.ValidarLogin("HLanuza", "2005Henry#");

            Assert.IsTrue(resultado.Exito);
            Assert.IsNotNull(resultado.UsuarioID);
            Assert.IsFalse(string.IsNullOrWhiteSpace(resultado.NombreUsuario));
        }

        [TestMethod]
        public void ValidarLogin_UsuarioRealYPasswordIncorrecta_RetornaError()
        {
            var service = new ServicioLogin(connectionString);

            var resultado = service.ValidarLogin("HLanuza", "2005");

            Assert.IsFalse(resultado.Exito);
            Assert.AreEqual("Usuario o contraseña incorrectos.", resultado.MensajeError);
        }

        [TestMethod]
        public void ValidarLogin_UsuarioNoExiste_RetornaError()
        {
            var service = new ServicioLogin(connectionString);

            var resultado = service.ValidarLogin("HLanu", "2005Henry#");

            Assert.IsFalse(resultado.Exito);
            Assert.AreEqual("Usuario o contraseña incorrectos.", resultado.MensajeError);
        }
    }
}
