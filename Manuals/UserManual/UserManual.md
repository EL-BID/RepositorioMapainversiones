# Repositorio MapaInversiones

## Manual de Usuario

### versión 1.0 - mayo de 2024



El Repositorio MapaInversiones es un sistema de información para el registro y seguimiento del portafolio de proyectos de inversión pública en gobiernos subnacionales.


Es una herramienta gratuita y de código abierto que provee la información necesaria para alimentar MapaInversiones.



***

# Introducción


Las instituciones públicas, en sus diversos niveles de gobierno, desempeñan un papel crucial en la planificación, financiamiento, ejecución y supervisión de las inversiones públicas. Sin embargo, en este proceso, surgen frecuentemente desafíos técnicos, económicos, administrativos y políticos que dificultan la implementación de las mejores prácticas. Superar estos desafíos es esencial para promover el desarrollo sostenible, fortalecer la confianza ciudadana en las instituciones y asegurar el uso óptimo de los fondos públicos en beneficio de toda la comunidad.


Las herramientas tecnológicas ofrecen a los gobiernos la capacidad de gestionar sus proyectos de inversión pública de manera eficiente, garantizar la transparencia y fomentar la participación ciudadana en la toma de decisiones. Un ejemplo destacado es MapaInversiones, una iniciativa del Banco Interamericano de Desarrollo (BID) presente en 14 países de América Latina y el Caribe. Esta iniciativa promueve la transparencia en el gasto, las inversiones y las contrataciones públicas mediante plataformas digitales que integran y visualizan datos públicos. MapaInversiones actúa como una ventana visible de los datos publicados por entidades nacionales y territoriales en los bancos de proyectos de los Sistemas Nacionales de Inversión Pública (SNIP), entre otras fuentes oficiales. No obstante, a nivel subnacional, la adopción de sistemas de información que permiten la recopilación, análisis y difusión de datos en tiempo real varía considerablemente.


Para cerrar estas brechas y fomentar el desarrollo local, el BID ofrece una herramienta tecnológica libre, gratuita y de código abierto diseñada para mejorar los sistemas y fuentes de datos existentes sin sustituirlos. El Repositorio MapaInversiones es una aplicación web que automatiza la entrada de datos, limpia los datos existentes y genera información organizada y accesible para los tomadores de decisiones mediante interfaces intuitivas, componentes reutilizables y plantillas preconstruidas. Esto permite un rápido despliegue de aplicaciones sin necesidad de amplios conocimientos de desarrollo de software. El Repositorio puede personalizarse y adaptarse a las necesidades específicas de cada gobierno, y la información puede resguardarse en servidores locales o en la nube, según la decisión de cada entidad. La licencia de este software permite a los usuarios reproducir, distribuir, ejecutar públicamente, prestar, poner a disposición del público y modificar el software libremente.



#### Objetivos

Los objetivos de esta solución incluyen:

- Facilitar y mejorar la gestión de proyectos de inversión pública mediante una base de datos que consolide todos los proyectos de las entidades gubernamentales.
- Optimizar la ejecución de recursos de inversión pública en los gobiernos subnacionales, proporcionando una herramienta que facilite el seguimiento y la identificación de riesgos.
- Fortalecer la integración de datos, la visualización y las capacidades analíticas para apoyar la toma de decisiones basada en datos y la evaluación de resultados.
- Mejorar la divulgación de información sobre la inversión pública gracias a la interoperabilidad con las plataformas de transparencia de MapaInversiones.

Esta solución permite a los gobiernos locales enfocarse en superar los retos estratégicos en lugar de los técnicos y es escalable según sea necesario, adaptándose a los requisitos cambiantes y a la evolución de las necesidades de cada gobierno. 



***

# Generalidades


## Proyectos
El sistema funciona en torno a proyectos. Maneja la información de los proyectos de inversión pública de una entidad gubernamental del orden subnacional. Para cada proyecto administra información básica del proyecto, información financiera, avance físico y tiempos del proyecto.

Para el seguimiento registra la información de lo planeado y lo ejecutado, y con base en ella genera tableros de control y reportes que permiten hacer seguimiento al nivel de ejecución física y financiera de un proyecto o un grupo de proyectos.

Hay diferentes grupos de información en un proyecto. 
###### Información estructural 
Determina las características del proyecto y los valores iniciales planeados de tiempos y recursos financieros. Generalmente permanece constante durante la vida del proyecto el sistema permite registrar Adiciones y Extensiones.
###### Información periódica 
Registra la ejecución financiera y física durante la ejecución del proyecto. Esta información se maneja en periodos de tiempo definidos por el gobierno.

###### Adiciones
Realizan una variación en el costo programado del proyecto.

###### Extensiones
Realizan una variación en el tiempo de ejecución del proyecto.

