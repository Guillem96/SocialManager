# SocialManager

<img src="Logo.png" alt="SocialManager Logo" style="width: 200px;"/>

Desktop application that allows you to organize socialsocial networks and chat with your friends.

Video de mustra de las funcionalidades de la aplicación [aquí](https://youtu.be/4VNhM-VQUGs).

## Getting Started

Para correr correctamente la aplicación es necessario perimero arrancar el servidor y cuando este este listo entonces abrir el cliente.

## Details

* Aplicación modelo cliente-servidor.
* Para la parte de administración de cuenta se utiliza protocólo UDP.
* Para la parte de chat se utilitza TCP/IP.
* El servidor es concurrente, crea un proceso para atender cada petición de los
clientes.
* El servidor almazena en una base de datos todos los datos de los clientes.
 Si estos quieren sus datos deben realizar una petición.
* Los clientes podrán manejar sus contactos(agregar, eliminar ...), los datos de su perfil(modificarlos) y sus redes sociales(Leer las últimas publicaciones, añadir nuevas publicaciones...).

### Estructura cliente - servidor

* La aplicación esta formada por un cliente con UI y un servidor en modo consola.
* El servidor almazena los datos de todos los clientes registrados en una base de datos SQLServer.
* El servidor se encarga de atender las peticiones de los clientes de forma concurrente.
* El cliente hace las llamadas a las apis respectivas de las redes sociales.

### Requests Types and ACKs

* *RegisterReq* : Petición de registro (UDP).
* *LoginReq* : Petición de loguearse(UDP).
* *AliveInf* : Petición de mantenerse activo(UDP).
* *LogoutReq* : Petición de logout(UDP).
* *DeleteAccountReq* : Petición de eliminar la cuenta(UDP).
* *NewContactReq* : Enviar una nueva petición de amistad(UDP).
* *AcceptNewContact* : Acceptar una petición de amistad(UDP).
* *RegNewContact* : Descartar una petición de amistad(UDP).
* *ListContactReq* : Petición de la lista de tus contactos(UDP).
* *ProfileUpdateReq* : Petición de actualizar el perfil(UDP).
* *ClientsQueryReq* : Pedir lista de usuarios registrados apartir de una query(UDP).
* *SendMessageReq* : Petición de enviar mensaje(TCP).
* *ReadyChatReq* : Petición de loguearse en el chat(TCP).
* *LinkSocialNetworkReq* : Petición de vincular una red social y almacenar los datos en el servidor (UDP).
* *DeleteLinkSocialNetReq* : Petición para eliminar el vínculo de la res social.

## Server Prerequisites

Nuget packages.

```
None
```

## Client Prerequisites

Nuget packages.

```
Autofac                             {3.5.2}                                  SocialManager_Client                                                                                             
InstagramLib.InstagramGot           {1.0.0}                                  SocialManager_Client                                                                                             
Microsoft.Bcl                       {1.1.10}                                 SocialManager_Client                                                                                             
Microsoft.Bcl.Async                 {1.0.168}                                SocialManager_Client                                                                                             
Microsoft.Bcl.Build                 {1.0.21}                                 SocialManager_Client                                                                                             
Microsoft.Net.Http                  {2.2.29}                                 SocialManager_Client                                                                                             
Newtonsoft.Json                     {10.0.3}                                 SocialManager_Client                                                                                             
PhantomJS                           {1.9.8}                                  SocialManager_Client                                                                                             
Selenium.WebDriver                  {3.4.0}                                  SocialManager_Client                                                                                                             
TweetinviAPI                        {1.2.0}                                  SocialManager_Client                                                                                             
WpfAnimatedGif                      {1.4.14}                                 SocialManager_Client        
```

InstagramLib.InstagramGot es un nuget package desarrollado por mí. Tambien puede ser descargado de mi github [InstagramGot](http://www.github.com/Guillem96/InstagramGot).

## Built With

* [Tweetinvi](https://github.com/linvi/tweetinvi) - Twitter api calls.
* [InstagramGot](http://www.github.com/Guillem96/InstagramGot) - Instagram api calls.

## Build

Como compilar el proyecto.

### Building the client
1. Abrir el proyecto ServerManager_Client con Visual Studio.
2. Añadir los siguientes archivos a la carpeta de compilación.
	* Carpeta Images
	* server.cfg
3. Compilar. Si compilamos en debug la carpeta de compilación será:
 `SocialManager\SocialManager_Client\SocialManager_Client\bin\Debug`
4. Ejecutar SocialManager_Client.exe.

### Building the server
1. Abrir el proyecto ServerManager_Server con Visual Studio.
2. Compilamos.
4. Ejecutar SocialManager_Server.exe.

### Start the app directly
1. Ir a la carpeta FinalApplication.
2. Ejecutar el servidor.
3. Ejecutar el cliente.

## Authors

* **Guillem Orellana Trullols** - [Guillem96](https://github.com/Guillem96)
