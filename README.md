
# Repositorio MapaInversiones

El Repositorio MapaInversiones se pone a disposición de los gobiernos con los siguientes objetivos:

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

El Manual de Usuario se puede descargar en  [Manual de Usuario](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/UserManual.md).


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

Adicionalmente se puede encontrar una base de datos con datos sintéticos en: [IMRepo Synthetic Data Database](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/DeploymentDbModel/IM_Repository_Synthetic_Data_Database.sql)

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

## Acknowledgments / Reconocimientos

**Copyright © [2026]. Inter-American Development Bank ("IDB"). Authorized Use.**  
The procedures and results obtained based on the execution of this software are those programmed by the developers and do not necessarily reflect the views of the IDB, its Board of Executive Directors or the countries it represents.

**Copyright © [2026]. Banco Interamericano de Desarrollo ("BID"). Uso Autorizado.**  
Los procedimientos y resultados obtenidos con la ejecución de este software son los programados por los desarrolladores y no reflejan necesariamente las opiniones del BID, su Directorio Ejecutivo ni los países que representa.

### Support and Usage Documentation / Documentación de Soporte y Uso

**Copyright © [2026]. Inter-American Development Bank ("IDB").** The Support and Usage Documentation is licensed under the Creative Commons License CC-BY 4.0 license. The opinions expressed in the Support and Usage Documentation are those of its authors and do not necessarily reflect the opinions of the IDB, its Board of Executive Directors, or the countries it represents.

**Copyright © [2026]. Banco Interamericano de Desarrollo (BID).** La Documentación de Soporte y Uso está licenciada bajo la licencia Creative Commons CC-BY 4.0. Las opiniones expresadas en la Documentación de Soporte y Uso son las de sus autores y no reflejan necesariamente las opiniones del BID, su Directorio Ejecutivo ni los países que representa.

### AI-Powered Services Disclaimer / Exención de responsabilidad por Servicios Impulsados por IA

The Software may include features which use, are powered by, or are an artificial intelligence system (“AI-Powered Services”), and as a result, the services provided via the Software may not be completely error-free or up to date. Additionally, the User acknowledges that due to the incorporation of AI-Powered Services in the Software, the Software may not dynamically (in “real time”) retrieve information and that, consequently, the output provided to the User may not account for events, updates, or other facts that have occurred or become available after the Software was trained. Accordingly, the User acknowledges that the use of the Software, and that any actions taken or reliance on such products, are at the User’s own risk, and the User acknowledges that the User must independently verify any information provided by the Software.

El Software puede incluir funciones que utilizan, están impulsadas por o son un sistema de inteligencia artificial (“Servicios Impulsados por IA”) y, como resultado, los servicios proporcionados a través del Software pueden no estar completamente libres de errores ni actualizados. Además, el Usuario reconoce que, debido a la incorporación de Servicios Impulsados por IA en el Software, este puede no recuperar información dinámicamente (en “tiempo real”) y que, en consecuencia, la información proporcionada al Usuario puede no reflejar eventos, actualizaciones u otros hechos que hayan ocurrido o estén disponibles después del entrenamiento del Software. En consecuencia, el Usuario reconoce que el uso del Software, y que cualquier acción realizada o la confianza depositada en dichos productos, se realiza bajo su propio riesgo, y reconoce que debe verificar de forma independiente cualquier información proporcionada por el Software.
