using libreria_notas.implementaciones;
using libreria_notas.interfaces;
using libreria_notas.nucleo;
using Microsoft.EntityFrameworkCore;

try
{
    IConexion conexion = new Conexion();
    conexion.StringConexion = DatosGenerales.ObtenerStringConexion();
    var lista_materias = conexion.Materias!.ToList();
    var lista_estudiantes = conexion.Estudiantes!.ToList();
    var lista_notas = conexion.Notas!.ToList();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}

Console.WriteLine("presentacion_csn");
