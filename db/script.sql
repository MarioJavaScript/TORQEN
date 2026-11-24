PRAGMA foreign_keys = ON;

-- Eliminar primero las tablas dependientes.
DROP TABLE IF EXISTS orden_servicio;
DROP TABLE IF EXISTS orden;
DROP TABLE IF EXISTS vehiculo;
DROP TABLE IF EXISTS servicio;
DROP TABLE IF EXISTS mecanico;
DROP TABLE IF EXISTS cliente;

CREATE TABLE cliente (
    id_cliente INTEGER PRIMARY KEY,
    nombre TEXT NOT NULL,
    telefono TEXT NOT NULL UNIQUE,
    email TEXT,
    fecha_registro TEXT NOT NULL DEFAULT (date('now'))
);

CREATE TABLE vehiculo (
    id_vehiculo INTEGER PRIMARY KEY,
    placa TEXT NOT NULL UNIQUE,
    marca TEXT NOT NULL,
    modelo TEXT NOT NULL,
    anio INTEGER NOT NULL CHECK (anio BETWEEN 1980 AND 2027),
    id_cliente INTEGER NOT NULL,
    FOREIGN KEY (id_cliente) REFERENCES cliente(id_cliente)
);

CREATE TABLE mecanico (
    id_mecanico INTEGER PRIMARY KEY,
    nombre TEXT NOT NULL,
    especialidad TEXT NOT NULL,
    telefono TEXT
);

CREATE TABLE servicio (
    id_servicio INTEGER PRIMARY KEY,
    nombre TEXT NOT NULL UNIQUE,
    precio_base REAL NOT NULL CHECK (precio_base > 0)
);

CREATE TABLE orden (
    id_orden INTEGER PRIMARY KEY,
    fecha TEXT NOT NULL DEFAULT (date('now')),
    estado TEXT NOT NULL DEFAULT 'abierta' CHECK (estado IN ('abierta','terminada')),
    id_vehiculo INTEGER NOT NULL,
    id_mecanico INTEGER NOT NULL,
    FOREIGN KEY (id_vehiculo) REFERENCES vehiculo(id_vehiculo),
    FOREIGN KEY (id_mecanico) REFERENCES mecanico(id_mecanico)
);

CREATE TABLE orden_servicio (
    id_orden INTEGER NOT NULL,
    id_servicio INTEGER NOT NULL,
    cantidad INTEGER NOT NULL DEFAULT 1 CHECK (cantidad > 0),
    precio REAL NOT NULL CHECK (precio > 0),
    PRIMARY KEY (id_orden, id_servicio),
    FOREIGN KEY (id_orden) REFERENCES orden(id_orden),
    FOREIGN KEY (id_servicio) REFERENCES servicio(id_servicio)
);

-- Datos ficticios con referencias de Sonora; importes en pesos mexicanos.
INSERT INTO cliente (id_cliente, nombre, telefono, email) VALUES
(1, 'María Fernanda López García', '6623104501', 'maria.lopez@example.com'),
(2, 'José Luis Valenzuela Ruiz', '6623104502', 'jose.valenzuela@example.com'),
(3, 'Ana Isabel Robles Castro', '6442104503', 'ana.robles@example.com'),
(4, 'Carlos Alberto Encinas Soto', '6313104504', 'carlos.encinas@example.com'),
(5, 'Guadalupe Martínez Ochoa', '6222104505', NULL),
(6, 'Jesús Antonio Salazar León', '6421104506', 'jesus.salazar@example.com'),
(7, 'Rosa Elena Bórquez López', '6623104507', NULL),
(8, 'Francisco Javier Durazo Gil', '6341104508', 'francisco.durazo@example.com'),
(9, 'Claudia Patricia Noriega Vega', '6331104509', 'claudia.noriega@example.com'),
(10, 'Miguel Ángel Córdova Ríos', '6511104510', NULL),
(11, 'Teresa de Jesús Molina Cruz', '6623104511', 'teresa.molina@example.com'),
(12, 'Ramón Eduardo Félix Navarro', '6442104512', NULL);

-- Los clientes 11 y 12 no tienen vehículos.
INSERT INTO vehiculo (id_vehiculo, placa, marca, modelo, anio, id_cliente) VALUES
(1, 'WFA-123-A', 'Nissan', 'Versa', 2020, 1),
(2, 'WFB-245-B', 'Toyota', 'Hilux', 2018, 2),
(3, 'WFC-367-C', 'Chevrolet', 'Aveo', 2019, 3),
(4, 'WFD-489-D', 'Ford', 'Ranger', 2017, 4),
(5, 'WFE-512-E', 'Volkswagen', 'Jetta', 2021, 5),
(6, 'WFF-634-F', 'Honda', 'Civic', 2016, 6),
(7, 'WFG-756-G', 'Kia', 'Rio', 2022, 7),
(8, 'WFH-878-H', 'Mazda', 'Mazda3', 2023, 8),
(9, 'WFJ-901-J', 'Hyundai', 'Accent', 2020, 9),
(10, 'WFK-123-K', 'Dodge', 'Attitude', 2015, 10),
(11, 'WFL-245-L', 'Chevrolet', 'Silverado', 2014, 1),
(12, 'WFM-367-M', 'Toyota', 'Corolla', 2024, 2),
(13, 'WFN-489-N', 'Nissan', 'Sentra', 2013, 3),
(14, 'WFP-512-P', 'Ford', 'Explorer', 2019, 4),
(15, 'WFR-634-R', 'Volkswagen', 'Pointer', 2005, 5);

