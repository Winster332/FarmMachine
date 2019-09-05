# ![Icon](https://github.com/Winster332/FarmMachine/blob/master/Resources/icon.png?raw=true) FarmMachine 

This is a service consisting of two microservices that allows you to configure offline trading. For this, services are integrated with external exchanges, and TradeView. TradeView is necessary for conducting backtests and receiving signals. Since there is currently no suitable tool for executing PineScript in the local system, I had to put TradeView into the Chromium engine, and already through it execute js scripts on the TradeView side. The FarmMachine.MonitorStrategy microservice is responsible for this. Microservices are connected by the RabbitMQ bus, and the MongoDB database is used for storage. When a new signal arrives, FarmMachine.MonitorStrategy sends the RabbitMQ bus signal. This signal is caught by the FarmMachine.ExchangeBroker service. He is responsible for integration with exchanges. When a buy / sell signal arrives, some calculations occur, after which a conclusion is made - what to buy, at what price and in what volume.

> The data project is experimental, and is not recommended for reliable use.

## Supported exchanges

| Name         | Version            | Support |
| ------------- |:------------------:| -------:|
|  [BITTREX](https://international.bittrex.com/)     |   1.0.0  | Yes   |
|  [BINANCE](https://www.binance.com/ru)         |   future   | No   |
|  [POLONIEX](https://poloniex.com/)         |   future   | No   |
|  [HitBTC](https://hitbtc.com/)         |   future   | No   |

## Build and start

```powershell
PS> .\farmmachine.ps1 build
Microsoft (R) Build Engine version 16.2.0-preview-19278-01+d635043bd for .NET Core
Copyright (C) Microsoft Corporation. All rights reserved.

  Restore completed in 512.73 ms for C:\Users\Winster332\Desktop\FarmMachine\FarmMachine.ExchangeBroker\FarmMachine.ExchangeBroker.csproj.
...
...

PS> .\farmmachine.ps1 start
Begin start FarmMachine.ExchangeBroker
Begin start FarmMachine.MonitorStrategy
```

## Links
[TradeView](https://ru.tradingview.com)
<br>
[Chromium](https://github.com/cefsharp/CefSharp)
<br>
[Bittrex.Net](https://github.com/JKorf/Bittrex.Net)
<br>
[CQ](https://github.com/jamietre/CsQuery)

LICENCE
-------
[GNU General Public License v3.0](https://github.com/Winster332/FarmMachine/blob/master/LICENSE)
