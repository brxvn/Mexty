# Notas de desarrollo

## TODO
- [ ] Añadir manejo de excepciones global [link](https://wpf-tutorial.com/wpf-application/handling-exceptions/).
- [ ] Armar los códigos de error para el manejo de errores en el programa.
- [ ] !!!! Pensar en como manejar si eliminamos un producto del cat de productos.

## Notas
- Que busque las imágenes en c:\Mexty\Logos\
  - Debe de haber un Chico.png, Mediano.png, Grande.png.
  
### Notas sig reunión


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

Donde ID es el Id de la tienda con la cual fue dada de alta y corresponde a la ubicación actual del programa.

### Error 11
Es un error causado cuando el programa no puede leer las credenciales de acceso a la base de datos del archivo de configuración Settings.ini ubicado en C:\Mexty\Settings.ini.
Estas son importantes ya que sin ellas no podremos acceder a los datos guardados.
El archivo de configuración las debe de tener declaradas de la siguiente manera.
> DbUser=usuario
> DbPass=contraseña

Donde usuario corresponde a el usuario dado de alta para conectarse a la base de datos y la contraseña su respectiva contraseña.