INSERT INTO mecanico (id_mecanico, nombre, especialidad, telefono) VALUES
(1, 'Héctor Manuel Leyva', 'Mantenimiento general', '6624104601'),
(2, 'Luis Fernando Armenta', 'Frenos', '6624104602'),
(3, 'Sergio Alfredo Gastélum', 'Suspensión', '6444104603'),
(4, 'Daniel Esteban Ibarra', 'Electricidad automotriz', '6314104604'),
(5, 'Adriana Beatriz Acosta', 'Diagnóstico electrónico', '6624104605'),
(6, 'Roberto Carlos Mendívil', 'Afinación', '6224104606'),
(7, 'Jorge Armando Verdugo', 'Transmisiones', '6424104607'),
(8, 'Patricia Alejandra Varela', 'Aire acondicionado', '6624104608'),
(9, 'Óscar Iván Camacho', 'Dirección y alineación', NULL),
(10, 'Manuel Alejandro Quijada', 'Sistema de enfriamiento', NULL);

-- Los servicios 9 y 10 quedan sin órdenes asociadas.
INSERT INTO servicio (id_servicio, nombre, precio_base) VALUES
(1, 'Cambio de aceite', 650.00),
(2, 'Afinación completa', 1800.00),
(3, 'Cambio de balatas', 1200.00),
(4, 'Alineación', 450.00),
(5, 'Balanceo por rueda', 150.00),
(6, 'Diagnóstico electrónico', 550.00),
(7, 'Servicio de aire acondicionado', 1600.00),
(8, 'Cambio de amortiguador', 1400.00),
(9, 'Reparación de transmisión', 8500.00),
(10, 'Limpieza de radiador', 950.00);

INSERT INTO orden (id_orden, fecha, estado, id_vehiculo, id_mecanico) VALUES
(1, '2026-01-12', 'terminada', 1, 1),
(2, '2026-02-03', 'terminada', 2, 6),
(3, '2026-03-17', 'terminada', 3, 2),
(4, '2026-04-08', 'terminada', 4, 9),
(5, '2026-05-21', 'terminada', 5, 5),
(6, '2026-06-10', 'terminada', 6, 8),
(7, '2026-07-14', 'terminada', 7, 3),
(8, '2026-08-05', 'terminada', 8, 1),
(9, '2026-09-19', 'terminada', 9, 6),
(10, '2026-10-02', 'terminada', 10, 2),
(11, '2026-11-18', 'abierta', 11, 4),
(12, '2026-11-19', 'abierta', 12, 9),
(13, '2026-11-20', 'abierta', 13, 7),
(14, '2026-11-23', 'abierta', 14, 10),
(15, '2026-11-24', 'abierta', 15, 8);

-- Cada precio conserva el valor unitario acordado en la orden.
INSERT INTO orden_servicio (id_orden, id_servicio, cantidad, precio) VALUES
(1, 1, 1, 650.00),
(2, 2, 1, 1800.00),
(2, 6, 1, 500.00),
(3, 3, 1, 1200.00),
(3, 4, 1, 450.00),
(4, 4, 1, 450.00),
(4, 5, 4, 150.00),
(5, 6, 1, 550.00),
(6, 7, 1, 1600.00),
(6, 1, 1, 700.00),
(7, 8, 4, 1400.00),
(7, 4, 1, 450.00),
(7, 5, 4, 150.00),
(8, 1, 1, 650.00),
(9, 2, 1, 1900.00),
(9, 3, 1, 1250.00),
(10, 3, 1, 1200.00),
(11, 6, 1, 550.00),
(12, 4, 1, 450.00),
(12, 5, 4, 160.00),
(13, 2, 1, 1800.00),
(14, 8, 2, 1500.00),
(14, 6, 1, 600.00),
(15, 7, 1, 1700.00),
(15, 1, 1, 650.00);

-- Verificar la cantidad de filas de cada tabla.
-- SELECT 'cliente' AS tabla, COUNT(*) AS filas FROM cliente
-- UNION ALL SELECT 'vehiculo', COUNT(*) FROM vehiculo
-- UNION ALL SELECT 'mecanico', COUNT(*) FROM mecanico
-- UNION ALL SELECT 'servicio', COUNT(*) FROM servicio
-- UNION ALL SELECT 'orden', COUNT(*) FROM orden
-- UNION ALL SELECT 'orden_servicio', COUNT(*) FROM orden_servicio;
