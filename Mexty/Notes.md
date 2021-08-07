# Notas de desarrollo

## TODO
- [ ] Añadir manejo de excepciones global [link](https://wpf-tutorial.com/wpf-application/handling-exceptions/).
- [ ] Armar los códigos de error para el manejo de errores en el programa.
- [ ] !!!! Pensar en como manejar si eliminamos un producto del cat de productos.

## Notas
- Que busque las imágenes en c:\Mexty\Logos\
  - Debe de haber un Chico.png, Mediano.png, Grande.png.
  
### Notas sig reunión
- Preguntar por el campo de DEBE en Venta-Mayoreo.
- Preguntar porque en venta menudeo Fecha-registro esta como llave primaria (y porque?) y en menudeo no.
- Preguntar por usuario-modifica y fecha modifica en venta mayoreo.

## Entradas al log.
- Error: para manejar excepciones, dando una descripción de lo que sucede y donde.
- Info: Cuando cargamos un modulo, escribimos a la base de datos.
- Debug: Cuando sucede algo en el programa y nos puede ayudar a debugear un problema.
- Warn: Errores en validaciones.

## Códigos de error
> Deben tener, código, descripción, razones por las que puede suceder y como arreglarlas.

### Error 10
Es un error causado cuando el programa no puede leer el campo de IdTienda del archivo de configuración Settings.ini ubicado en C:\Mexty\Settings.ini.
El archivo debe de tener el campo declarado de la siguiente manera.
> IdTienda=ID

Donde ID es el Id numérico de la tienda con la cual fue dada de alta y corresponde a la ubicación actual del programa.

### Error 11
Es un error causado cuando el programa no puede leer las credenciales de acceso a la base de datos del archivo de configuración Settings.ini ubicado en C:\Mexty\Settings.ini.
Estas son importantes ya que sin ellas no podremos acceder a los datos guardados.
El archivo de configuración las debe de tener declaradas de la siguiente manera.
> DbUser=usuario
> DbPass=contraseña

Donde usuario corresponde a el usuario dado de alta para conectarse a la base de datos y la contraseña su respectiva contraseña.

### Error 12
Es un error en el que el programa ha fallado al validar la primera consulta para iniciar sesión en el programa.

Recomendamos contactar a los administradores del programa o hacer una restauración de la base de datos ya que esta podría estar comprometida.

### Error 13
Es un error causado cuando el programa no puede validar que el Id de sucursal dado en el archivo de configuración exista.

Revise el archivo de configuración Settings.ini ubicado en C:\Mexty\Settings.ini en el campo llamado `IdTienda=` y verifique que es un id valido de una tienda 
previamente dada de alta en la base de datos.

### Error 14
Es un error en el que el programa falla al intentar obtener los datos sobre algún modulo de la base de datos.

Recomendamos contactar a los administradores del programa o hacer una restauración de la base de datos ya que esta podría estar comprometida.

### Error 15
Es un error en el que el programa falla al intentar guardar o actualizar los datos sobre algún modulo de la base de datos.

Recomendamos contactar a los administradores del programa o hacer una restauración de la base de datos ya que esta podría estar comprometida.

### Error 16
Es un error que ocurre cuando falla el proceso de exportar los cambios a un script SQL. Puede ser causado porque la base de datos ha sido comprometida o porque la carpeta 
Donde se van a guardar los cambios `C:\Mexty\Backups\` es de solo escritura.

Recomendamos contactar a los administradores del programa o hacer una restauración de la base de datos ya que esta podría estar comprometida.


### Error 16.5
Ocurre cuando el proceso de vaciar la tabla de los cambios y hacer un reset de su id falla.

Recomendamos contactar a los administradores del programa o hacer una restauración de la base de datos ya que esta podría estar comprometida.

### Error 17
Este error se presenta cuando el programa falla al exportar toda la estructura y datos de la base de datos.

Recomendamos contactar a los administradores del programa o hacer una restauración de la base de datos ya que esta podría estar comprometida.


### Error 18
Es un error que ocurre cuando al importar los cambios de la base de datos uno de estos falla o cuando falla el import de toda la base de datos.

Los cambios son un script SQL el cual es ejecutado secuencialmente, si uno de estos falla puede significar que este script fue manipulado a mano.
No es recomendable manipular a mano el script SQL ya que al no saber la sintaxis de SQL podríamos tener errores y provocar este error.

TODO: poner ejemplo de queries SQL.
