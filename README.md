[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=EL-BID_RepositorioMapainversiones&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=EL-BID_RepositorioMapainversiones)

# Repositorio MapaInversiones

El Repisotorio MapaInversiones pone a disposición de los gobiernos subnacionales un Repositorio de Proyectos de Inversión Pública con los siguientes objetivos:

*a.* Mejorar la trasparencia y rendición de cuentas sobre el uso de recursos públicos.

*b.* Fortalecer la eficiencia en la ejecución de proyectos de inversión pública.

*c.* Brindar appoyo a la toma de decisiones basadas en datos.


## Conceptos básicos

**Costo Planeado**           Costo planeado incial del proyecto

**Costo Programado**		Costo del proyecto ajustado con las adiciones y los costos previstos para cada producto del proyecto.

**Entidad Ejecutora**	        Representa el listado de entidades que ejecutan los proyectos.

**Entidad Financiadora**	Representa el listado de las entidades que pueden financiar los proyectos.

**Entidad Gubernamental**	Hace referencia al ente territorial para el cual se instala el sistema.

**Entidad Responsable**	        Cada una de las áreas u oficinas de la entidad gubernamental que es responsable de uno o varios proyectos.

**Fuente de Financiación**	Representa cómo se financia un proyecto. En cada fuente se registra la entidad financiador y el tipo de financiamiento (prestamo, donación, recursos propios, APP).

**Etapa Pago**                   Representa el listado de posibles etapas de un pago (presentado, aprobado, pagado).

**Etapa Proyecto**          Representa el listado de estados posibles de un proyecto (planeación, ejecución, terminado).

**Producto:**               Representa los productos que se estarán generado para cumplir con cada objetivo específico.

**Proyecto:**               Representa la información básica de los proyectos.

**Rol:**                       Representa el listado de roles que puede tener un actor dentro de la ejecución de los proyectos.

**Sector:**	                    Representa el listado de sectores bajo los que se ejecutan los proyectos. Ejemplo. Educación, vías, salud.


## Guía de usuario

Al ingresar al Repositorio el sistema presenta un tablero de control que resume la información del portafolio de proyectos registrado en la base de datos. 

Desde el tablero de control el usuario puede seleccionar el proyecto que quiere ver o trabajar y pasará al tablero de resumen del proyecto. Donde se presenta consolidada toda la información del proyecto seleccionado. Dependiendo del tipo de usuario, se podrá actualizar la información.

Para la creación de un proyecto el usuario puede ingresar la información básica del proyecto: Nombre, Sector, Subsector, Área responsable, Entidad Ejecutora, Etapa, ODS, Costo estimado, fechas de inicio y terminación planeadas, Descripción y Objetivos. En un mapa permite registrar la Ubicación del proyecto. Adicionalmente se registran las fuentes de financiamiento y se pueden anexar documentos relacionados.

Durante la ejecución del proyecto el sistema registra pagos, adiciones de costo programado, extensiones de tiempo de ejecución. También permite el registro de imágenes y videos que muestren el avance del proyecto.

Para todos los usuarios autorizados, adicional al tablero de control del portafolio y al tablero de resumen del proyecto, ofrece una serie de reportes relacionados con los proyectos y sus pagos, adiciones y extensiones.

El Manual de Usario se puede descargar en  [Manual de Usuario](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/UserManual.md).


#### Usuario

El acceso al sistema está restringido a los usuarios registrados, generalmente usuarios que hacer parte de la administración pública del ente gubernamental. Para tener acceso a la información del sistema, es necesario ser un usuario registrado. El registro lo realiza cada usuario y un administrador debe aprobar la solicitud asignándole el tipo de usuario que corresponda.

- Dirección
- Operación
- Consulta
- Administración

La dirección registra la información estructural del proyecto y una vez registrada solo puede modificarse mediante un proceso especial a través de Administración. 

La información periódica del proyecto es registrada por la Operación y aprobada por la Dirección. El nivel de operación registra la información periódica y esta queda en la etapa “registrada”. Una vez la información periódica es registrada en el Repositorio, debe ser aprobada por la dirección. Sólo la información que se encuentre en la etapa “aprobada” es considerada para los reportes y cuadros de ejecución real del proyecto.

Usuarios de tipo consulta tienen acceso a la información, tableros y reportes, pero no pueden crear ni modificar ningún tipo de dato.

Los usuarios que no se encuentren registrados en el sistema pueden acceder a la información disponible en MapaInversiones. El acceso al Repositorio MapaInversiones es de acceso restringido a usuarios registrados y aprobados por el ente gubernamental, generalmente funcionarios o colaboradores de la administración pública.


#### Administrador

Los usuarios de tipo Administración, son los responsables de la configuración del sistema, de aprobar nuevos usuarios y de actualizar información de acceso restringido para los demás niveles de usuario. Esto incluye la información estructural del proyecto o ajustes a la información periódica de periodos que ya no estén activos; para la realización de estas modificaciones el gobierno debe establecer los procesos y soportes que deben acompañar estas solicitudes.

Como parte de la configuración del sistema son los reponsables de mantener actualizada la información de: Áreas Responsables, Entidades Ejecutoras, Fuentes de Financiamiento, Sectores y Subsectores.

En la Administración de Usuarios es el responsable de activar o desactivar los usuarios registrados, aprobar las solicitudes de registro y asignar el perfil de usuario que corresponde a cada usuario. También puede restablecer la clave de acceso de un usuario.

Adicionalmente tiene acceso a toda la información de proyectos y puede realizar ajustes a la información histórica de los proyectos y su ejecución. 

