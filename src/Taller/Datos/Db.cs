using Microsoft.Data.Sqlite;

namespace Taller.Datos;

public static class Db
{
    public static SqliteConnection Conectar()
    {
        DirectoryInfo? carpeta = new DirectoryInfo(AppContext.BaseDirectory);
        while (carpeta != null)
        {
            string ruta = Path.Combine(carpeta.FullName, "db", "taller.db");
            if (File.Exists(ruta))
            {
                var conexion = new SqliteConnection(new SqliteConnectionStringBuilder
                {
                    DataSource = ruta,
                    Mode = SqliteOpenMode.ReadWrite
                }.ToString());
                try
                {
                    conexion.Open();
                    using var comando = conexion.CreateCommand();
                    comando.CommandText = "PRAGMA foreign_keys = ON;";
                    comando.ExecuteNonQuery();
                    return conexion;
                }
                catch
                {
                    conexion.Dispose();
                    throw;
                }
            }
            carpeta = carpeta.Parent;
        }
        throw new FileNotFoundException("No se encontró db/taller.db buscando desde " +
            AppContext.BaseDirectory + ". Genera la base antes de iniciar el programa.");
    }
}
