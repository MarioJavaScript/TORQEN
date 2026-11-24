using Microsoft.Data.Sqlite;
using Taller.Datos;
using Taller.Modelos;

namespace Taller.UI;

public static class MenuOrden
{
    public static void Mostrar()
    {
        Consola.Titulo("Registrar orden de servicio");
        Console.WriteLine("Vehículos disponibles:");
        foreach (var vehiculo in VehiculoDatos.Listar())
            Console.WriteLine(vehiculo.IdVehiculo.ToString().PadRight(6) +
                vehiculo.Placa.PadRight(14) + vehiculo.NombreCliente);
        int idVehiculo = Consola.LeerEntero("Id del vehículo: ");

        Console.WriteLine("Mecánicos disponibles:");
        foreach (var mecanico in OrdenDatos.ListarMecanicos())
            Console.WriteLine(mecanico.IdMecanico.ToString().PadRight(6) + mecanico.Nombre);
        int idMecanico = Consola.LeerEntero("Id del mecánico: ");

        var servicios = OrdenDatos.ListarServicios();
        Console.WriteLine("Servicios disponibles:");
        foreach (var servicio in servicios)
            Console.WriteLine(servicio.IdServicio.ToString().PadRight(6) +
                servicio.Nombre.PadRight(36) + "$" + servicio.PrecioBase.ToString("F2"));

        var detalles = new List<DetalleOrden>();
        while (true)
        {
            int idServicio = Consola.LeerEntero("Id del servicio (0 para terminar): ");
            if (idServicio == 0)
            {
                if (detalles.Count > 0)
                    break;
                Console.WriteLine("Agrega al menos un servicio.");
                continue;
            }
            detalles.Add(new DetalleOrden
            {
                IdServicio = idServicio,
                Cantidad = Consola.LeerEntero("Cantidad: ")
            });
        }

        Consola.Titulo("Resumen de la orden");
        Console.WriteLine("Id del vehículo: " + idVehiculo);
        Console.WriteLine("Id del mecánico: " + idMecanico);
        decimal totalEstimado = 0;
        foreach (var detalle in detalles)
        {
            var servicio = servicios.Find(s => s.IdServicio == detalle.IdServicio);
            Console.WriteLine("Servicio " + detalle.IdServicio + ": " +
                (servicio.Nombre ?? "No encontrado") + " | Cantidad: " + detalle.Cantidad +
                " | Precio base: $" + servicio.PrecioBase.ToString("F2"));
            totalEstimado += detalle.Cantidad * servicio.PrecioBase;
        }
        Console.WriteLine("Total estimado: $" + totalEstimado.ToString("F2"));
        string respuesta;
        do
        {
            respuesta = Consola.LeerTexto("¿Registrar orden? (s/n): ");
        } while (!respuesta.Equals("s", StringComparison.OrdinalIgnoreCase) &&
                 !respuesta.Equals("n", StringComparison.OrdinalIgnoreCase));
        if (respuesta.Equals("n", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Registro cancelado.");
            return;
        }

        int idOrden;
        try
        {
            idOrden = OrdenDatos.RegistrarOrden(idVehiculo, idMecanico, detalles);
        }
        catch (SqliteException ex)
        {
            Console.WriteLine("No se registró la orden, se deshicieron todos los cambios.");
            Console.WriteLine(Errores.Mensaje(ex));
            return;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine("No se registró la orden, se deshicieron todos los cambios.");
            Console.WriteLine(ex.Message);
            return;
        }

        decimal total = OrdenDatos.TotalOrden(idOrden);
        Console.WriteLine("Orden #" + idOrden + " registrada. Total: $" + total.ToString("F2"));
    }
}