La ejecución del proyecto se registra a través de los pagos.

###### Pagos
Indican el valor de un pago realizado y el porcentaje de avance que soporta la solicitud del pago. El valor pagado representa el avance financiero y el porcentaje representa el avance físico.



## Tipos de usuario

El acceso al sistema está restringido a los usuarios registrados, generalmente usuarios que hacer parte de la administración pública del ente gubernamental. Para tener acceso a la información del sistema, es necesario ser un usuario registrado. El registro lo realiza cada usuario y un administrador debe aprobar la solicitud asignándole el tipo de usuario que corresponda.

- Dirección
- Operación
- Consulta
- Administración

La dirección registra la información estructural del proyecto y una vez registrada solo puede modificarse mediante un proceso especial a través de Administración. 

La información periódica del proyecto es registrada por la Operación y aprobada por la Dirección. El nivel de operación registra la información periódica y esta queda en la etapa “registrada”. Una vez la información periódica es registrada en el Repositorio, debe ser aprobada por la dirección. Sólo la información que se encuentre en la etapa “aprobada” es considerada para los reportes y cuadros de ejecución real del proyecto.

Usuarios de tipo consulta tienen acceso a la información, tableros y reportes, pero no pueden crear ni modificar ningún tipo de dato.

Los usuarios de tipo Administración, son los responsables de la configuración del sistema, de aprobar nuevos usuarios y de actualizar información de acceso restringido para los demás niveles de usuario. Esto incluye la información estructural del proyecto o ajustes a la información periódica de periodos que ya no estén activos; para la realización de estas modificaciones el gobierno debe establecer los procesos y soportes que deben acompañar estas solicitudes.

Los usuarios que no se encuentren registrados en el sistema pueden acceder a la información disponible en MapaInversiones. El acceso al Repositorio MapaInversiones es de acceso restringido a usuarios registrados y aprobados por el ente gubernamental, generalmente funcionarios o colaboradores de la administración pública.



***

# Funcionalidad

Cuando el usuario ingresa al sistema es recibido con este tablero que muestra una síntesis de la composición y del estado actual de los proyectos que lo componen.


## Tablero de gestión de proyectos

El tablero de gestión de proyectos presenta al usuario una síntesis la composición y el estado actual del portafolio de proyectos de inversión pública de la entidad.


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Tablero_de_gestion_de_proyectos_2.png)

En cualquier momento, durante la navegación por el sistema, se puede regresar al Tablero de Gestión de Proyectos usando el ícono del Repositorio de MapaInversiones.


#### Filtros
El tablero presenta la información de todos los proyectos de la entidad gubernamental. Se pueden aplicar uno o varios filtros que se muestre en el tablero que correspondan a las opciones seleccionadas. Filtros disponibles:
- **Área Responsable**: El tablero presenta las cifras consolidadas correspondientes a los proyectos que son responsabilidad del área seleccionada.
- **Sector**: El tablero presenta las cifras consolidadas correspondientes a los proyectos que son responsabilidad del sector seleccionado.
- **Subsector**: El tablero presenta las cifras consolidadas correspondientes a los proyectos que son responsabilidad del subsector seleccionado. Para poder seleccionar un subsector es necesario seleccionar primero al sector al que pertenece el subsector.


### Grupos de Información Presentada

###### Mapa de Ubicación de los proyectos
El primer segmento presenta un mapa que muestra la ubicación de todos los proyectos que componen el portafolio.

###### Proyectos no Iniciados
Muestra información correspondiente a los proyectos que se encuentran en etapa de Planeación. Cantidad de proyectos y el costo total planeado.

###### Proyectos en ejecución
Muestra información correspondiente a los proyectos que se encuentran en etapa de Ejecución. Cantidad de proyectos y el costo total programado.

###### Proyectos terminados
Muestra información correspondiente a los proyectos que se encuentran en la etapa Terminado. Cantidad de proyectos y la sumatoria del costo real del proyecto una vez finalizado.

###### Proyectos por estado
En cifras presenta el costo estimado total de los proyectos, el valor total ejecutado a la fecha y el saldo pendiente por ejecutar (costo estimado – total pagado); para cada uno de los estados planeación, ejecución, y terminado.  

###### Aprobaciones pendientes
El cuadro con el título “pendientes”, muestra el número de documentos pendientes de aprobación para Pagos, Adiciones y Extensiones. Para cada uno muestra el número de documentos pendiente de aprobación para los Pagos y Adiciones el valor total de los documentos pendientes de aprobación.

###### Proyectos Finalizados por Año
Muestra para los 4 últimos años, incluido el año en curso, muestra el número de proyectos finalizados en cada año. Para hacer el gráfico se usa la fecha real de finalización de cada proyecto terminado.

