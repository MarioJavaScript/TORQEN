using Microsoft.Data.Sqlite;
using Taller.Datos;
using Taller.Modelos;

namespace Taller.UI;

public static class MenuClientes
{
    public static void Mostrar()
    {
        while (true)
        {
            Consola.Titulo("Clientes");
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

    private static void Alta()
    {
        var cliente = new Cliente
        {
            Nombre = Consola.LeerTexto("Nombre: "),
            Telefono = Consola.LeerTexto("Teléfono: "),
            Email = Consola.LeerTextoOpcional("Email (opcional): ")
        };
        int id = ClienteDatos.Insertar(cliente);
        Console.WriteLine("Cliente registrado con id " + id + ".");
    }

    private static void Listar()
    {
        var clientes = ClienteDatos.Listar();
        if (clientes.Count == 0)
        {
            Console.WriteLine("No hay clientes registrados.");
            return;
        }
        int anchoNombre = Math.Max(6, clientes.Max(c => c.Nombre.Length));
        int anchoTelefono = Math.Max(8, clientes.Max(c => c.Telefono.Length));
        int anchoEmail = Math.Max(5, clientes.Max(c => (c.Email ?? "—").Length));
        Console.WriteLine("ID".PadRight(6) + "Nombre".PadRight(anchoNombre + 2) +
            "Teléfono".PadRight(anchoTelefono + 2) + "Email".PadRight(anchoEmail + 2) + "Fecha de registro");
        foreach (var cliente in clientes)
        {
            Console.WriteLine(cliente.IdCliente.ToString().PadRight(6) +
                cliente.Nombre.PadRight(anchoNombre + 2) + cliente.Telefono.PadRight(anchoTelefono + 2) +
                (cliente.Email ?? "—").PadRight(anchoEmail + 2) + cliente.FechaRegistro);
        }
    }

    private static Cliente? PedirCliente()
    {
        int id = Consola.LeerEntero("Id del cliente: ");
        var cliente = ClienteDatos.BuscarPorId(id);
        if (cliente == null)
            Console.WriteLine("Cliente no encontrado");
        return cliente;
    }

    private static void MostrarCliente(Cliente cliente)
    {
        Console.WriteLine("Id: " + cliente.IdCliente);
        Console.WriteLine("Nombre: " + cliente.Nombre);
        Console.WriteLine("Teléfono: " + cliente.Telefono);
        Console.WriteLine("Email: " + (cliente.Email ?? "Sin email"));
        Console.WriteLine("Fecha de registro: " + cliente.FechaRegistro);
    }

    private static void Consultar()
    {
        var cliente = PedirCliente();
        if (cliente != null)
            MostrarCliente(cliente);
    }

    private static void Modificar()
    {
        var cliente = PedirCliente();
        if (cliente == null)
            return;
        MostrarCliente(cliente);
        Console.WriteLine("Deja vacío para conservar el valor actual.");
        string nombre = Consola.LeerTextoOpcional("Nuevo nombre: ");
        string telefono = Consola.LeerTextoOpcional("Nuevo teléfono: ");
        string email = Consola.LeerTextoOpcional("Nuevo email: ");
        if (nombre.Length > 0) cliente.Nombre = nombre;
        if (telefono.Length > 0) cliente.Telefono = telefono;
        if (email.Length > 0) cliente.Email = email;
        Console.WriteLine(ClienteDatos.Actualizar(cliente) ? "Cliente actualizado." : "Cliente no encontrado");
    }

    private static void Baja()
    {
        var cliente = PedirCliente();
        if (cliente == null)
            return;
        MostrarCliente(cliente);
        string respuesta;
        do
        {
            respuesta = Consola.LeerTexto("¿Eliminar cliente? (s/n): ");
        } while (!respuesta.Equals("s", StringComparison.OrdinalIgnoreCase) &&
                 !respuesta.Equals("n", StringComparison.OrdinalIgnoreCase));
        if (respuesta.Equals("s", StringComparison.OrdinalIgnoreCase))
            Console.WriteLine(ClienteDatos.Eliminar(cliente.IdCliente) ? "Cliente eliminado." : "Cliente no encontrado");
        else
            Console.WriteLine("Baja cancelada.");
    }
}
