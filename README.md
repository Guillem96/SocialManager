# SocialManager
Desktop application that allows you to organize socialsocial networks and chat with your friends.

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

### Requests Types and ACKs

* *RegisterReq* : Petición de registro.
* *LoginReq* : Petición de loguearse.
* *AliveInf* : Petición de mantenerse activo.
* *LogoutReq* : Petición de logout.
* *DeleteAccountReq* : Petición de eliminar la cuenta.
* *NewContactReq* : Enviar una nueva petición de amistad.
* *AcceptNewContact* : Acceptar una petición de amistad.
* *RegNewContact* : Descartar una petición de amistad.
* *ListContactReq* : Petición de la lista de tus contactos.
* *ProfileUpdateReq* : Petición de actualizar el perfil.
* *ClientsQueryReq* : Pedir lista de usuarios registrados apartir de una query.
* *SendMessageReq* : Petición de enviar mensaje.
* *ReadyChatReq* : Petición de loguearse en el chat.

### Prerequisites

Nuget packages.

```

```

## Built With

* [Tweetinvi](https://github.com/linvi/tweetinvi) - Twitter api calls.

## Authors

* **Guillem Orellana Trullols** - [Guillem96](https://github.com/Guillem96)