###### Proyectos en Ejecución por avance físico
Muestra el número de proyectos que se encuentran en el porcentaje de avance físico correspondiente a cada rango. Los rangos presentados son: Sin avance, 0 a 25%, 25% a 50%, 50% a 75% y 75% a 100%. Para cada rango presenta los proyectos que tengan un porcentaje de avance mayor al porcentaje inicial y menor o igual que el porcentaje final.

###### Proyectos por Sector
Representa el porcentaje de proyectos correspondientes a cada sector. Si se coloca el cursor sobre un sector del gráfico mostrará el número de proyectos en ese sector y el porcentaje que representa del total.

###### Consultar un Proyecto
Esta sección situada al final del tablero permite seleccionar un proyecto. Una vez seleccionado un proyecto, mostrará al usuario el tablero que presenta el resumen del proyecto seleccionado. 
La lista presenta todos los proyectos. Se pueden ingresar algunos caracteres que sean parte del nombre o del código del proyecto para filtrar la lista y mostrará los proyectos que coincidan con el criterio de búsqueda ingresado.



## Tablero resumen de proyecto

Al seleccionar un proyecto, el repositorio presenta el tablero con el resumen del proyecto. En este tablero se presenta la información del proyecto, su ejecución y sus modificaciones.

![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Tablero_resumen_de_proyecto_2.jpeg)

El resumen presenta varias secciones: 
- Encabezado
- Información General
- Productos
- Pagos
- Adiciones
- Extensiones
- Ubicación


### Encabezado

El encabezado superior del Resumen muestra el nombre y el código del proyecto.


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Encabezado_2.png)
Este encabezado estará presente tanto en el resumen del proyecto como en cualquier otra pantalla donde se trabaje información específica del proyecto.

En el resumen, adicional al nombre del proyecto, presenta la etapa en que se encuentra el proyecto (ej. planeación, ejecución, terminado) y un resumen financiero.


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Encabezado_3.png)

El resumen financiero contiene la siguiente información:
- **Costo Inicial**: El costo inicial planeado cuando se definió el proyecto.
- **Costo Programado**: Calculado con base en el costo planeado de cada producto y las adiciones realizadas.
- **Total Pagado**: Valor total pagado a la fecha.
- **Por Ejecutar**: la diferencia entre el toral programado y el total pagado.
 


### Información General del Proyecto


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Informacion_General_del_Proyecto_2.png)

En el grupo de la izquierda presenta la información general con los siguientes datos:

- **Código**: Código que identifica al proyecto
- **Área responsable**: Organismo o área de la administración municipal responsable del proyecto.
- **Sector**: Principal sector al que aporta la ejecución del proyecto.
- **Subsector**: Subsector en el que se desarrolla el proyecto.
- **ODS**: Principal Objetivo de Desarrollo Sostenible al que apunta el proyecto.
- **Duración planeada**: Duración estimada en la etapa de planeación del proyecto. Se maneja en días.
- **Duración ajustada**: Duración programada a la fecha; se calcula teniendo en cuenta la duración planeada inicial y las extensiones definidas para el proyecto. Se maneja en días.
- **Fecha de Inicio planeada**: Fecha estimada de inicio del proyecto.
- **Fecha de Inicio Real**: Fecha real de inicio del proyecto.
- **Fecha de Finalización**: Fecha real de finalización del proyecto. Aplica únicamente para proyectos terminados.

El grupo de la derecha contiene dos diagramas que indican el porcentaje de avance físico y financiero del proyecto.

El botón "Fotos y Videos" permite acceder al material fotográfico y de videos del proyecto.

Al final de la información general se encuentra la descripción del proyecto.



### Productos

Esta sección presenta una lista de los productos del proyecto con la siguiente información:
- **Nombre del Producto**
- **Costo Estimado**: Se calcula usando el costo planeado inicial del producto ajustado con las adiciones registradas para el producto.



![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Productos_2.png)

En esta tabla, los usuarios del rol dirección, tendrán acceso a modificar la información de los productos existentes y a crear nuevos productos 

Los usuarios del rol operación y consulta, tendrán acceso a consultar la información de los productos existentes.



### Pagos

Esta sección presenta los pagos registrados para el proyecto.


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Pagos_2.png)

En la parte superior muestra valores totales que resumen cantidad de pagos y valor total de pagos en tres grupos:


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Pagos_3.png)

En la lista se presenta cada uno de los pagos registrados con la siguiente información:
- **Total**: todos los pagos registrados en el sistema para el proyecto, independiente de la etapa en que se encuentren.
- **Presentados**: los pagos registrados en el sistema que no han sido aprobados.
- **Aprobados**: pagos registrados en el sistema que han sido aprobados. Incluye aprobados y pagados.

