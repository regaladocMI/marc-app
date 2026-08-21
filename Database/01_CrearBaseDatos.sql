/*============================================================================
  MARC - Tutor de Ingles Conversacional
  Script de creacion de base de datos
  Motor: Microsoft SQL Server | Esquema: marc.* (13 tablas)
============================================================================*/

/* ---------- 1. Base de datos y esquema ---------- */

USE master;
GO

IF DB_ID('MarcDB') IS NOT NULL
BEGIN
    ALTER DATABASE MarcDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE MarcDB;
END
GO

CREATE DATABASE MarcDB;
GO

USE MarcDB;
GO

CREATE SCHEMA marc;
GO


/* ---------- 2. Tablas de catalogo ---------- */

IF OBJECT_ID('marc.nivel_ingles', 'U') IS NOT NULL DROP TABLE marc.nivel_ingles;
GO
CREATE TABLE marc.nivel_ingles (
    id_nivel_ingles INT IDENTITY(1,1) PRIMARY KEY,
    codigo          VARCHAR(2)  NOT NULL UNIQUE,
    descripcion     VARCHAR(50) NOT NULL
);
GO

IF OBJECT_ID('marc.tipo_emisor', 'U') IS NOT NULL DROP TABLE marc.tipo_emisor;
GO
CREATE TABLE marc.tipo_emisor (
    id_tipo_emisor INT IDENTITY(1,1) PRIMARY KEY,
    nombre         VARCHAR(30) NOT NULL UNIQUE
);
GO

IF OBJECT_ID('marc.modo_conversacion', 'U') IS NOT NULL DROP TABLE marc.modo_conversacion;
GO
CREATE TABLE marc.modo_conversacion (
    id_modo_conversacion INT IDENTITY(1,1) PRIMARY KEY,
    nombre                VARCHAR(30) NOT NULL UNIQUE
);
GO

IF OBJECT_ID('marc.tipo_error', 'U') IS NOT NULL DROP TABLE marc.tipo_error;
GO
CREATE TABLE marc.tipo_error (
    id_tipo_error INT IDENTITY(1,1) PRIMARY KEY,
    nombre        VARCHAR(50) NOT NULL UNIQUE
);
GO


/* ---------- 3. Usuario ---------- */

IF OBJECT_ID('marc.usuario', 'U') IS NOT NULL DROP TABLE marc.usuario;
GO
CREATE TABLE marc.usuario (
    id_usuario     INT IDENTITY(1,1) PRIMARY KEY,
    nombre_usuario VARCHAR(100) NOT NULL,
    correo         VARCHAR(150) NULL,
    fecha_registro DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);
GO


/* ---------- 4. Temas y contexto ---------- */

IF OBJECT_ID('marc.tema', 'U') IS NOT NULL DROP TABLE marc.tema;
GO
CREATE TABLE marc.tema (
    id_tema         INT IDENTITY(1,1) PRIMARY KEY,
    id_usuario      INT NOT NULL,
    id_nivel_ingles INT NOT NULL,
    nombre          VARCHAR(150) NOT NULL,
    prompt_base     VARCHAR(MAX) NOT NULL,
    activo          BIT NOT NULL DEFAULT 1,
    fecha_creacion  DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT fk_tema_usuario FOREIGN KEY (id_usuario)
        REFERENCES marc.usuario(id_usuario) ON DELETE CASCADE,
    CONSTRAINT fk_tema_nivelingles FOREIGN KEY (id_nivel_ingles)
        REFERENCES marc.nivel_ingles(id_nivel_ingles),
    CONSTRAINT uq_tema_usuario_nombre UNIQUE (id_usuario, nombre)
);
GO

IF OBJECT_ID('marc.tema_contexto', 'U') IS NOT NULL DROP TABLE marc.tema_contexto;
GO
CREATE TABLE marc.tema_contexto (
    id_tema_contexto INT IDENTITY(1,1) PRIMARY KEY,
    id_tema          INT NOT NULL,
    contenido        VARCHAR(MAX) NOT NULL,
    activo           BIT NOT NULL DEFAULT 1,
    fecha_creacion   DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT fk_temacontexto_tema FOREIGN KEY (id_tema)
        REFERENCES marc.tema(id_tema) ON DELETE CASCADE
);
GO


