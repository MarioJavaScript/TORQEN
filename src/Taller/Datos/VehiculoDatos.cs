using Taller.Modelos;

namespace Taller.Datos;

public static class VehiculoDatos
{
    public static List<Vehiculo> Listar()
    {
        using var conexion = Db.Conectar();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT v.id_vehiculo, v.placa, v.marca, v.modelo, v.anio, v.id_cliente, c.nombre FROM vehiculo v JOIN cliente c ON c.id_cliente = v.id_cliente ORDER BY v.id_vehiculo;";
        using var lector = comando.ExecuteReader();
        var vehiculos = new List<Vehiculo>();
        while (lector.Read())
        {
            vehiculos.Add(new Vehiculo
            {
                IdVehiculo = lector.GetInt32(0),
                Placa = lector.GetString(1),
                Marca = lector.GetString(2),
                Modelo = lector.GetString(3),
                Anio = lector.GetInt32(4),
                IdCliente = lector.GetInt32(5),
                NombreCliente = lector.GetString(6)
            });
        }
        return vehiculos;
    }

    public static Vehiculo? BuscarPorId(int id)
    {
        using var conexion = Db.Conectar();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT v.id_vehiculo, v.placa, v.marca, v.modelo, v.anio, v.id_cliente, c.nombre FROM vehiculo v JOIN cliente c ON c.id_cliente = v.id_cliente WHERE v.id_vehiculo = $id;";
        comando.Parameters.AddWithValue("$id", id);
        using var lector = comando.ExecuteReader();
        if (!lector.Read())
            return null;
        return new Vehiculo
        {
            IdVehiculo = lector.GetInt32(0),
            Placa = lector.GetString(1),
            Marca = lector.GetString(2),
            Modelo = lector.GetString(3),
            Anio = lector.GetInt32(4),
            IdCliente = lector.GetInt32(5),
            NombreCliente = lector.GetString(6)
        };
    }

    public static int Insertar(Vehiculo vehiculo)
    {
        using var conexion = Db.Conectar();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "INSERT INTO vehiculo (placa, marca, modelo, anio, id_cliente) VALUES ($placa, $marca, $modelo, $anio, $id_cliente); SELECT last_insert_rowid();";
        comando.Parameters.AddWithValue("$placa", vehiculo.Placa.ToUpperInvariant());
        comando.Parameters.AddWithValue("$marca", vehiculo.Marca);
        comando.Parameters.AddWithValue("$modelo", vehiculo.Modelo);
        comando.Parameters.AddWithValue("$anio", vehiculo.Anio);
        comando.Parameters.AddWithValue("$id_cliente", vehiculo.IdCliente);
        return Convert.ToInt32(comando.ExecuteScalar());
    }

    public static bool Actualizar(Vehiculo vehiculo)
    {
        using var conexion = Db.Conectar();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "UPDATE vehiculo SET placa = $placa, marca = $marca, modelo = $modelo, anio = $anio, id_cliente = $id_cliente WHERE id_vehiculo = $id;";
        comando.Parameters.AddWithValue("$placa", vehiculo.Placa.ToUpperInvariant());
        comando.Parameters.AddWithValue("$marca", vehiculo.Marca);
        comando.Parameters.AddWithValue("$modelo", vehiculo.Modelo);
        comando.Parameters.AddWithValue("$anio", vehiculo.Anio);
        comando.Parameters.AddWithValue("$id_cliente", vehiculo.IdCliente);
        comando.Parameters.AddWithValue("$id", vehiculo.IdVehiculo);
        return comando.ExecuteNonQuery() == 1;
    }

    public static bool Eliminar(int id)
    {
        using var conexion = Db.Conectar();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "DELETE FROM vehiculo WHERE id_vehiculo = $id;";
        comando.Parameters.AddWithValue("$id", id);
        return comando.ExecuteNonQuery() == 1;
    }
}