Los usuarios del rol dirección, tendrán acceso a modificar la información de los pagos existentes y a crear nuevos pagos. Los usuarios del rol operación podrán registrar nuevos pagos en estado presentado; únicamente los usuarios del rol dirección podrán aprobarlos.

Los usuarios del rol operación y consulta, tendrán acceso a consultar la información de los pagos existentes.



### Adiciones


Esta sección presenta las adiciones registradas para el proyecto. Las adiciones registran las modificaciones en el valor planeado del costo del proyecto.


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Adiciones_2.png)

En la parte superior muestra valores totales que resumen cantidad de adiciones y valor total adicionado en tres grupos:



![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Adiciones_3.png)

En la lista se presenta cada una de las adiciones registrados con la siguiente información:
- **Total:** todas las adiciones registradas en el sistema para el proyecto, independiente de la etapa en que se encuentren.
- **Por aprobar:** las adiciones registradas en el sistema que no han sido aprobadas.
- **Aprobadas:** adiciones registradas en el sistema que han sido aprobados.

Los usuarios del rol dirección, tendrán acceso a modificar la información de las adiciones existentes y a crear nuevas adiciones. Los usuarios del rol operación podrán registrar nuevas adiciones en estado presentada; únicamente los usuarios del rol dirección podrán aprobarlas.

Los usuarios del rol operación y consulta, tendrán acceso a consultar la información de las adiciones existentes.



### Extensiones

Esta sección presenta las extensiones registradas para el proyecto. Las extensiones registran una variación en los tiempos de ejecución del proyecto.


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Extensiones_2.png)


En la parte superior muestra valores totales que resumen cantidad de extensiones y el número total de días de extensión. Estos valores se presentan en tres grupos:


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Extensiones_3.png)


En la lista se presenta cada una de las extensiones registradas con la siguiente información:
- **Total**: todas las extensiones registradas en el sistema para el proyecto, independiente de la etapa en que se encuentren.
- **Por aprobar**: las extensiones registradas en el sistema que no han sido aprobadas.
- **Aprobadas**: extensiones registradas en el sistema que han sido aprobados.

Los usuarios del rol dirección, tendrán acceso a modificar la información de las extensiones existentes y a crear nuevas extensiones. Los usuarios del rol operación podrán registrar nuevas extensiones en estado presentado; únicamente los usuarios del rol dirección podrán aprobarlas.

Los usuarios del rol operación y consulta, tendrán acceso a consultar la información de las extensiones existentes.



### Ubicación

Esta sección muestra la ubicación del proyecto en el mapa. La ubicación puede ser representada por una combinación de polígonos, líneas o marcadores.


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Ubicacion_2.png)



***

## Aprobaciones Pendientes

Para facilitar la labor de la dirección, el sistema cuenta con una sección especial que presenta todos los pendientes de aprobación agrupados por tipo de aprobación. Para cada pendiente de aprobación, permite ingresar y aprobar la solicitud pendiente (cambiar la etapa en que se encuentra).

Se presentan tres grupos:
- Pagos
- Adiciones
- Extensiones


#### Pagos pendientes de trámite

Presenta todos los pagos que se encuentren en etapa presentado o aprobado. Permite seleccionar el pago y modificar su estado a aprobado o pagado; las fechas correspondientes: fecha de aprobación y fecha pagado; y los anexos soporte de avance y soporte de pago.


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Pagos_pendientes_de_tramite_2.png)

Para cada pago pendiente de trámite presenta:
- **Producto**: el producto al que corresponde el pago.
- **Código**: Código que identifica el pago o solicitud de pago.
- **Mes de medición**: El mes y año a los que corresponde el porcentaje de avance físico reportado.
- **Monto**: el monto del pago.
- **Avance Físico**: El porcentaje de avance físico reportado para el periodo indicado.
- **Etapa**: La etapa en la que se encuentra el pago. Se presentan los pagos en etapa presentado o aprobado que son los que deben continuar un trámite hasta llegar a pagado.



#### Adiciones pendientes de Aprobación

Presenta las Adiciones que se encuentren pendientes de aprobación. Todas las adiciones que se encuentran en estado Presentada.

Permite seleccionar la adición y aprobarla registrando la fecha de aprobación; adicionalmente permite crear notas u observaciones y anexar un documento de aprobación.


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Adiciones_pendientes_de_Aprobacion_2.png)

El ícono de lápiz permite aprobar la solicitud. 

La lista presenta:
- **Producto**: el producto al que corresponde la Adición.
- **Fecha presentación**: la fecha en la que se presentó la adición.
- **Costo Adicional**: el costo adicional de la adición que será incrementado al costo programado total del proyecto.
- **Etapa**: Se presentan las adiciones en etapa "Presentada", las pendientes de aprobación.


#### Extensiones pendientes de aprobación


Presenta las Extensiones que se encuentren pendientes de aprobación. Todas las extensiones que se encuentran en estado Presentada.

