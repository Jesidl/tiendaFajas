using aplicacion_tiendaFajas.entidades;
using aplicacion_tiendaFajas.interfaces;
using aplicacion_tiendaFajas.nucleo;
using Microsoft.EntityFrameworkCore;

namespace pruebas_tiendaFajas.Nucleo
{
    /// <summary>
    /// Fabrica de entidades para las pruebas de repositorio.
    ///
    /// Resuelve tres problemas que no se pueden resolver dentro de cada prueba:
    ///
    /// 1. Llaves foraneas. Veinte de las veinticuatro tablas apuntan a otras.
    ///    Una prueba de Detalles_Ventas necesita una Venta, que necesita un
    ///    Cliente y un Empleado, que necesita una Sucursal y un Cargo. Aqui
    ///    cada fabrica crea y guarda su propia cadena de padres, asi que
    ///    ninguna prueba depende de los datos que ya haya en la base ni de que
    ///    otra prueba haya corrido antes.
    ///
    /// 2. Limpieza. Toda fila creada se apila junto con la accion que la borra.
    ///    Limpiar() las deshace en orden inverso, es decir hijos antes que
    ///    padres, que es el unico orden que no viola una llave foranea.
    ///
    /// 3. Aislamiento entre pruebas en paralelo. Cada prueba marca sus
    ///    escrituras con una etiqueta propia en ContextoUsuario, de modo que
    ///    pueda reconocer y borrar exactamente sus propios registros de
    ///    auditoria sin tocar los de otra prueba que corra al mismo tiempo.
    ///
    /// Valores: cada campo con restriccion UNIQUE se llena con Unico(), y
    /// ningun texto generado excede el NVARCHAR declarado en Script.sql.
    /// </summary>
    public class EntidadesNucleo
    {
        private readonly IConexion conexion;
        private readonly Stack<Action> deshacer = new Stack<Action>();
        private static int secuencia = 0;

        /// <summary>
        /// Marca que queda en la columna Usuario de cada registro de auditoria
        /// escrito por esta prueba. Es la llave con la que Limpiar() encuentra
        /// sus propios rastros.
        /// </summary>
        public string Etiqueta { get; private set; }

        public EntidadesNucleo(IConexion conexion, string etiqueta)
        {
            this.conexion = conexion;
            this.Etiqueta = "prueba:" + etiqueta;
        }

        /// <summary>
        /// Se llama al comienzo del metodo de prueba y no en el constructor:
        /// ContextoUsuario usa AsyncLocal, y el valor solo fluye hacia las
        /// llamadas que se hacen despues de asignarlo dentro del mismo flujo.
        /// MSTest construye la clase y luego invoca el metodo, y no garantiza
        /// que el contexto de ejecucion del constructor llegue al metodo.
        /// </summary>
        public void Iniciar()
        {
            ContextoUsuario.Usuario = this.Etiqueta;
        }

        /// <summary>
        /// Texto irrepetible para las columnas UNIQUE, recortado a la longitud
        /// que declara la tabla. El contador es estatico e incrementado con
        /// Interlocked, porque las pruebas corren en paralelo y la hora sola
        /// no alcanza para distinguirlas.
        /// </summary>
        public static string Unico(string prefijo, int largo)
        {
            var numero = Interlocked.Increment(ref secuencia);
            var cuerpo = DateTime.Now.ToString("HHmmssfff") + "-" + numero.ToString();
            var texto = prefijo + cuerpo;

            if (texto.Length <= largo)
                return texto;

            // se recorta por la izquierda: los digitos de la derecha son los
            // que mas varian, asi que son los que no se pueden perder
            var sobra = texto.Length - largo;
            if (sobra >= cuerpo.Length)
                throw new Exception("El prefijo '" + prefijo + "' no cabe en " + largo);

            return prefijo + cuerpo.Substring(sobra);
        }