/* ---------- 5. Sesiones y mensajes ---------- */

IF OBJECT_ID('marc.sesion', 'U') IS NOT NULL DROP TABLE marc.sesion;
GO
CREATE TABLE marc.sesion (
    id_sesion        INT IDENTITY(1,1) PRIMARY KEY,
    id_usuario       INT NOT NULL,
    id_tema          INT NOT NULL,
    fecha_inicio     DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    fecha_fin        DATETIME2 NULL,
    puntaje_promedio DECIMAL(4,2) NULL,
    CONSTRAINT fk_sesion_usuario FOREIGN KEY (id_usuario)
        REFERENCES marc.usuario(id_usuario) ON DELETE CASCADE,
    CONSTRAINT fk_sesion_tema FOREIGN KEY (id_tema)
        REFERENCES marc.tema(id_tema) ON DELETE NO ACTION
);
GO
-- NOTA: la regla "maximo una sesion abierta (fecha_fin NULL) por usuario a
-- la vez" NO se blinda aqui con un indice filtrado (esa sintaxis es
-- especifica de SQL Server y rompe la portabilidad del script). Se controla
-- en Marc.Data: el repositorio de Sesion debe validar que no exista una
-- sesion abierta para el usuario antes de insertar una nueva.

IF OBJECT_ID('marc.mensaje', 'U') IS NOT NULL DROP TABLE marc.mensaje;
GO
CREATE TABLE marc.mensaje (
    id_mensaje     INT IDENTITY(1,1) PRIMARY KEY,
    id_sesion      INT NOT NULL,
    id_tipo_emisor INT NOT NULL,
    texto          VARCHAR(MAX) NOT NULL,
    orden          INT NOT NULL,
    puntaje        TINYINT NULL CHECK (puntaje IS NULL OR puntaje BETWEEN 1 AND 10),
    fecha_creacion DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT fk_mensaje_sesion FOREIGN KEY (id_sesion)
        REFERENCES marc.sesion(id_sesion) ON DELETE CASCADE,
    CONSTRAINT fk_mensaje_tipoemisor FOREIGN KEY (id_tipo_emisor)
        REFERENCES marc.tipo_emisor(id_tipo_emisor)
);
GO


/* ---------- 6. Correcciones y vocabulario ---------- */

IF OBJECT_ID('marc.correccion', 'U') IS NOT NULL DROP TABLE marc.correccion;
GO
CREATE TABLE marc.correccion (
    id_correccion    INT IDENTITY(1,1) PRIMARY KEY,
    id_mensaje       INT NOT NULL,
    id_tipo_error    INT NOT NULL,
    texto_original   VARCHAR(MAX) NOT NULL,
    texto_corregido  VARCHAR(MAX) NOT NULL,
    explicacion      VARCHAR(MAX) NULL,
    CONSTRAINT fk_correccion_mensaje FOREIGN KEY (id_mensaje)
        REFERENCES marc.mensaje(id_mensaje) ON DELETE CASCADE,
    CONSTRAINT fk_correccion_tipoerror FOREIGN KEY (id_tipo_error)
        REFERENCES marc.tipo_error(id_tipo_error)
);
GO

IF OBJECT_ID('marc.vocabulario', 'U') IS NOT NULL DROP TABLE marc.vocabulario;
GO
CREATE TABLE marc.vocabulario (
    id_vocabulario   INT IDENTITY(1,1) PRIMARY KEY,
    id_usuario       INT NOT NULL,
    palabra_o_frase  VARCHAR(200) NOT NULL,
    significado      VARCHAR(MAX) NULL,
    fecha_creacion   DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT fk_vocabulario_usuario FOREIGN KEY (id_usuario)
        REFERENCES marc.usuario(id_usuario) ON DELETE CASCADE,
    CONSTRAINT uq_vocabulario_usuario_palabra UNIQUE (id_usuario, palabra_o_frase)
);
GO