Permite seleccionar la extensión y aprobarla registrando la fecha de aprobación; adicionalmente permite crear notas u observaciones y anexar un documento de aprobación.



![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Extensiones_pendientes_de_aprobacion_2.png)

El ícono de lápiz permite aprobar la solicitud. 

La lista presenta:

- **Producto**: el producto al que corresponde la Adición.
- **Fecha presentación**: la fecha en la que se presentó la adición.
- **Observación**: Nota u observación registrada con la extensión.
- **Días de extensión**: Número de días en que se incrementa la duración del proyecto.
- **Etapa**: Se presentan todas las extensiones de etapa "Presentada", las pendientes de aprobación.


****

## Reportes

El Repositorio ofrece reportes variados con diferentes tipos de información de los proyectos. Adicionalmente ofrece reportes para pagos, adiciones y extensiones.

Todos los reportes pueden ser consultados en pantalla o descargados en un archivo tipo Excel.



#### Reporte de Avance de Proyectos

Muestra el estado de avance financiero, físico y en tiempo de cada proyecto. Presenta información de lo planeado, lo ejecutado y el porcentaje de avance que esto representa. 



![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Reporte_de_Avance_de_Proyectos_2.png)


Para cada proyecto contiene:
- **Código del proyecto**
- **Nombre del proyecto***
- **Costo planeado**: Costo planeado inicialmente para el proyecto.
- **Costo programado**: La suma de los costos planeados de cada producto más el total de las adiciones. *
- **Valor pagado***
- **% de avance financiero***
- **% avance físico***
- **Inicio real**: Fecha real de inicio del proyecto
- **Fin real**: Fecha real de finalización
- **Fin Programado**: Fecha de terminación programada
- **Transcurrido**: Tiempo desde el inicio real del proyecto a la fecha actual o a la fecha de terminación, si ésta existe. *
- **% tiempo***

Para facilitar la lectura de los reportes, en pantallas pequeñas como los dispositivos móviles sólo se muestran las columnas marcadas con *.


#### Proyectos (Resumen)

Muestra la información general de los proyectos.



![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Proyectos_Resumen__2.png)


Para cada proyecto presenta:
- **Código** del proyecto. *
- **Nombre** del proyecto.
- **Área Responsable** u oficina responsable del proyecto. *
- **Sector**
- **Subsector**
- **Etapa***
- **Costo** estimado inicialmente para el proyecto. *
- **Descripción**
- **Objetivos**
- **Fuente de Financiamiento**: Entidades que financian el proyecto. *


Para facilitar la lectura de los reportes, en pantallas pequeñas como los dispositivos móviles sólo se muestran las columnas marcadas con *.



#### Proyectos (Básico)

Muestra la información básica de cada proyecto.



![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Proyectos_Basico__2.png)


Para cada proyecto presenta:

Contador de proyectos (columna sin título).
- **Código** del proyecto *
- **Nombre** del proyecto *
- **Área responsable**: Área u oficina responsable del proyecto.
- **Sector**
- **Subsector**
- **Etapa**: Etapa en la que se encuentra el proyecto (Planeación, ejecución, terminado). *

Para facilitar la lectura de los reportes, en pantallas pequeñas como los dispositivos móviles sólo se muestran las columnas marcadas con *.


#### Reporte de Pagos

Presenta los pagos registrados para los proyectos.


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Reporte_de_Pagos_2.png)

Para cada pago presenta:

- **Id proyecto**: Código del proyecto
- **Nombre del proyecto***
- **Id pago**: Código del pago
- **Etapa** en la que se encuentra el pago (Presentado, aprobado o pagado)
- **Periodo** al que corresponde el porcentaje de avance reportado*
- **Valor del pago** o solicitud de pago*
- **Porcentaje de avance físico***
- **Presentado**: Fecha en que se presentó la solicitud de pago*
- **Devengado**: Fecha en que se aprobó la solicitud de pago
- **Pagado**: Fecha de pago
- **‘Certificado?’**: Indica si se tiene anexo documento de certificado
- **‘Orden de pago?’**: Indica si se tiene anexo documento de orden de pago
- **Adjuntos**: Si tiene otros documentos adjuntos indica la cantidad.
- **Días trámite**: Días transcurridos entre la fecha presentado y la fecha actual (si no ha sido pagado) o fecha de pago (si ya fue pagado).*


Para facilitar la lectura de los reportes, en pantallas pequeñas como los dispositivos móviles sólo se muestran las columnas marcadas con *.




#### Reporte de Adiciones

Las adiciones registradas para cada proyecto.



![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Reporte_de_Adiciones_2.png)


