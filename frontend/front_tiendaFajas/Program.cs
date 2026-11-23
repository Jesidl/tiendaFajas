using aplicacion_tiendaFajas.implementaciones;
using aplicacion_tiendaFajas.interfaces;
using presentacion_tiendaFajas.implementaciones;
using presentacion_tiendaFajas.interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddControllers();
builder.Services.AddScoped<IComunicaciones, Comunicaciones>();
builder.Services.AddScoped<ICategoriasPresentacion, CategoriasPresentacion>();
builder.Services.AddScoped<IMarcasPresentacion, MarcasPresentacion>();
builder.Services.AddScoped<IMaterialesPresentacion, MaterialesPresentacion>();
builder.Services.AddScoped<ICargosPresentacion, CargosPresentacion>();
builder.Services.AddScoped<ISucursalesPresentacion, SucursalesPresentacion>();
builder.Services.AddScoped<IClientesPresentacion, ClientesPresentacion>();
builder.Services.AddScoped<IProveedoresPresentacion, ProveedoresPresentacion>();
builder.Services.AddScoped<ITallasPresentacion, TallasPresentacion>();
builder.Services.AddScoped<IColoresPresentacion, ColoresPresentacion>();
builder.Services.AddScoped<IMetodos_PagoPresentacion, Metodos_PagoPresentacion>();
builder.Services.AddScoped<IEmpleadosPresentacion, EmpleadosPresentacion>();
builder.Services.AddScoped<IMedidasPresentacion, MedidasPresentacion>();
builder.Services.AddScoped<IProductosPresentacion, ProductosPresentacion>();
builder.Services.AddScoped<IVariantesPresentacion, VariantesPresentacion>();
builder.Services.AddScoped<IInventariosPresentacion, InventariosPresentacion>();
builder.Services.AddScoped<IVentasPresentacion, VentasPresentacion>();
builder.Services.AddScoped<IDetalles_VentasPresentacion, Detalles_VentasPresentacion>();
builder.Services.AddScoped<IPagosPresentacion, PagosPresentacion>();
builder.Services.AddScoped<IPedidosPresentacion, PedidosPresentacion>();
builder.Services.AddScoped<IDetalles_PedidosPresentacion, Detalles_PedidosPresentacion>();
builder.Services.AddScoped<IRolesPresentacion, RolesPresentacion>();
builder.Services.AddScoped<IUsuariosPresentacion, UsuariosPresentacion>();
builder.Services.AddScoped<IPermisosPresentacion, PermisosPresentacion>();
builder.Services.AddScoped<IAuditoriasPresentacion, AuditoriasPresentacion>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.Run();