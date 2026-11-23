using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.implementaciones;
using aplicacion_tiendaFajas.interfaces;
using Microsoft.EntityFrameworkCore;
using pruebas_tiendaFajas.Nucleo;

namespace pruebas_tiendaFajas.Repositorios
{
    /// <summary>
    /// Prueba del repositorio de Imagenes.
    ///
    /// Es la unica tabla con una columna binaria, asi que ademas del ciclo
    /// normal comprueba tres cosas propias de ese caso: que los bytes vuelvan
    /// de la base identicos a los que se guardaron, que la bitacora no
    /// registre el binario, y que una proyeccion sin la columna devuelva la
    /// ficha sin arrastrarlo.
    /// </summary>
    [TestClass]
    public class ImagenesPrueba
    {
        private readonly IConexion conexion;
        private readonly EntidadesNucleo nucleo;
        private List<Imagenes>? lista;
        private Imagenes? entidad;

        public ImagenesPrueba()
        {
            this.conexion = new Conexion();
            this.nucleo = new EntidadesNucleo(this.conexion, "Imagenes");
        }

        [TestMethod]
        public void Ejecutar()
        {
            this.nucleo.Iniciar();

            try
            {
                Assert.IsTrue(Guardar(), "no inserto Imagenes");
                Assert.IsTrue(Listar(), "no listo Imagenes");
                Assert.IsTrue(Buscar(), "no encontro la Imagenes insertada");
                Assert.IsTrue(ElBinarioVuelveIntacto(), "el binario volvio distinto");
                Assert.IsTrue(Modificar(), "no actualizo Imagenes");
                Assert.IsTrue(Auditado(), "no quedo rastro de auditoria de Imagenes");
                Assert.IsTrue(LaBitacoraNoGuardaElBinario(),
                    "la bitacora esta registrando el contenido del archivo");
                Assert.IsTrue(Borrar(), "no borro Imagenes");
            }
            finally
            {
                this.nucleo.Limpiar();
            }
        }

        public bool Guardar()
        {
            this.entidad = this.nucleo.Imagenes();

            this.conexion.Imagenes!.Add(this.entidad);
            this.conexion.SaveChanges();
            this.nucleo.Vigilar(this.conexion.Imagenes!, this.entidad);

            return this.entidad.Id > 0;
        }

        public bool Listar()
        {
            this.lista = this.conexion.Imagenes!.ToList();
            return this.lista.Count > 0;
        }

        public bool Buscar()
        {
            var llave = this.entidad!.Id;
            return this.conexion.Imagenes!.FirstOrDefault(x => x.Id == llave) != null;
        }

        /// <summary>
        /// Un VARBINARY(MAX) tiene que volver byte por byte. Si volviera
        /// truncado o con la codificacion cambiada, la imagen no se abriria en
        /// el navegador y el error seria dificil de rastrear.
        /// </summary>
        public bool ElBinarioVuelveIntacto()
        {
            var llave = this.entidad!.Id;

            var leida = this.conexion.Imagenes!.FirstOrDefault(x => x.Id == llave);
            if (leida == null || leida.Contenido == null)
                return false;

            if (leida.Tamano != EntidadesNucleo.Png.Length)
                return false;

            return leida.Contenido.SequenceEqual(EntidadesNucleo.Png);
        }

        public bool Modificar()
        {
            var llave = this.entidad!.Id;
            this.entidad!.Nombre = "renombrada.png";

            var entry = this.conexion.Entry<Imagenes>(this.entidad);
            entry.State = EntityState.Modified;
            this.conexion.SaveChanges();

            var leida = this.conexion.Imagenes!.FirstOrDefault(x => x.Id == llave);
            return leida != null && leida.Nombre == "renombrada.png";
        }

        public bool Auditado()
        {
            var llave = this.entidad!.Id;
            var etiqueta = this.nucleo.Etiqueta;

            return this.conexion.Auditorias!.Any(x =>
                x.Entidad == "Imagenes" &&
                x.Llave == llave &&
                x.Operacion == "Insertar" &&
                x.Usuario == etiqueta);
        }

        /// <summary>
        /// La columna Datos de la bitacora se arma recorriendo las propiedades
        /// de la entidad. Un binario no cabe ahi y no dice nada legible, asi
        /// que Conexion salta las columnas de tipo byte[]. El tamaño del
        /// archivo ya queda registrado en su propia columna.
        /// </summary>
        public bool LaBitacoraNoGuardaElBinario()
        {
            var llave = this.entidad!.Id;
            var etiqueta = this.nucleo.Etiqueta;

            var datos = this.conexion.Auditorias!
                .Where(x => x.Entidad == "Imagenes"
                    && x.Llave == llave
                    && x.Usuario == etiqueta)
                .Select(x => x.Datos)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(datos))
                return false;

            // ni el nombre de la columna ni ningun rastro del arreglo
            return !datos!.Contains("Contenido")
                && !datos.Contains("Byte[]")
                && datos.Contains("Tipo");
        }

        public bool Borrar()
        {
            var llave = this.entidad!.Id;

            this.conexion.Imagenes!.Remove(this.entidad!);
            this.conexion.SaveChanges();
            this.nucleo.Olvidar();

            return this.conexion.Imagenes!.FirstOrDefault(x => x.Id == llave) == null;
        }
    }
}