Para cada adición presenta:
- **Código** del proyecto
- **Nombre** del proyecto*
- **Código la adición***
- **Etapa** en la que se encuentra la adición (presentada o aprobada)*
- **Valor** adicionado al costo del proyecto. *
- **Presentado**: Fecha en que se presenta la adición. *
- **Aprobado**: Fecha en que se aprueba la adición. *
- **Anexo**: indica si tiene archivo anexo.
- **Días trámite**: Días entre la fecha presentada y la fecha actual (si no ha sido aprobada) o fecha aprobación (si ya fue aprobada). *

Para facilitar la lectura de los reportes, en pantallas pequeñas como los dispositivos móviles sólo se muestran las columnas marcadas con *.




#### Reporte de Extensiones de Plazo

Las extensiones registradas para cada proyecto.


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Reporte_de_Extensiones_de_Plazo_2.png)

Para cada extensión presenta:
- **Id Proyecto**: Código del proyecto*
- Nombre del **proyecto**
- **id Extensión**: Código de la extensión
- **Etapa** en la que se encuentra la extensión (presentada o aprobada) *
- **Días** adicionales a la duración del proyecto. *
- **Presentado**: Fecha en que se presenta la extensión. *
- **Aprobado**: Fecha en que se aprueba la extensión.
- **Anexo extensión**: indica si tiene archivo anexo.
- **Notas**: o comentarios registrados
- **Tiempo trámite**: Días entre la fecha presentada y la fecha actual (si no ha sido aprobada) o fecha aprobación (si ya fue aprobada). *

Para facilitar la lectura de los reportes, en pantallas pequeñas como los dispositivos móviles sólo se muestran las columnas marcadas con *.








***

## Agregar o actualizar información


A través del Resumen de Proyecto o de las opciones del menú principal, los usuarios que se encuentren en los roles de administración, de dirección y operación tendrán acceso a pantallas de registro de información. El acceso a cada grupo de información depende del rol del usuario y en algunos casos de la etapa en que se encuentre la información a actualizar.

Usuarios del perfil operación pueden ingresar información como pagos, adiciones o extensiones y dejarlas en etapa “registrado” o pendiente de aprobación. Pueden crear proyectos nuevos y dejarlos en etapa “registro”. 

Usuarios del perfil dirección pueden aprobar la información ingresada por la operación o pasar los proyectos a una de las etapas activas (planeación, ejecución o terminado). 

La información de los proyectos en estados activos no puede ser modificada. Las modificaciones a proyectos en estados activos deben ser realizadas por un usuario de perfil Administrador del Sistema; para estos casos la entidad gubernamental debe establecer un protocolo que garantice la integridad de la información.

Usuarios de perfil consulta pueden consultar toda la información contenida en el sistema sin tener acceso a ningún tipo de modificación.


#### Proyecto



![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Proyecto_2.png)


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Proyecto_3.png)

- **Nombre**: Nombre del proyecto
- **Código**: Código que identifica al proyecto
- **Sector**: Principal sector al que aporta la ejecución del proyecto.
- **Subsector**: Subsector en el que se desarrolla el proyecto.
- **Área responsable proyecto**: Organismo o área de la administración municipal responsable del proyecto.
- **Entidad Ejecutora**: Entidad que ejecuta el proyecto.
- **Etapa**: Estado en que se encuentra el proyecto.
- **ODS**: Objetivo de Desarrollo Sostenible al que contribuye el proyecto con mayor relevancia.
- **Costo estimado**: Costo estimado del proyecto
- **Duración estimada**: Duración en días estimada del proyecto.
- **Fecha de Inicio planeada**: Fecha estimada de inicio del proyecto.
- **Fecha de Inicio Real**: Fecha real de inicio del proyecto.
- **Fecha de Finalización**: Fecha real de finalización del proyecto.
- **Descripción**: Descripción del proyecto
- **Objetivos**: Objetivos del Proyecto
- **Ubicación**: Ubicación georreferenciada del proyecto.
- **Fuentes de financiamiento**: Fuentes de financiamiento del proyecto.
- **Anexos**: Archivos anexos al proyecto.



#### Fuente de Financiamiento

Cada una de las fuentes de financiamiento del proyecto. Un proyecto puede tener una o varias fuentes de financiamiento. Se registra cada fuente para cada tipo de financiamiento. Una misma entidad puede aparecer en dos fuentes de financiamiento, por ejemplo, el BID puede aparecer en una fuente de financiamiento con un crédito y en otra fuente de financiamiento con una donación.


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Fuente_de_Financiamiento_2.png)


- **Proyecto**: Proyecto al que corresponde la fuente de financiamiento.
- **Tipo de Financiamiento**: El tipo de financiamiento correspondiente al monto indicado para la entidad. (Préstamo, recursos propios, donación, APP)
- **Fuente**: Organismo o entidad que financia el monto indicado.
- **Valor**: Monto o valor a ser financiado por la fuente.



