# ![Icon](https://github.com/Winster332/FarmMachine/blob/master/Resources/icon.png?raw=true) FarmMachine 


This is a service consisting of two microservices that allows you to configure offline trading. For this, services are integrated with external exchanges, and TradeView. TradeView is necessary for conducting backtests and receiving signals. Since there is currently no suitable tool for executing PineScript in the local system, I had to put TradeView into the Chromium engine, and already through it execute js scripts on the TradeView side. The FarmMachine.MonitorStrategy microservice is responsible for this. Microservices are connected by the RabbitMQ bus, and the MongoDB database is used for storage. When a new signal arrives, FarmMachine.MonitorStrategy sends the RabbitMQ bus signal. This signal is caught by the FarmMachine.ExchangeBroker service. He is responsible for integration with exchanges. When a buy / sell signal arrives, some calculations occur, after which a conclusion is made - what to buy, at what price and in what volume.

> The data project is experimental, and is not recommended for reliable use.

The project works exclusively on Windows. Since the project uses the chromium engine, this greatly complicates the transfer of the project to other OS.

## Supported exchanges

| Name         | Version            | Support |
| ------------- |:------------------:| -------:|
|  [BITTREX](https://international.bittrex.com/)     |   1.0.0  | Yes   |
|  [BINANCE](https://www.binance.com/ru)         |   future   | No   |
|  [POLONIEX](https://poloniex.com/)         |   future   | No   |
|  [HitBTC](https://hitbtc.com/)         |   future   | No   |

## Build and start

Before that, you need to configure RabbitMq and MongoDB. Then change the settings appsettings.json for yourself, and after that you can execute these commands

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
After that, the integration service with exchanges should start. If everything went well, a terminal with your settings will appear. And after it, a second service will be launched to monitor the strategy

### Configurations

The configuration file is located in **FarmMachine/FarmMachine.ExchangeBroker/appsettings.json**

```javascript
{
  "database": { // MongoDB config section
    "dataBaseName": "FarmMachine",
    "connectionString": "mongodb://localhost:27017/"
  },
  "rabbitMq": { // RabbitMQ config section
    "host": "rabbitmq://127.0.0.1/yuor_virtual_host", 
    "concurrencyLimit": 64
  },
  "bittrex": {
    "key": "your_bittrex_key", // Need set your key from bittrex api
    "secret": "your_bittrex_secret", // Need set your secret from bittrex api
    "market": "USD-BTC", // Maker name
    "riskManager": {
      "baseCurrency": "USD", // Your base currency
      "type": "percent", // types risk manager: percent and fixed
      "amount": 0, // for type fixed. Need if you selected type fixed. Fixed amount
      "percent": 100, // // for type percent
      "balanceMinLimit": 15 // minimal balance on you wallet
    }
  }
}
```

## Integration through RabbitMQ

If you want to use only part of this project. You can easily integrate through the RabbitMQ bus. To do this, you need to connect to the queue
> farm_machine

and after that send messages to buy or sell:

> FarmMachine.Domain.Commands.Exchange:SellCurrency
```C#
public interface BuyCurrency
{
    Guid Id { get; set; }
    DateTime Created { get; set; }
    decimal Amount { get; set; }
    decimal Bid { get; set; }
}
```
> FarmMachine.Domain.Commands.Exchange:BuyCurrency
```C#
public interface SellCurrency
{
  Guid Id { get; set; }
  DateTime Created { get; set; }
  decimal Amount { get; set; }
  decimal Ask { get; set; }
}
```
## Links
[MassTransit](https://github.com/MassTransit/MassTransit) - for RabbitMQ
<br>
[TradeView](https://ru.tradingview.com) - read strategy
<br>
[Chromium](https://github.com/cefsharp/CefSharp) - load and integration with TradeView
<br>
[Bittrex.Net](https://github.com/JKorf/Bittrex.Net) - for integration with BITTREX API
<br>
[Topshelf](https://github.com/Topshelf/Topshelf) - run app as service
<br>
[CQ](https://github.com/jamietre/CsQuery) - html parser
<br>
[Serilog](https://github.com/serilog/serilog) - logger
<br>
[Autofac](https://github.com/autofac/Autofac) - DI
<br>
[MongoDB](https://github.com/mongodb/mongo-csharp-driver) - as load and save documents

LICENCE
-------
[GNU General Public License v3.0](https://github.com/Winster332/FarmMachine/blob/master/LICENSE)
