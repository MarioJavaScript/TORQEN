# Taller mecánico

Proyecto final de Bases de Datos: aplicación de consola en C# con SQLite para administrar clientes y vehículos, registrar órdenes de servicio con una transacción y consultar seis reportes.

## Requisitos y ejecución

- SDK de .NET 10 y su entorno de ejecución.
- Acceso a NuGet en la primera compilación para restaurar [Microsoft.Data.Sqlite 10.0.15](https://www.nuget.org/packages/Microsoft.Data.Sqlite/10.0.15).
- Conservar la estructura del repositorio, incluido `db/taller.db`.

Desde la raíz del proyecto:

```powershell
dotnet run --project src/Taller
```

Para comprobar la compilación y tratar las advertencias como errores:

```powershell
dotnet build src/Taller -warnaserror
```

El menú permite administrar clientes, administrar vehículos, registrar una orden y consultar reportes. La opción 0 termina el programa; en los submenús regresa al menú principal.

La conexión busca `db/taller.db` subiendo desde la carpeta del ejecutable. No crea una base vacía si falta el archivo. Cada conexión activa `PRAGMA foreign_keys = ON` para comprobar las relaciones.

La base de ejemplo ya está incluida. Si se desea reiniciarla y se dispone de sqlite3, ejecutar desde la raíz:

```powershell
sqlite3 db/taller.db ".read db/script.sql"
```

Este comando elimina las tablas y vuelve a cargar los datos del script; descarta los cambios registrados en la aplicación.

## Estructura de carpetas

```text
db/
  script.sql             Definición de tablas y datos de ejemplo
  taller.db              Base SQLite utilizada por la aplicación
docs/
  modelo.md              Entidades, relaciones, diagrama y preguntas de defensa
src/Taller/
  Taller.csproj          Configuración de .NET y dependencia SQLite
  Program.cs             Conexión inicial y menú principal
  Modelos/               Cliente, Vehiculo y DetalleOrden
  Datos/                 Conexión, errores, CRUD, transacción y consultas
  UI/                    Entrada por consola y menús
README.md                Ejecución y ubicación de requisitos
```

## Requisito → archivo donde se cumple

| Requisito | Archivo y ubicación |
|---|---|
| Seis tablas | [db/script.sql](db/script.sql#L11): cliente, vehiculo, mecanico, servicio, orden y orden_servicio |
| Relación 1:N cliente–vehículo | [db/script.sql](db/script.sql#L26), FK de vehiculo hacia cliente |
| Relación N:M orden–servicio con cantidad y precio | [db/script.sql](db/script.sql#L52), tabla orden_servicio con PK compuesta |
| PRIMARY KEY | [db/script.sql](db/script.sql#L12) |
| FOREIGN KEY | [db/script.sql](db/script.sql#L26) |
| NOT NULL | [db/script.sql](db/script.sql#L13) |
| UNIQUE | [db/script.sql](db/script.sql#L14) |
| CHECK | [db/script.sql](db/script.sql#L24) |
| DEFAULT | [db/script.sql](db/script.sql#L16) |
| Diez o más filas por tabla principal | [db/script.sql](db/script.sql#L63), INSERT de las seis tablas |
| PRAGMA al abrir cada conexión | [Db.cs](src/Taller/Datos/Db.cs#L22), apertura seguida del PRAGMA |
| Menú principal completo | [Program.cs](src/Taller/Program.cs#L21) |
| CRUD de clientes | [ClienteDatos.cs](src/Taller/Datos/ClienteDatos.cs#L7) y [MenuClientes.cs](src/Taller/UI/MenuClientes.cs#L9) |
| CRUD de vehículos | [VehiculoDatos.cs](src/Taller/Datos/VehiculoDatos.cs#L7) y [MenuVehiculos.cs](src/Taller/UI/MenuVehiculos.cs#L9) |
| SQL con parámetros | [ClienteDatos.cs](src/Taller/Datos/ClienteDatos.cs#L51), [VehiculoDatos.cs](src/Taller/Datos/VehiculoDatos.cs#L58), [OrdenDatos.cs](src/Taller/Datos/OrdenDatos.cs#L42) y [ReporteDatos.cs](src/Taller/Datos/ReporteDatos.cs#L89) |
| Conexiones liberadas mediante using | [ClienteDatos.cs](src/Taller/Datos/ClienteDatos.cs#L9), mismo patrón en las demás clases de datos |
| Transacción que modifica dos tablas | [OrdenDatos.cs](src/Taller/Datos/OrdenDatos.cs#L31), inserciones en orden y orden_servicio, Commit y Rollback |
| Mensajes de SqliteException | [Errores.cs](src/Taller/Datos/Errores.cs#L7), utilizados por los menús |
| R1: INNER JOIN | [ReporteDatos.cs](src/Taller/Datos/ReporteDatos.cs#L8) |
| R2: LEFT JOIN | [ReporteDatos.cs](src/Taller/Datos/ReporteDatos.cs#L18) |
| R3: GROUP BY y HAVING | [ReporteDatos.cs](src/Taller/Datos/ReporteDatos.cs#L26) |
| R4: subconsulta | [ReporteDatos.cs](src/Taller/Datos/ReporteDatos.cs#L35) |
| R5: LIKE, AND y OR | [ReporteDatos.cs](src/Taller/Datos/ReporteDatos.cs#L44), comodines en el parámetro en la línea 114 |
| R6: ExecuteScalar | [ReporteDatos.cs](src/Taller/Datos/ReporteDatos.cs#L129), suma de órdenes terminadas y conversión de NULL a cero |
| Submenú de seis reportes | [MenuReportes.cs](src/Taller/UI/MenuReportes.cs#L10) |
| Compatibilidad con .NET 10 | [Taller.csproj](src/Taller/Taller.csproj#L5) |

R3 cuenta las órdenes que incluyen cada servicio, no la suma de sus cantidades. Sus ingresos incluyen órdenes abiertas y terminadas; R6 considera únicamente las terminadas. Los importes se expresan en pesos mexicanos.

## Verificación de la base de ejemplo

Se regeneró una copia temporal desde el script y se compararon los conteos con la base incluida. Coincidieron en las seis tablas; no fue necesario reemplazar `taller.db`.

| Tabla | Filas en taller.db | Filas en la copia |
|---|---:|---:|
| cliente | 12 | 12 |
| vehiculo | 15 | 15 |
| mecanico | 10 | 10 |
| servicio | 10 | 10 |
| orden | 15 | 15 |
| orden_servicio | 25 | 25 |

La comprobación de integridad devolvió `ok` y no se encontraron referencias inválidas. Esta comparación acredita los conteos, no la igualdad de cada valor almacenado.

La revisión de todo el código fuente no encontró SQL construido mediante concatenación, interpolación ni `string.Format`. Los signos `+` usados para presentar mensajes no forman consultas. En la búsqueda LIKE, los comodines se añaden al valor del parámetro.

Los seis comentarios de los reportes identifican las técnicas requeridas y se conservaron por su utilidad para la defensa. No se encontraron textos «Próximamente» ni código muerto en la revisión manual.

## Pruebas de revisión

El proyecto se dirige a `net10.0` y utiliza Microsoft.Data.Sqlite 10.0.15. Se ejecutó `dotnet run --project src/Taller` de forma nativa en .NET 10, sin variables temporales ni ajustes de runtime: abrió el menú, conectó a la base y el reporte de total facturado devolvió $20,150.00.

| Opción | Comprobación |
|---|---|
| Clientes | Alta, listado, consulta, modificación, cancelación de baja, baja y regreso |
| Vehículos | Alta con placa en mayúsculas, listado, consulta, modificación, baja y regreso |
| Registrar orden | Orden válida con dos servicios y total de $2,450.00; comprobada con SELECT |
| Reportes | Ejecución de los seis reportes; R3 probado con mínimos 0 y 3000 |
| Salir | Terminación normal con la opción 0 |

Se eliminaron los registros creados durante la prueba y se compararon los datos completos de las seis tablas antes y después: quedaron iguales.

Las inserciones y eliminaciones de prueba modificaron los bytes internos de `taller.db`, por lo que Git puede marcarlo como modificado aunque sus registros sean los mismos. No se reemplazó la base por la copia regenerada.