#### Anexo Proyecto

El sistema permite almacenar archivos anexos que amplíen la información del proyecto. Se pueden anexar documentos como, por ejemplo: Documento de Diseño del Proyecto, Documento de viabilidad financiera del proyecto, acta de inicio, convenio con la entidad financiadora.
 
![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Anexo_Proyecto_2.png)

- **Proyecto**: Proyecto al que pertenece el archivo anexo.
- **Título**: Título que indica el contenido del anexo
- **Archivo**: Nombre del archivo que contiene el anexo.
- **Fecha de carga**: Fecha en que se carga el documento en el sistema.



#### Producto

Cada proyecto tiene uno o varios productos, los pagos y registros de avance físico se registran por producto. Si la entidad gubernamental no administra los proyectos por producto, puede crear un producto genérico para cada proyecto.


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Producto_2.png)


- **Proyecto**: Proyecto al que pertenece el producto.
- **Nombre del Producto**: El nombre que identifica al producto del proyecto.
- **Costo Estimado**: Costo estimado inicial del producto
- **Descripción**: Descripción del producto
- **Objetivo Específico**: Objetivo Específico al que aporta el producto.






#### Pago

El avance físico y financiero del proyecto se registra a través de los pagos. Para cada pago se indica el valor pagado (avance financiero) y el porcentaje de avance físico que soporta el pago (avance físico).

![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Pago_2.jpeg)

- **Código**: Código de Certificado asignado automáticamente por el sistema. Se usa para facilitar la identificación y consulta del certificado.
- **Producto**: Producto al que corresponde el pago realizado y el avance registrado
- **Fuente de Financiamiento**: La fuente de financiamiento de la que se obtienen los recursos para el pago.
- **Monto**: Valor o monto del pago.
- **Avance físico (%)**: El avance físico reportado en el certificado.
- **Etapa**: La etapa en la que se encuentra el Certificado de Pago. (Presentado, aprobado, pagado)
- **Soporte de avance**: Archivo anexo que contiene el documento que soporta el avance reportado.
- **Soporte de pago**: Archivo anexo que contiene el documento que soporta el pago realizado.



#### Adición

Las adiciones registran las modificaciones o ajustes al costo programado del proyecto.

![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Adicion_2.png)

- **Código**: Código de la Adición. Asignado automáticamente por el sistema. Se usa para facilitar la búsqueda o identificación de la Adición al costo del producto.
- **Producto**: Producto al que se le hace una adición en el costo planeado.
- **Costo Adicional**: Valor que se adiciona al costo del producto.
- **Etapa**: Etapa en la que se encuentra la Adición (presentada o aprobada)
- **Notas**: Comentarios o anotaciones que explican o justifican la Adición al costo del producto.
- **Soporte de aprobación**: Archivo adjunto que contiene el documento donde se aprueba la Adición al costo planeado del producto.
- **Otros anexos**: Archivos anexos que soportan la Adición




#### Extensión

Las extensiones registran las modificaciones a la duración del proyecto. El registro se hace en días.

![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Extension_2.png)

- **Código**: Código de la Extensión asignado automáticamente por el sistema. Se usa para que los usuarios puedan buscar o identificar la Extensión de tiempo de ejecución del proyecto.
- **Proyecto**: Proyecto al que aplica la extensión.
- **Días de extensión**: Tiempo en días en el que se extiende el plazo del proyecto
- **Observación**: Motivo por el cual se hace la Extensión de tiempo o de plazo del proyecto.
- **Etapa**: La Etapa en que se encuentra la solicitud de extensión de tiempo (presentada, aprobada)
- **Acto de aprobación**: Archivo adjunto que contiene el documento en el que se soporta y aprueba la Extensión de plazo del proyecto.
- **Anexos**: Permite administrar los anexos para la Extensión.


#### Fotos y Videos

Registro de imágenes y videos del proyecto. Presenta las imágenes y videos registradas para el proyecto y permite descargar las imágenes y reproducir los videos. A los usuarios del perfil adecuado les permite registrar nuevas imágenes o videos.


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Fotos_y_Videos_2.png)



#### Imagen

El sistema permite cargar imágenes que muestren el estado del proyecto.


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Imagen_2.png)

Para cada imagen cargada se registra la siguiente información:

- **Proyecto**: Proyecto al que corresponde la imagen.
- **Archivo**: Nombre del archivo que contiene la imagen.
- **Descripción**: Descripción de la imagen.
- **Fecha de cargue**: Fecha en que se registra la imagen en el sistema.



#### Video

El sistema permite crear vínculos externos a videos publicados relativos al proyecto.
 
![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Video_2.png)

