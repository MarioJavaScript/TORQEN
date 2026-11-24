using Taller.Modelos;

namespace Taller.Datos;

public static class ClienteDatos
{
    public static List<Cliente> Listar()
    {
        using var conexion = Db.Conectar();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT id_cliente, nombre, telefono, email, fecha_registro FROM cliente ORDER BY id_cliente;";
        using var lector = comando.ExecuteReader();
        var clientes = new List<Cliente>();
        while (lector.Read())
        {
            clientes.Add(new Cliente
            {
                IdCliente = lector.GetInt32(0),
                Nombre = lector.GetString(1),
                Telefono = lector.GetString(2),
                Email = lector.IsDBNull(3) ? null : lector.GetString(3),
                FechaRegistro = lector.GetString(4)
            });
        }
        return clientes;
    }

    public static Cliente? BuscarPorId(int id)
    {
        using var conexion = Db.Conectar();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT id_cliente, nombre, telefono, email, fecha_registro FROM cliente WHERE id_cliente = $id;";
        comando.Parameters.AddWithValue("$id", id);
        using var lector = comando.ExecuteReader();
        if (!lector.Read())
            return null;
        return new Cliente
        {
            IdCliente = lector.GetInt32(0),
            Nombre = lector.GetString(1),
            Telefono = lector.GetString(2),
            Email = lector.IsDBNull(3) ? null : lector.GetString(3),
            FechaRegistro = lector.GetString(4)
        };
    }

    public static int Insertar(Cliente cliente)
    {
        using var conexion = Db.Conectar();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "INSERT INTO cliente (nombre, telefono, email) VALUES ($nombre, $telefono, $email); SELECT last_insert_rowid();";
        comando.Parameters.AddWithValue("$nombre", cliente.Nombre);
        comando.Parameters.AddWithValue("$telefono", cliente.Telefono);
        comando.Parameters.AddWithValue("$email", string.IsNullOrWhiteSpace(cliente.Email) ? DBNull.Value : cliente.Email);
        return Convert.ToInt32(comando.ExecuteScalar());
    }

    public static bool Actualizar(Cliente cliente)
    {
        using var conexion = Db.Conectar();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "UPDATE cliente SET nombre = $nombre, telefono = $telefono, email = $email WHERE id_cliente = $id;";
        comando.Parameters.AddWithValue("$nombre", cliente.Nombre);
        comando.Parameters.AddWithValue("$telefono", cliente.Telefono);
        comando.Parameters.AddWithValue("$email", string.IsNullOrWhiteSpace(cliente.Email) ? DBNull.Value : cliente.Email);
        comando.Parameters.AddWithValue("$id", cliente.IdCliente);
        return comando.ExecuteNonQuery() == 1;
    }

    public static bool Eliminar(int id)
    {
        using var conexion = Db.Conectar();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "DELETE FROM cliente WHERE id_cliente = $id;";
        comando.Parameters.AddWithValue("$id", id);
        return comando.ExecuteNonQuery() == 1;
    }
}
