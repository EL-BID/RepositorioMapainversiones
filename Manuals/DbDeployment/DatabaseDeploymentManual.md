# Repositorio MapaInversiones
## Despliegue de la base de datos

---

Los siguientes son los pasos para lograr tener una base de datos funcional del Repositorio MapaInversiones. 

***

#### Descomprimir el Backup
Descomprimir el backup que se encuentra en formato zip.

![Image](images/Descomprimir_el_Backup_2.png)

***

#### Crear la base de datos
Crear una base de datos llamada InvestmentMapRepositoryC4D.

Si está usando Microsoft SQL Server Management Studio:
- En el lado izquierdo de la pantalla, clic derecho sobre Databases
- New Database…
- En Database name: InvestmentMapRepositoryC4D

![Image](images/Crear_la_base_de_datos_2.png)

***

#### Restaurar la base de datos
Restaurar la base de datos desde la copia de respaldo InvestmentMapRepositoryC4D.bak

Si está usando Microsoft SQL Server Management Studio:
- En el lado izquierdo de la pantalla, clic derecho sobre el nombre de la nueva base de datos (InvestmentMapRepositoryC4D)
- Dentro de la opción Tasks usar las opciones Restore y Database…

![Image](images/Restaurar_la_base_de_datos_2.png)

- Seleccionar el archivo de respaldo descomprimido.
-- Marcar la opción Device y usar el botón  para buscar el archivo de respaldo descomprimido y Ok.
![Image](images/Restaurar_la_base_de_datos_3.png)
-Verificar que esté seleccionada la base de datos InvestmentMapRepositoryC4D

![Image](images/Restaurar_la_base_de_datos_4.png)

***

#### Registrar la cadena de conexión en el código fuente del Sistema
Actualizar el código fuente para que utilice la base de datos que se instaló.
- En el archivo appsettings.json del código fuente registrar, la cadena de conexión

![Image](images/Registrar_la_cadena_de_conexion_en_el_Codigo_fuente_del_Sistema_2.png)

- En el archivo Program.cs del código fuente, poner el nombre de la nueva cadena de conexión.

![Image](images/Registrar_la_cadena_de_conexion_en_el_Codigo_fuente_del_Sistema_3.png)

***

#### Usuario pre-registrado de Administración del Sistema
La base de datos restaurada tiene un usuario del sistema de nivel administrador.
Lo primero que se debe hacer al instalar el sistema y la base de datos es ingresar al sistema con el usuario administrador pre-registrado y modificar la clave de acceso.

![Image](images/Usuario_pre_registrado_de_Administracion_del_Sistema_2.png)

Información del usuario pre-registrado:

- Email: admin@mail.com
- Contraseña: 

Una vez modificada la contraseña se registra la información de las diferentes tablas de configuración para poder empezar a trabajar en los proyectos. Los nuevos usuarios deben registrarse en el sistema y luego ser autorizados por un administrador del sistema que les asigna su rol correspondiente.