IF OBJECT_ID('marc.vocabulario_ocurrencia', 'U') IS NOT NULL DROP TABLE marc.vocabulario_ocurrencia;
GO
CREATE TABLE marc.vocabulario_ocurrencia (
    id_vocabulario_ocurrencia INT IDENTITY(1,1) PRIMARY KEY,
    id_vocabulario             INT NOT NULL,
    id_mensaje                 INT NOT NULL,
    fecha_ocurrencia           DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT fk_vocabulario_ocurrencia_vocabulario FOREIGN KEY (id_vocabulario)
        REFERENCES marc.vocabulario(id_vocabulario) ON DELETE NO ACTION,
    CONSTRAINT fk_vocabulario_ocurrencia_mensaje FOREIGN KEY (id_mensaje)
        REFERENCES marc.mensaje(id_mensaje) ON DELETE CASCADE
);
GO


/* ---------- 7. Configuracion ---------- */

IF OBJECT_ID('marc.configuracion_usuario', 'U') IS NOT NULL DROP TABLE marc.configuracion_usuario;
GO
CREATE TABLE marc.configuracion_usuario (
    id_configuracion_usuario INT IDENTITY(1,1) PRIMARY KEY,
    id_usuario                INT NOT NULL UNIQUE,
    id_modo_conversacion      INT NOT NULL,
    velocidad_habla           DECIMAL(3,2) NOT NULL DEFAULT 1.00,
    paciencia_segundos        DECIMAL(4,2) NOT NULL DEFAULT 2.00,
    ocultar_transcripcion     BIT NOT NULL DEFAULT 0,
    microfono_preferido       VARCHAR(200) NULL,
    salida_audio_preferida    VARCHAR(200) NULL,
    CONSTRAINT fk_configusuario_usuario FOREIGN KEY (id_usuario)
        REFERENCES marc.usuario(id_usuario) ON DELETE CASCADE,
    CONSTRAINT fk_configusuario_modoconversacion FOREIGN KEY (id_modo_conversacion)
        REFERENCES marc.modo_conversacion(id_modo_conversacion)
);
GO


/* ---------- 8. Indices sobre columnas FK ---------- */

CREATE INDEX ix_tema_usuario ON marc.tema(id_usuario);
CREATE INDEX ix_temacontexto_tema ON marc.tema_contexto(id_tema);
CREATE INDEX ix_sesion_usuario ON marc.sesion(id_usuario);
CREATE INDEX ix_sesion_tema ON marc.sesion(id_tema);
CREATE INDEX ix_mensaje_sesion ON marc.mensaje(id_sesion);
CREATE INDEX ix_correccion_mensaje ON marc.correccion(id_mensaje);
CREATE INDEX ix_vocabulario_usuario ON marc.vocabulario(id_usuario);
CREATE INDEX ix_vocabularioocurrencia_vocabulario ON marc.vocabulario_ocurrencia(id_vocabulario);
CREATE INDEX ix_vocabularioocurrencia_mensaje ON marc.vocabulario_ocurrencia(id_mensaje);
GO


/* ---------- 9. Datos de catalogo (semilla) ---------- */

INSERT INTO marc.nivel_ingles (codigo, descripcion) VALUES
    ('A1', 'Principiante'),
    ('A2', 'Basico'),
    ('B1', 'Intermedio'),
    ('B2', 'Intermedio alto'),
    ('C1', 'Avanzado'),
    ('C2', 'Dominio (nativo o casi nativo)');

INSERT INTO marc.tipo_emisor (nombre) VALUES
    ('Usuario'),
    ('Tutor');

INSERT INTO marc.modo_conversacion (nombre) VALUES
    ('Voz'),
    ('Texto'),
    ('Manual');

INSERT INTO marc.tipo_error (nombre) VALUES
    ('Gramatica'),
    ('Vocabulario'),
    ('Pronunciacion'),
    ('Uso y contexto');
GO

PRINT 'Base de datos MarcDB creada correctamente: 13 tablas + datos de catalogo.';
GO