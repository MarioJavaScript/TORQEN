using Taller.Modelos;

namespace Taller.Datos;

public static class OrdenDatos
{
    public static List<(int IdMecanico, string Nombre)> ListarMecanicos()
    {
        using var conexion = Db.Conectar();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT id_mecanico, nombre FROM mecanico ORDER BY id_mecanico;";
        using var lector = comando.ExecuteReader();
        var mecanicos = new List<(int IdMecanico, string Nombre)>();
        while (lector.Read())
            mecanicos.Add((lector.GetInt32(0), lector.GetString(1)));
        return mecanicos;
    }

    public static List<(int IdServicio, string Nombre, decimal PrecioBase)> ListarServicios()
    {
        using var conexion = Db.Conectar();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT id_servicio, nombre, precio_base FROM servicio ORDER BY id_servicio;";
        using var lector = comando.ExecuteReader();
        var servicios = new List<(int IdServicio, string Nombre, decimal PrecioBase)>();
        while (lector.Read())
            servicios.Add((lector.GetInt32(0), lector.GetString(1), lector.GetDecimal(2)));
        return servicios;
    }

    public static int RegistrarOrden(int idVehiculo, int idMecanico, List<DetalleOrden> detalles)
    {
        using var conexion = Db.Conectar();
        using var transaccion = conexion.BeginTransaction();
        try
        {
            if (detalles.Count == 0)
                throw new InvalidOperationException("La orden debe incluir al menos un servicio.");

            using var comando = conexion.CreateCommand();
            comando.Transaction = transaccion;
            comando.CommandText = "INSERT INTO orden (id_vehiculo, id_mecanico) VALUES ($id_vehiculo, $id_mecanico); SELECT last_insert_rowid();";
            comando.Parameters.AddWithValue("$id_vehiculo", idVehiculo);
            comando.Parameters.AddWithValue("$id_mecanico", idMecanico);
            int idOrden = Convert.ToInt32(comando.ExecuteScalar());

            foreach (var detalle in detalles)
            {
                using var comandoDetalle = conexion.CreateCommand();
                comandoDetalle.Transaction = transaccion;
                comandoDetalle.CommandText = "INSERT INTO orden_servicio (id_orden, id_servicio, cantidad, precio) SELECT $id_orden, id_servicio, $cantidad, precio_base FROM servicio WHERE id_servicio = $id;";
                comandoDetalle.Parameters.AddWithValue("$id_orden", idOrden);
                comandoDetalle.Parameters.AddWithValue("$cantidad", detalle.Cantidad);
                comandoDetalle.Parameters.AddWithValue("$id", detalle.IdServicio);
                if (comandoDetalle.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException("Servicio no encontrado: " + detalle.IdServicio + ".");
            }

            transaccion.Commit();
            return idOrden;
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public static decimal TotalOrden(int idOrden)
    {
        using var conexion = Db.Conectar();
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT COALESCE(SUM(cantidad * precio), 0) FROM orden_servicio WHERE id_orden = $id;";
        comando.Parameters.AddWithValue("$id", idOrden);
        return Convert.ToDecimal(comando.ExecuteScalar());
    }
}