        // ------------------------------------------------------------------
        // Ciclo de vida de las filas
        // ------------------------------------------------------------------

        /// <summary>
        /// Guarda una fila padre y registra como deshacerla.
        /// </summary>
        private T Guardar<T>(DbSet<T> conjunto, T fila) where T : class
        {
            conjunto.Add(fila);
            this.conexion.SaveChanges();
            Vigilar(conjunto, fila);
            return fila;
        }

        /// <summary>
        /// La prueba inserta su propia fila, asi que la registra aqui para que
        /// Limpiar() se la lleve si la prueba falla antes de borrarla.
        /// </summary>
        public void Vigilar<T>(DbSet<T> conjunto, T fila) where T : class
        {
            this.deshacer.Push(() =>
            {
                conjunto.Remove(fila);
                this.conexion.SaveChanges();
            });
        }

        /// <summary>
        /// La prueba ya borro la fila que acababa de insertar, asi que su
        /// accion de deshacer sobra. Es siempre la ultima apilada.
        /// </summary>
        public void Olvidar()
        {
            if (this.deshacer.Count > 0)
                this.deshacer.Pop();
        }

        /// <summary>
        /// Deshace todo lo creado, en orden inverso. No lanza: se llama desde
        /// un finally, y una excepcion aqui taparia la falla real de la
        /// prueba. Lo que no se pudo borrar queda reportado en la salida.
        /// </summary>
        public void Limpiar()
        {
            while (this.deshacer.Count > 0)
            {
                var accion = this.deshacer.Pop();
                try
                {
                    accion();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[" + this.Etiqueta +
                        "] no se pudo deshacer una fila: " + ex.Message);
                }
            }

            try
            {
                LimpiarAuditorias();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[" + this.Etiqueta +
                    "] no se pudo limpiar la auditoria: " + ex.Message);
            }
        }

        /// <summary>
        /// Borra los rastros de auditoria que dejo esta prueba. Se hace al
        /// final, porque borrar las filas de arriba tambien escribe auditoria.
        /// Filtra por la etiqueta propia, no por fecha ni por tabla, para no
        /// tocar los rastros de otra prueba que corra en paralelo ni los de la
        /// aplicacion real.
        /// </summary>
        private void LimpiarAuditorias()
        {
            var etiqueta = this.Etiqueta;

            var rastros = this.conexion.Auditorias!
                .Where(x => x.Usuario == etiqueta)
                .ToList();

            if (rastros.Count == 0)
                return;

            this.conexion.Auditorias!.RemoveRange(rastros);
            this.conexion.SaveChanges();
        }

        // ------------------------------------------------------------------
        // Tablas sin llaves foraneas
        // ------------------------------------------------------------------

        public Cargos Cargos()
        {
            return new Cargos
            {
                Nombre = Unico("Cargo ", 100),
                Descripcion = "cargo creado por las pruebas",
                Salario = 1450000.00m
            };
        }

        public Sucursales Sucursales()
        {
            return new Sucursales
            {
                Nombre = Unico("Sucursal ", 100),
                Direccion = "Calle 52 # 49-30",
                Ciudad = "Medellin",
                Telefono = Unico("604", 20),
                Estado = true
            };
        }

        public Clientes Clientes()
        {
            return new Clientes
            {
                Cedula = Unico("C", 50),
                Nombre = Unico("Cliente ", 200),
                Telefono = Unico("300", 20),
                Correo = Unico("cli", 20) + "@prueba.com",
                Direccion = "Calle 10 # 20-30"
            };
        }

        public Proveedores Proveedores()
        {
            return new Proveedores
            {
                Nit = Unico("N", 50),
                Nombre_Empresa = Unico("Proveedor ", 200),
                Telefono = Unico("604", 20),
                Correo = Unico("prov", 20) + "@prueba.com"
            };
        }

        public Categorias Categorias()
        {
            return new Categorias
            {
                Nombre = Unico("Categoria ", 100),
                Descripcion = "categoria creada por las pruebas",
                Estado = true
            };
        }

