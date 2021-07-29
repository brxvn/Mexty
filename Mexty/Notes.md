# Notas de desarrollo

## TODO
- [x] Arreglar y buscar los bugs de la ultima observación.
- [x] Llenar el programa de entradas al log.
    - [x] Poner entradas en el modulo de usuarios.
    - [x] Poner entradas en el modulo de Productos.
    - [x] Poner entradas en el modulo de Clientes.
    - [x] Poner entradas en el modulo de Sucursal.
- [x]Añadir CI [link](https://www.youtube.com/watch?v=VIlDni8-iWM).
  - [x] Ver que onda con el archivo de configuración del logger en el deploy.
- [x] Terminar el modulo de sucursales.
- [x] Ajustar base de datos con las nuevas especificaciones.
- [x] !!!! Añadir try y catch en todas las funciones de alta, update y activar.
- [ ] Ver como hacer el cron job de la pestaña de BD para hacer el volcado de bd automáticamente cada x tiempo.
- [ ] Añadir manejo de excepciones global [link](https://wpf-tutorial.com/wpf-application/handling-exceptions/).
- [ ] Armar los códigos de error para el manejo de errores en el programa.

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


## Notas
- Que busque las imágenes en c:\Mexty\Logos\
  - Debe de haber un Chico.png, Mediano.png, Grande.png.
- Campo sincroniza tipo boolean.
  - Campo de fecha_sincroniza en string.
  
### Notas sig reunión
- Preguntar si debemos de poner avisos de error cuando algo salga mal y que deben de decir.
  - Ejemplo cuando una query sale mal por alguna razón mandar una advertencia o algo.
  - Si si, agregar un trow en los catch de database.
  