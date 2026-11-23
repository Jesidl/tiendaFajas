using aplicacion_tiendaFajas.implementaciones;
using aplicacion_tiendaFajas.interfaces;
using aplicacion_tiendaFajas.nucleo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IConexion, Conexion>();
builder.Services.AddScoped<ICategoriasAplicacion, CategoriasAplicacion>();
builder.Services.AddScoped<IMarcasAplicacion, MarcasAplicacion>();
builder.Services.AddScoped<IMaterialesAplicacion, MaterialesAplicacion>();
builder.Services.AddScoped<ICargosAplicacion, CargosAplicacion>();
builder.Services.AddScoped<ISucursalesAplicacion, SucursalesAplicacion>();
builder.Services.AddScoped<IClientesAplicacion, ClientesAplicacion>();
builder.Services.AddScoped<IProveedoresAplicacion, ProveedoresAplicacion>();
builder.Services.AddScoped<ITallasAplicacion, TallasAplicacion>();
builder.Services.AddScoped<IColoresAplicacion, ColoresAplicacion>();
builder.Services.AddScoped<IMetodos_PagoAplicacion, Metodos_PagoAplicacion>();
builder.Services.AddScoped<IEmpleadosAplicacion, EmpleadosAplicacion>();
builder.Services.AddScoped<IMedidasAplicacion, MedidasAplicacion>();
builder.Services.AddScoped<IProductosAplicacion, ProductosAplicacion>();
builder.Services.AddScoped<IVariantesAplicacion, VariantesAplicacion>();
builder.Services.AddScoped<IInventariosAplicacion, InventariosAplicacion>();
builder.Services.AddScoped<IVentasAplicacion, VentasAplicacion>();
builder.Services.AddScoped<IDetalles_VentasAplicacion, Detalles_VentasAplicacion>();
builder.Services.AddScoped<IPagosAplicacion, PagosAplicacion>();
builder.Services.AddScoped<IPedidosAplicacion, PedidosAplicacion>();
builder.Services.AddScoped<IDetalles_PedidosAplicacion, Detalles_PedidosAplicacion>();
builder.Services.AddScoped<IRolesAplicacion, RolesAplicacion>();
builder.Services.AddScoped<IUsuariosAplicacion, UsuariosAplicacion>();
builder.Services.AddScoped<IPermisosAplicacion, PermisosAplicacion>();
builder.Services.AddScoped<IAuditoriasAplicacion, AuditoriasAplicacion>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
// quien hizo la peticion viaja en la cabecera X-Usuario y se deja en el
// contexto para que Conexion lo escriba en la auditoria
app.Use(async (contexto, siguiente) =>
{
    ContextoUsuario.Usuario = contexto.Request
        .Headers[ContextoUsuario.Cabecera].FirstOrDefault();
    await siguiente();
});

app.MapControllers();
app.MapGet("/", () => "");
app.Run();