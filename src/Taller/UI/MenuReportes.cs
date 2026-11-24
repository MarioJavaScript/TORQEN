using System.Globalization;
using Microsoft.Data.Sqlite;
using Taller.Datos;

namespace Taller.UI;

public static class MenuReportes
{
    public static void Mostrar()
    {
        while (true)
        {
            Consola.Titulo("Reportes");
            Console.WriteLine("1. Listado de órdenes (INNER JOIN)");
            Console.WriteLine("2. Clientes sin vehículos (LEFT JOIN)");
            Console.WriteLine("3. Ingreso por servicio (GROUP BY y HAVING)");
            Console.WriteLine("4. Servicios más caros que el promedio (Subconsulta)");
            Console.WriteLine("5. Búsqueda de vehículos (LIKE, AND y OR)");
            Console.WriteLine("6. Total facturado (ExecuteScalar)");
            Console.WriteLine("0. Volver");
            int opcion = Consola.LeerEntero("Opción: ");
            if (opcion == 0)
                return;

            try
            {
                switch (opcion)
                {
                    case 1: ListarOrdenes(); break;
                    case 2: ClientesSinVehiculos(); break;
                    case 3: IngresoPorServicio(); break;
                    case 4: ServiciosSobrePromedio(); break;
                    case 5: BuscarVehiculos(); break;
                    case 6: Console.WriteLine("Total facturado: " + Moneda(ReporteDatos.TotalFacturado())); break;
                    default: Console.WriteLine("Opción inválida."); break;
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine(Errores.Mensaje(ex));
            }
            Consola.Pausa();
        }
    }

    private static void ListarOrdenes()
    {
        var filas = new List<string[]>();
        foreach (var orden in ReporteDatos.ListarOrdenes())
            filas.Add([orden.IdOrden.ToString(), orden.Fecha, orden.Estado, orden.Placa, orden.Cliente, orden.Mecanico]);
        MostrarTabla(["ID", "Fecha", "Estado", "Placa", "Cliente", "Mecánico"], filas);
    }

    private static void ClientesSinVehiculos()
    {
        var filas = new List<string[]>();
        foreach (var cliente in ReporteDatos.ClientesSinVehiculos())
            filas.Add([cliente.IdCliente.ToString(), cliente.Nombre]);
        MostrarTabla(["ID", "Cliente"], filas);
    }

    private static void IngresoPorServicio()
    {
        decimal minimo = Consola.LeerDecimal("Ingreso mínimo (se muestran ingresos mayores): ");
        var filas = new List<string[]>();
        foreach (var servicio in ReporteDatos.IngresoPorServicio(minimo))
            filas.Add([servicio.Nombre, servicio.Veces.ToString(), Moneda(servicio.Ingreso)]);
        Console.WriteLine("Veces solicitado cuenta las órdenes que incluyen el servicio.");
        MostrarTabla(["Servicio", "Veces solicitado", "Ingreso"], filas);
    }

    private static void ServiciosSobrePromedio()
    {
        var filas = new List<string[]>();
        foreach (var servicio in ReporteDatos.ServiciosSobrePromedio())
            filas.Add([servicio.IdServicio.ToString(), servicio.Nombre, Moneda(servicio.PrecioBase), Moneda(servicio.Promedio)]);
        MostrarTabla(["ID", "Servicio", "Precio base", "Promedio"], filas);
    }

    private static void BuscarVehiculos()
    {
        string texto = Consola.LeerTexto("Texto en marca, modelo o placa: ");
        int anioMinimo = Consola.LeerEntero("Año mínimo: ");
        var filas = new List<string[]>();
        foreach (var vehiculo in ReporteDatos.BuscarVehiculos(texto, anioMinimo))
            filas.Add([vehiculo.IdVehiculo.ToString(), vehiculo.Placa, vehiculo.Marca, vehiculo.Modelo, vehiculo.Anio.ToString()]);
        MostrarTabla(["ID", "Placa", "Marca", "Modelo", "Año"], filas);
    }

    private static string Moneda(decimal valor)
    {
        return "$" + valor.ToString("N2", CultureInfo.InvariantCulture);
    }

    private static void MostrarTabla(string[] columnas, List<string[]> filas)
    {
        if (filas.Count == 0)
        {
            Console.WriteLine("Sin resultados");
            return;
        }
        var anchos = new int[columnas.Length];
        for (int i = 0; i < columnas.Length; i++)
            anchos[i] = Math.Max(columnas[i].Length, filas.Max(fila => fila[i].Length)) + 2;

        for (int i = 0; i < columnas.Length; i++)
            Console.Write(columnas[i].PadRight(anchos[i]));
        Console.WriteLine();
        foreach (var fila in filas)
        {
            for (int i = 0; i < columnas.Length; i++)
                Console.Write(fila[i].PadRight(anchos[i]));
            Console.WriteLine();
        }
    }
}