Es importante resaltar la importancia de que la entidad gubernamental cuente con protocoles establecidos que determinen los procesos y soportes para las solicitudes de autorizaciones o modificaciones a las administración del sistema.


#### Manual

Aquí puede ver el  [Manual de Usuario](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/UserManual.md).
 	

## Guía de instalación

#### Bases de Datos

Esta herramienta tiene su base de datos creada en Microsoft SQL Server 2019. La base datos se puede descargar de [MapaInversionesC4D](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/DeploymentDbModel/InvestmentMapRepositoryC4D.zip) y cargarla en el servidor de base de datos que se configure para su publicación. 

Pasos para la instalación y configuración de la base de datos:
Restaurar el backup de la base de datos.
Actualizar la cadena de conexión en el código del sistema.
Cargar la información de las tablas de configuración.
Registrar y autorizar usuarios.

Para un mayor detalle puede consultar el [Manual de Instalación de Base de Datos](https://github.com/EL-BID/RepositorioMapaInversiones/tree/main/Manuals/DbDeployment/DatabaseDeploymentManual.md). 


#### Primeros pasos

1. Configure la conexión del tipo de publicación que desea realizar (Web deploy, FTP, file system, etc).
2. Configure el perfil de publicación, teniendo en cuenta los siguientes parametros:

    2.1. Configuración (Debug o Release - Any CPU)
    
    2.2. Target framework: .Net 6.0
    
    2.3. Seleccione el tipo de despliegue que quiere realizar. Existen dos opciones:
   	Framework-dependent deployment (El framework de .Net Core debe estar preinstalado).
   	Self-contained deployment (Contiene los binarios de .Net Core).

    2.4. Target runtime: Seleccione el runtime del servidor donde instalará la plataforma (Windows, linux, MacOS)

3. Realice la compilación y publicación de la solución.


#### Instalación en IIS

4. Instale o verifique que el framework de .Net Core 3.1 este instalado en el servidor que hospedará la aplicación (https://dotnet.microsoft.com/en-us/download/dotnet/6.0).
5. Como esta es una plataforma web el runtime requerido es ASP.NET Core Runtime 6.0.30
6. Se recomienda instalar el Windows Hosting Bundle que contiene el runtime y el ASP.NET Core Module.
8. Para el correcto funcionamiento de la aplicación, se requiere crear un sitio web, no funciona como aplicación virtual.
9. Crear un sitio web definiendo los parametros:	
   	Puerto (Ej: 80, 81, 443)
   	Protocolo (HTTP o HTTPS)
   	Certificado de seguridad (Opcional)
   	Ruta de archivos (La carpeta raíz donde se encuentra la app)
10. Revise las cadenas de conexión a base de datos.
11. Inicie o reinicie el sitio web.


#### Dependencias
- Microsoft Windows Server 2019 Datacenter o superior
- Microsoft SQL server 2019 estándar edition SP2 o superior: Motor de bases de datos relacionales.
- Internet information server (IIS): Servidor de aplicaciones web, se requiere la versión 10 o superior.
- .NET core 6.0.
- Microsoft Visual Studio 2019 o superior: IDE de desarrollo.

---
## Autor/es:
- [Juan Cruz Vieyra](https://www.linkedin.com/in/juan-cruz-vieyra-345b253/ "Juan Cruz Vieyra")
- [Sebastian del Hoyo](https://www.linkedin.com/in/sebastiandelhoyo/ "Sebastian del Hoyo")
- [José Niño](https://www.linkedin.com/in/jose-ni%C3%B1o-a2a8a731/ "José Niño")
- [Jaime Alberto Osorio](mailto:jaime.osorio@yahoo.com  "Jaime Alberto Osorio")
- [Wilson Muñoz Camelo](https://www.linkedin.com/in/wilson-mu%C3%B1oz-camelo-24b11324/ "Wilson Muñoz Camelo")
- [David Olaciregui](https://www.linkedin.com/in/david-olaciregui-35196015/ "David Olaciregui")
- [Anyela Milena Chavarro Muñoz ](https://www.linkedin.com/in/anyela-milena-chavarro-mu%C3%B1oz-0a79a524/ "Anyela Milena Chavarro Muñoz ")
- [Vladimiro Bellini](https://www.linkedin.com/in/vladimirobellini/ "Vladimiro Bellini")
- [Andrés Felipe Villamizar Vecino](mailto:villamizarvecino@hotmail.com "Andrés Felipe Villamizar Vecino")
- [Luis Mendez](https://www.linkedin.com/in/luisefe80/ "Luis Mendez")

## Licencia 
Licencia BID [LICENSE](https://github.com/EL-BID/RepositorioMapaInversiones/tree/main/LICENSE.md)

## Limitación de responsabilidades

El BID no será responsable, bajo circunstancia alguna, de daño ni indemnización, moral o patrimonial; directo o indirecto; accesorio o especial; o por vía de consecuencia, previsto o imprevisto, que pudiese surgir:

i. Bajo cualquier teoría de responsabilidad, ya sea por contrato, infracción de derechos de propiedad intelectual, negligencia o bajo cualquier otra teoría; y/o

ii. A raíz del uso de la Herramienta Digital, incluyendo, pero sin limitación de potenciales defectos en la Herramienta Digital, o la pérdida o inexactitud de los datos de cualquier tipo. Lo anterior incluye los gastos o daños asociados a fallas de comunicación y/o fallas de funcionamiento de computadoras, vinculados con la utilización de la Herramienta Digital.
