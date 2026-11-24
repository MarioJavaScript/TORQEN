using Microsoft.Data.Sqlite;
using Taller.Datos;
using Taller.UI;

try
{
    using var conexion = Db.Conectar();
    Console.WriteLine("Conectado a taller.db");
}
catch (SqliteException ex)
{
    Console.WriteLine(Errores.Mensaje(ex));
    return;
}
catch (FileNotFoundException ex)
{
    Console.WriteLine(ex.Message);
    return;
}

while (true)
{
    Consola.Titulo("Taller mecánico");
    Console.WriteLine("1. Clientes");
    Console.WriteLine("2. Vehículos");
    Console.WriteLine("3. Registrar orden de servicio");
    Console.WriteLine("4. Reportes");
    Console.WriteLine("0. Salir");

    try
    {
        int opcion = Consola.LeerEntero("Opción: ");
        if (opcion == 0)
            break;

        try
        {
            switch (opcion)
            {
                case 1:
                    MenuClientes.Mostrar();
                    break;
                case 2:
                    MenuVehiculos.Mostrar();
                    break;
                case 3:
                    MenuOrden.Mostrar();
                    break;
                case 4:
                    Console.WriteLine("Próximamente");
                    break;
                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine(Errores.Mensaje(ex));
        }
        Consola.Pausa();
    }
    catch (EndOfStreamException)
    {
        break;
    }
}
