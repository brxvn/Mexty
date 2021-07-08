# Notas de desarrollo

## TODO
- [ ] Arreglar y buscar los bugs de la ultima observación.
- [x] Llenar el programa de entradas al log.
    - [x] Poner entradas en el modulo de usuarios.
    - [x] Poner entradas en el modulo de Productos.
    - [x] Poner entradas en el modulo de Clientes.
    - [x] Poner entradas en el modulo de Sucursal.
- [ ]Añadir manejo de excepciones global [link](https://wpf-tutorial.com/wpf-application/handling-exceptions/).
- [x]Añadir CI [link](https://www.youtube.com/watch?v=VIlDni8-iWM).
  - [ ] Ver que onda con el archivo de configuración del logger en el deploy.
- [ ] Terminar el modulo de sucursales.
- [ ] Ver como hacer el cron job de la pestaña de BD para hacer el volcado de bd automáticamente cada x tiempo.


## Entradas al log.
- Error: para manejar excepciones, dando una descripción de lo que sucede y donde.
- Info: Cuando cargamos un modulo, escribimos a la base de datos.
- Debug: Cuando sucede algo en el programa y nos puede ayudar a debugear un problema.
- Warn: Errores en validaciones.