        public Marcas Marcas()
        {
            return new Marcas
            {
                Nombre = Unico("Marca ", 100),
                Pais = "Colombia",
                Estado = true
            };
        }

        public Materiales Materiales()
        {
            return new Materiales
            {
                Nombre = Unico("Material ", 100),
                Descripcion = "material creado por las pruebas"
            };
        }

        public Tallas Tallas()
        {
            // Nombre es NVARCHAR(10): el texto se recorta a ese limite
            return new Tallas
            {
                Nombre = Unico("T", 10),
                Descripcion = "talla creada por las pruebas"
            };
        }

        public Colores Colores()
        {
            return new Colores
            {
                Nombre = Unico("Color ", 50),
                Codigo_Hex = "#F5F5DC"
            };
        }

        public Metodos_Pago Metodos_Pago()
        {
            return new Metodos_Pago
            {
                Nombre = Unico("Metodo ", 50),
                Descripcion = "metodo creado por las pruebas",
                Estado = true
            };
        }

        public Roles Roles()
        {
            return new Roles
            {
                Nombre = Unico("Rol ", 50),
                Descripcion = "rol creado por las pruebas",
                Estado = true
            };
        }

        public Auditorias Auditorias()
        {
            return new Auditorias
            {
                Entidad = "Pruebas",
                Llave = 0,
                Operacion = "Insertar",
                Usuario = this.Etiqueta,
                Fecha = DateTime.Now,
                Datos = "fila escrita a mano por la prueba de repositorio"
            };
        }

        // ------------------------------------------------------------------
        // Tablas con una o mas llaves foraneas. Cada fabrica crea y guarda
        // sus propios padres, para no depender de los datos de la base.
        // ------------------------------------------------------------------

        public Empleados Empleados()
        {
            var sucursal = Guardar(this.conexion.Sucursales!, Sucursales());
            var cargo = Guardar(this.conexion.Cargos!, Cargos());

            return new Empleados
            {
                Cedula = Unico("E", 50),
                Nombre = Unico("Empleado ", 200),
                Telefono = Unico("310", 20),
                Sucursal = sucursal.Id,
                Cargo = cargo.Id
            };
        }

        public Medidas Medidas()
        {
            var cliente = Guardar(this.conexion.Clientes!, Clientes());

            return new Medidas
            {
                Cliente = cliente.Id,
                Cintura = 82.00m,
                Cadera = 98.00m,
                Busto = 90.00m,
                Fecha = DateTime.Now
            };
        }

        public Productos Productos()
        {
            var categoria = Guardar(this.conexion.Categorias!, Categorias());
            var marca = Guardar(this.conexion.Marcas!, Marcas());
            var material = Guardar(this.conexion.Materiales!, Materiales());

            return new Productos
            {
                Codigo = Unico("PRD-", 50),
                Nombre = Unico("Producto ", 200),
                Descripcion = "producto creado por las pruebas",
                Precio = 280000.00m,
                Estado = true,
                Categoria = categoria.Id,
                Marca = marca.Id,
                Material = material.Id
            };
        }

        public Usuarios Usuarios()
        {
            var rol = Guardar(this.conexion.Roles!, Roles());

            // la clave se deriva aqui tambien: una prueba de repositorio
            // escribe directo en la tabla, sin pasar por UsuariosAplicacion,
            // y la tabla no debe quedar nunca con una clave en texto plano
            return new Usuarios
            {
                Correo = Unico("usr", 20) + "@prueba.com",
                Nombre = Unico("Usuario ", 200),
                Clave = Clave.Derivar("Prueba.123"),
                Rol = rol.Id,
                Estado = true
            };
        }

        public Permisos Permisos()
        {
            var rol = Guardar(this.conexion.Roles!, Roles());

            return new Permisos
            {
                Rol = rol.Id,
                Modulo = Unico("Modulo ", 100),
                Consultar = true,
                Insertar = true,
                Actualizar = true,
                Borrar = true
            };
        }

