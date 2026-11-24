using Microsoft.Data.Sqlite;
using Taller.Datos;
using Taller.Modelos;

namespace Taller.UI;

public static class MenuVehiculos
{
    public static void Mostrar()
    {
        while (true)
        {
            Consola.Titulo("Vehículos");
            Console.WriteLine("1. Alta");
            Console.WriteLine("2. Listar todos");
            Console.WriteLine("3. Consultar por id");
            Console.WriteLine("4. Modificar");
            Console.WriteLine("5. Baja");
            Console.WriteLine("0. Volver");
            int opcion = Consola.LeerEntero("Opción: ");
            if (opcion == 0)
                return;

            try
            {
                switch (opcion)
                {
                    case 1: Alta(); break;
                    case 2: Listar(); break;
                    case 3: Consultar(); break;
                    case 4: Modificar(); break;
                    case 5: Baja(); break;
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

    private static void MostrarClientes()
    {
        Console.WriteLine("Clientes disponibles:");
        foreach (var cliente in ClienteDatos.Listar())
            Console.WriteLine(cliente.IdCliente.ToString().PadRight(6) + cliente.Nombre);
    }

    private static void Alta()
    {
        var vehiculo = new Vehiculo
        {
            Placa = Consola.LeerTexto("Placa: ").ToUpperInvariant(),
            Marca = Consola.LeerTexto("Marca: "),
            Modelo = Consola.LeerTexto("Modelo: "),
            Anio = Consola.LeerEntero("Año: ")
        };
        MostrarClientes();
        vehiculo.IdCliente = Consola.LeerEntero("Id del cliente: ");
        int id = VehiculoDatos.Insertar(vehiculo);
        Console.WriteLine("Vehículo registrado con id " + id + ".");
    }

    private static void Listar()
    {
        var vehiculos = VehiculoDatos.Listar();
        if (vehiculos.Count == 0)
        {
            Console.WriteLine("No hay vehículos registrados.");
            return;
        }
        int anchoPlaca = Math.Max(5, vehiculos.Max(v => v.Placa.Length));
        int anchoMarca = Math.Max(5, vehiculos.Max(v => v.Marca.Length));
        int anchoModelo = Math.Max(6, vehiculos.Max(v => v.Modelo.Length));
        Console.WriteLine("ID".PadRight(6) + "Placa".PadRight(anchoPlaca + 2) +
            "Marca".PadRight(anchoMarca + 2) + "Modelo".PadRight(anchoModelo + 2) +
            "Año".PadRight(8) + "Id cliente".PadRight(12) + "Dueño");
        foreach (var vehiculo in vehiculos)
        {
            Console.WriteLine(vehiculo.IdVehiculo.ToString().PadRight(6) +
                vehiculo.Placa.PadRight(anchoPlaca + 2) + vehiculo.Marca.PadRight(anchoMarca + 2) +
                vehiculo.Modelo.PadRight(anchoModelo + 2) + vehiculo.Anio.ToString().PadRight(8) +
                vehiculo.IdCliente.ToString().PadRight(12) + vehiculo.NombreCliente);
        }
    }

    private static Vehiculo? PedirVehiculo()
    {
        int id = Consola.LeerEntero("Id del vehículo: ");
        var vehiculo = VehiculoDatos.BuscarPorId(id);
        if (vehiculo == null)
            Console.WriteLine("Vehículo no encontrado");
        return vehiculo;
    }

    private static void MostrarVehiculo(Vehiculo vehiculo)
    {
        Console.WriteLine("Id: " + vehiculo.IdVehiculo);
        Console.WriteLine("Placa: " + vehiculo.Placa);
        Console.WriteLine("Marca: " + vehiculo.Marca);
        Console.WriteLine("Modelo: " + vehiculo.Modelo);
        Console.WriteLine("Año: " + vehiculo.Anio);
        Console.WriteLine("Cliente: " + vehiculo.IdCliente + " - " + vehiculo.NombreCliente);
    }

    private static void Consultar()
    {
        var vehiculo = PedirVehiculo();
        if (vehiculo != null)
            MostrarVehiculo(vehiculo);
    }

    private static void Modificar()
    {
        var vehiculo = PedirVehiculo();
        if (vehiculo == null)
            return;
        MostrarVehiculo(vehiculo);
        Console.WriteLine("Deja vacío para conservar el valor actual.");
        string placa = Consola.LeerTextoOpcional("Nueva placa: ");
        string marca = Consola.LeerTextoOpcional("Nueva marca: ");
        string modelo = Consola.LeerTextoOpcional("Nuevo modelo: ");
        int anio = LeerEnteroOpcional("Nuevo año: ", vehiculo.Anio);
        MostrarClientes();
        int idCliente = LeerEnteroOpcional("Nuevo id del cliente: ", vehiculo.IdCliente);
        if (placa.Length > 0) vehiculo.Placa = placa.ToUpperInvariant();
        if (marca.Length > 0) vehiculo.Marca = marca;
        if (modelo.Length > 0) vehiculo.Modelo = modelo;
        vehiculo.Anio = anio;
        vehiculo.IdCliente = idCliente;
        Console.WriteLine(VehiculoDatos.Actualizar(vehiculo) ? "Vehículo actualizado." : "Vehículo no encontrado");
    }

    private static int LeerEnteroOpcional(string mensaje, int actual)
    {
        while (true)
        {
            string texto = Consola.LeerTextoOpcional(mensaje);
            if (texto.Length == 0)
                return actual;
            if (int.TryParse(texto, out int valor))
                return valor;
            Console.WriteLine("Ingresa un número entero válido.");
        }
    }

    private static void Baja()
    {
        var vehiculo = PedirVehiculo();
        if (vehiculo == null)
            return;
        MostrarVehiculo(vehiculo);
        string respuesta;
        do
        {
            respuesta = Consola.LeerTexto("¿Eliminar vehículo? (s/n): ");
        } while (!respuesta.Equals("s", StringComparison.OrdinalIgnoreCase) &&
                 !respuesta.Equals("n", StringComparison.OrdinalIgnoreCase));
        if (respuesta.Equals("s", StringComparison.OrdinalIgnoreCase))
            Console.WriteLine(VehiculoDatos.Eliminar(vehiculo.IdVehiculo) ? "Vehículo eliminado." : "Vehículo no encontrado");
        else
            Console.WriteLine("Baja cancelada.");
    }
}
