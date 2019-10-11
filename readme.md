# Test Jaya

El proyecto está construido usando **.net Core 3.0** y **Entity FrameworkCore 3.0** . 
Para el uso de sockets existe un middleware que crea un Socket por cada ejecución y que lo identifica con el Id del usuario y de la sala a la que se ingresó, de esta forma, cuando un usuario  envía un mensaje, este solo es reenviado a los sockets que están conectados a la misma sala.


# Frontend

El FE es muy básico, en el chat solo hay una página básica y un Js básico también que envia y recibe mensajes y los muestra.

# Backend

El BE hace uso de multiples patrones de diseño, como Reporitory, se usa SimpleInjector para la inyección de dependencias y clases abstractas para la creación de los repositorios y servicios, de tal forma que no es necesario implementar cada uno de ellos, smplemente se necesita extender la clase abstracta y  registrar el servicio/repositorio en el contenedor para la inyección de su dependencia.


# Ejecución

En el archivo DbContext del proyecto TestJaya.Data  en la linea 27, cambiar el connectionString por uno valido.
Crear la base de datos con EF ejecutando el comando:  **dotnet ef database update**
Ejecutar el proyecto Test.Jaya