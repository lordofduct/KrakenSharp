# KrakenSharp
C# library for accessing both the websocket and rest api's of Kraken.

This implementation was originally based on user [m4cx](https://github.com/m4cx) implementation [kraken-wsapi-dotnet](https://github.com/m4cx/kraken-wsapi-dotnet).

Upon my initial use of that api there were some issues connecting with Kraken as the API had slightly changed at that point. It also was missing some of the websocket methods like 'cancelAll' and 'cancelAllOrdersAfter'. While digging through the code and making some changes to add support for things I wanted I ended up drastically modifying it that it couldn't be submitted to the original repository for approval (I did not try, I know the changes were drastic enough to make doing so difficult). So instead I'm submitting my own repository.

This explanation is here to give credit to m4cx, where credit is do. I've made said changes under the permissiveness of the MIT license and this repository is also available under the same MIT license. I have added notes in the code files that heavily borrow from m4cx.

Changes include a rehaul of how the socket connection is managed, added new methods from the Kraken API, and started implementing simple async callbacks for the Kraken REST API.

# API Documentation

See the official documentation...

WebSocket Documentation:
https://www.kraken.com/features/websocket-api

REST API Documentation:
https://www.kraken.com/en-us/features/api