- **Proyecto**: Proyecto al que corresponde el video
- **Enlace**: Enlace que permite acceder al video. El video puede publicarse en una plataforma como YouTube e incluir aquí el enlace correspondiente.
- **Descripción**: Descripción del video
- **Fecha de cargue**: Fecha en que se registra el video en el sistema.



***

# Configuración del Sistema


El sistema puede adaptarse a las particularidades de cada gobierno a través del módulo de configuración. El perfil de administrador del sistema puede configurar información de uso general.

- Usuarios
- Áreas Responsables
- Entidades Ejecutoras
- Fuentes de Financiamiento
- Sectores
- Subsectores


Para la configuración inicial del sistema es necesario completar cada las tablas de configuración para que las diferentes opciones se encuentren disponibles al momento de ingresar los proyectos.



#### Usuarios

Para que un usuario nuevo tenga acceso al sistema debe ser autorizado por la administración del sistema. 

Una vez la administración del sistema valida el usuario, completa la información del usuario y le asigna el nivel de acceso que corresponda.

La administración del sistema puede reiniciar la clave de un usuario que la haya olvidado.



![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Usuarios_2.png)

El módulo de administración de usuarios presenta la lista de usuarios registrados. 

 Actualizar usuario autorizado
![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Usuarios_3.png)
 Usuario pendiente de autorización
![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Usuarios_4.png)
 Restablecer contraseña
![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Usuarios_5.png)

Todos los usuarios registrados y autorizados tienen acceso a la información de proyectos registrada en el sistema. La diferencia está en la información que pueden modificar.

El rol del usuario determina su nivel de acceso al sistema:

- **Dirección**: Crear, actualizar y aprobar proyectos, pagos, extensiones y adiciones que se encuentren pendientes de aprobación. Aprobar pagos, extensiones y adiciones.
- **Operación**: Crear y actualizar proyectos, pagos, extensiones y adiciones que se encuentren pendientes de aprobación. 
- **Consulta**: No puede modificar información en el Repositorio.
- **Administrador del Sistema**: Puede modificar toda la información. Es el único rol con acceso al módulo de configuración.

Al seleccionar la opción de actualizar un usuario la pantalla permite registrar o actualizar la información del usuario.



![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Usuarios_6.png)


- **Correo electrónico**: Correo Electrónico del usuario. Campo informativo, no puede ser modificado.
- **Rol**: El rol que se le asigna al usuario según su nivel de acceso. (Dirección, Operación o Consulta).
- **Nombre**: Nombre del usuario
- **Apellido**: Apellido del usuario
- **Oficina**: Oficina o área de la administración de la entidad gubernamental a la que pertenece el usuario
- **Notas**: Anotaciones o comentarios

Para autorizar un usuario, este debe haberse registrado previamente en el sistema. 



#### Oficinas o Áreas Responsables

Cada una de las oficinas o áreas de la entidad gubernamental que puede ser responsable de un proyecto.


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Oficinas_o_Areas_Responsables_2.png)

Permite seleccionar una oficina existente para actualizar su información o crear una nueva. 

![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Oficinas_o_Areas_Responsables_3.png)

**Nombre**: Nombre de la oficina o área de la administración de la entidad gubernamental.



#### Entidad Ejecutora

Cada una de las entidades que ejecuta algún proyecto. Para poder asignar la entidad a un proyecto debe haber sido ingresada al sistema por la administración del sistema. Antes de entrar una nueva entidad, hay que revisar que no esté registrada con otro nombre o con otra forma de escribir el nombre.



![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Entidad_Ejecutora_2.png)

- **Nombre**: Nombre o razón social de la entidad.
- **Sigla**: La sigla o abreviatura que permite identificar la entidad ejecutora con un texto breve. Para ser utilizada en reportes.

- **/# ID**: Número de identificación oficial o fiscal de la entidad ejecutora.



#### Entidad Financiadora

Cada una de las entidades que son fuente de financiación de los proyectos. La lista debe incluir la misma entidad gubernamental para el caso de los proyectos financiados con recursos propios. Antes de entrar una nueva entidad, hay que revisar que no esté registrada con otro nombre o con otra forma de escribir el nombre.


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Entidad_Financiadora_2.png)


- **Nombre**: Nombre que identifica a la agencia de financiamiento o la entidad que financia.
- **Sigla**: La sigla o abreviatura que permite identificar la entidad financiadora con un texto breve. Para ser utilizada en reportes.



#### Sector

Los diferentes sectores en los que se agrupan los proyectos.


![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Sector_2.png)

**Nombre**: Nombre o título que identifica al sector




#### Subsector

Define los subsectores de cada sector.

![Image](https://github.com/EL-BID/RepositorioMapaInversiones/blob/main/Manuals/UserManual/images/Subsector_2.png)

- **Sector**: Sector al que pertenece el subsector.
- **Nombre**: Nombre o título que identifica al subsector.







