# This script contains the creation of tables that'll be used on the project

CREATE TABLE Empleados (
  id_empleado NUMERIC(8) NOT NULL,
  nombre VARCHAR(30) NULL,
  ap_paterno VARCHAR(30) NULL,
  ap_materno VARCHAR(30) NULL,
  clave VARCHAR(30) NOT NULL,
constraint pk_emplados primary key(id_empleado)
);

CREATE TABLE FolioImpresion (
  num_Folio VARCHAR(30) NOT NULL,
constraint pk_FolImp primary key(num_Folio)
);

CREATE TABLE PRESENTADO (
  id_PRESENTADO VARCHAR(1) NOT NULL,
  DESCRIPCIÓN VARCHAR(30) NOT NULL,
constraint pk_presentado primary key(id_PRESENTADO),
CONSTRAINT ck_presentado CHECK(id_PRESENTADO = 'V' OR id_PRESENTADO= 'M')
);

CREATE TABLE GENERO (
  id_GENERO VARCHAR(30) NOT NULL,
  DESCRIPCIÓN VARCHAR(30) NOT NULL,
constraint pk_gen primary key(id_GENERO),
CONSTRAINT ck_gen CHECK (id_GENERO = 'M' OR id_GENERO = 'F')
);

CREATE TABLE pais (
  id_pais VARCHAR(30) NOT NULL,
  nombre VARCHAR(30) NOT NULL,
  nacionalidad varchar(30) NOT NULL,
constraint pk_Pais primary key(id_Pais)
);

CREATE TABLE entidad (
  id_entidad VARCHAR(30) NOT NULL,
  id_pais VARCHAR(30) NOT NULL,
  nombre varchar(50) NOT NULL,
constraint pk_Entidad primary key(id_entidad),
constraint fk_EntPais foreign key(id_pais) references pais(id_pais)
);

CREATE TABLE municipio (
  id_Municipio VARCHAR(30) NOT NULL,
  id_entidad VARCHAR(30) NOT NULL,
  nombre VARCHAR(40) NOT NULL,
constraint pk_municipio primary key(id_Municipio),
constraint fk_MunEntidad foreign key(id_entidad) references entidad(id_entidad)
);

CREATE TABLE ELEMENTOS_REGISTRO (
  NO_OFICIALIA NUMERIC(10) NOT NULL,
  NO_LIBRO NUMERIC(10) NOT NULL,
  NO_ACTA NUMERIC(10) NOT NULL,
  id_Municipio VARCHAR(30) NOT NULL,
  FECHA_REGISTRO DATE NOT NULL,
  nombreOficialMayor VARCHAR(30) NOT NULL,
  apPaternoOficialMayor VARCHAR(30) NOT NULL,
  no_Tramite NUMERIC(10) NOT NULL,
  apMaternoOficialMayor VARCHAR(30) NOT NULL,
  nombreAsienta VARCHAR(30) NOT NULL,
  apPaternoAsienta VARCHAR(30) NOT NULL,
  apMaternoAsienta VARCHAR(30) NOT NULL,
constraint pk_ElmReg primary key(NO_OFICIALIA, NO_LIBRO, NO_ACTA),
constraint fk_Municipio foreign key(id_Municipio) references municipio(id_Municipio)
);


CREATE TABLE AbuelosMaternos (
  id_Abuelo VARCHAR(30) NOT NULL,
  id_Abuela VARCHAR(30)  NOT NULL,
  id_pais VARCHAR(30) NOT NULL,
  nomAbuelo VARCHAR(30) NULL,
  apPaternoAbuelo VARCHAR(30) NULL,
  apMaternoAbuelo VARCHAR(30) NULL,
  nomAbuela VARCHAR(30) NULL,
  apPaternoAbuela VARCHAR(30) NULL,
  apMaternoAbuela VARCHAR(30) NULL,
constraint pk_AbuMat primary key(id_Abuelo, id_abuela),
constraint fk_matNac foreign key(id_pais) references pais (id_pais)
);

CREATE TABLE AbuelosPaternos (
  id_Abuelo VARCHAR(30) NOT NULL,
  id_Abuela VARCHAR(30) NOT NULL,
  id_pais VARCHAR(30) NOT NULL,
  nomAbuelo VARCHAR(30) NULL,
  apPaternoAbuelo VARCHAR(30) NULL,
  apMaternoAbuelo VARCHAR(30) NULL,
  nomAbuela VARCHAR(30) NULL,
  apPaternoAbuela VARCHAR(30) NULL,
  apMaternoAbuela VARCHAR(30) NULL,
constraint pk_AbuPat primary key(id_Abuelo, id_Abuela),
constraint fk_patNac foreign key(id_pais) references pais (id_pais)
);


CREATE TABLE madre (
  Curp VARCHAR(30) NOT NULL,
  id_pais VARCHAR(30) NOT NULL,
  nombres VARCHAR(30) NOT NULL,
  ap_paterno VARCHAR(30) NOT NULL,
  ap_materno VARCHAR(30) NOT NULL,
  edad NUMERIC NOT NULL,
constraint pk_CurpMot primary key(Curp),
constraint fk_MamaNac foreign key(id_pais) references pais (id_pais)
);



CREATE TABLE padre (
  curp VARCHAR(30) NOT NULL,
  id_pais VARCHAR(30) NOT NULL,
  nombres VARCHAR(30) NOT NULL,
  ap_paterno VARCHAR(30) NOT NULL,
  ap_materno VARCHAR(30) NOT NULL,
  edad NUMERIC NOT NULL,
constraint pk_CurpFat primary key(Curp),
constraint fk_PapaNac foreign key(id_pais) references pais (id_pais)
);



CREATE TABLE PESONA_REGISTRADA (
  curp VARCHAR(30) NOT NULL,
  madre_Curp VARCHAR(30) NULL,
  padre_curp VARCHAR(30) NULL,
  id_AbueloPat VARCHAR(30) NULL,
  id_AbuelaPat VARCHAR(30) NULL,
  id_AbueloMat VARCHAR(30) NULL,
  id_AbuelaMat VARCHAR(30) NULL,
  idMunicipio VARCHAR(30) NOT NULL,
  id_GENERO VARCHAR(30) NOT NULL,
  idPRESENTADO VARCHAR(1) NOT NULL,
  NO_ACTA NUMERIC(10) NOT NULL,
  NO_LIBRO NUMERIC(10) NOT NULL,
  NO_OFICIALIA NUMERIC(10) NOT NULL,
  NOMBRES VARCHAR(30) NOT NULL,
  APELLIDO_MATERNO VARCHAR(30) NOT NULL,
  APELLIDO_PATERNO VARCHAR(30) NOT NULL,
  FECHA_NACIMIENTO DATE NOT NULL,
  HORA_NAC TIME NOT NULL,
  CRIP VARCHAR(50) NOT NULL,
constraint pk_PersReg primary key(Curp),
constraint fk_maReg foreign key(madre_Curp) references madre (curp),
constraint fk_PaReg foreign key(padre_curp) references padre (curp),
constraint fk_AbuMaReg foreign key(id_AbueloMat, id_AbuelaMat) references AbuelosMaternos (id_Abuelo, id_abuela),
constraint fk_AbuPaReg foreign key(id_AbueloPat, id_AbuelaPat) references AbuelosPaternos (id_Abuelo, id_Abuela),
);

--------------------------CREACIÓN DE BASE DATOS--------------------
CREATE DATABASE bd_actas
--------------------------CREACIÓN DE USUARIOS--------------------
Administrador:
user: admin_actas2
password: its2023