        public Variantes Variantes()
        {
            var producto = Guardar(this.conexion.Productos!, Productos());
            var talla = Guardar(this.conexion.Tallas!, Tallas());
            var color = Guardar(this.conexion.Colores!, Colores());

            return new Variantes
            {
                Sku = Unico("SKU-", 50),
                Producto = producto.Id,
                Talla = talla.Id,
                Color = color.Id,
                Precio_Adicional = 0.00m
            };
        }

        public Ventas Ventas()
        {
            var cliente = Guardar(this.conexion.Clientes!, Clientes());
            var empleado = Guardar(this.conexion.Empleados!, Empleados());

            return new Ventas
            {
                Codigo = Unico("V-", 50),
                Cliente = cliente.Id,
                Empleado = empleado.Id,
                Fecha = DateTime.Now,
                Total = 100000.00m
            };
        }

        public Pedidos Pedidos()
        {
            var proveedor = Guardar(this.conexion.Proveedores!, Proveedores());
            var empleado = Guardar(this.conexion.Empleados!, Empleados());

            return new Pedidos
            {
                Codigo = Unico("P-", 50),
                Proveedor = proveedor.Id,
                Empleado = empleado.Id,
                Fecha = DateTime.Now,
                Estado = "Pendiente",
                Total = 1800000.00m
            };
        }

        public Inventarios Inventarios()
        {
            var variante = Guardar(this.conexion.Variantes!, Variantes());
            var sucursal = Guardar(this.conexion.Sucursales!, Sucursales());

            return new Inventarios
            {
                Variante = variante.Id,
                Sucursal = sucursal.Id,
                Cantidad = 30,
                Stock_Minimo = 5,
                Fecha_Actualizacion = DateTime.Now
            };
        }

        public Detalles_Ventas Detalles_Ventas()
        {
            var venta = Guardar(this.conexion.Ventas!, Ventas());
            var variante = Guardar(this.conexion.Variantes!, Variantes());

            return new Detalles_Ventas
            {
                Venta = venta.Id,
                Variante = variante.Id,
                Cantidad = 2,
                Precio_Unitario = 280000.00m,
                Subtotal = 560000.00m
            };
        }

        public Pagos Pagos()
        {
            var venta = Guardar(this.conexion.Ventas!, Ventas());
            var metodo = Guardar(this.conexion.Metodos_Pago!, Metodos_Pago());

            return new Pagos
            {
                Venta = venta.Id,
                Metodo_Pago = metodo.Id,
                Valor = 100000.00m,
                Fecha = DateTime.Now
            };
        }

        /// <summary>
        /// PNG valido de un pixel. Se usa un archivo real y no bytes al azar
        /// para que la prueba recorra el mismo camino que una imagen de
        /// verdad: el controlador valida el tipo de contenido.
        /// </summary>
        public static readonly byte[] Png = Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8" +
            "z8DwHwAFAAH/q842iQAAAABJRU5ErkJggg==");

        public Imagenes Imagenes()
        {
            var producto = Guardar(this.conexion.Productos!, Productos());

            return new Imagenes
            {
                Producto = producto.Id,
                Nombre = Unico("img", 40) + ".png",
                Tipo = "image/png",
                Tamano = Png.Length,
                Fecha = DateTime.Now,
                Contenido = Png
            };
        }

        public Detalles_Pedidos Detalles_Pedidos()
        {
            var pedido = Guardar(this.conexion.Pedidos!, Pedidos());
            var variante = Guardar(this.conexion.Variantes!, Variantes());

            return new Detalles_Pedidos
            {
                Pedido = pedido.Id,
                Variante = variante.Id,
                Cantidad = 2,
                Precio_Compra = 180000.00m,
                Subtotal = 360000.00m
            };
        }
    }
}
