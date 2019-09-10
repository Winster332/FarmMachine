# ![Icon](https://github.com/Winster332/FarmMachine/blob/master/Resources/icon.png?raw=true) FarmMachine 


This is a service consisting of two microservices that allows you to configure offline trading. For this, services are integrated with external exchanges, and TradeView. TradeView is necessary for conducting backtests and receiving signals. Since there is currently no suitable tool for executing PineScript in the local system, I had to put TradeView into the Chromium engine, and already through it execute js scripts on the TradeView side. The FarmMachine.MonitorStrategy microservice is responsible for this. Microservices are connected by the RabbitMQ bus, and the MongoDB database is used for storage. When a new signal arrives, FarmMachine.MonitorStrategy sends the RabbitMQ bus signal. This signal is caught by the FarmMachine.ExchangeBroker service. He is responsible for integration with exchanges. When a buy / sell signal arrives, some calculations occur, after which a conclusion is made - what to buy, at what price and in what volume.

> The data project is experimental, and is not recommended for reliable use.

Reports on the work of Rabbit can be seen in [this table](https://docs.google.com/spreadsheets/d/1I9c0Wa98iYQpuwfH7Ijr6h4on4wzOlCQhKeiOg3TFtA/edit?usp=sharing). 
<br>
The project works exclusively on Windows. Since the project uses the chromium engine, this greatly complicates the transfer of the project to other OS.

## Supported exchanges

| Name         | Version            | Support |
| ------------- |:------------------:| -------:|
|  [BITTREX](https://international.bittrex.com/)     |   1.0.0  | Yes   |
|  [BINANCE](https://www.binance.com/ru)         |   1.0.0   | Yes   |
|  [POLONIEX](https://poloniex.com/)         |   future   | No   |
|  [HitBTC](https://hitbtc.com/)         |   future   | No   |
|  [Bitstamp](https://www.bitstamp.net/)         |   future   | No   |
|  [Bitfinex](https://www.bitfinex.com/)         |   future   | No   |

Use the api key and secret to authorize your api exchange

### Services

- [FarmMachine.ExchangeBroker](docs/FarmMachine.ExchangeBroker.md)
- [FarmMachine.MonitorStrategy](docs/FarmMachine.MonitorStrategy.md)

### Scheme

orange - development in progress
<br>
red - part due to which docker is not used
<br>
blue - direct functions not available

![N|Solid](https://github.com/Winster332/FarmMachine/blob/master/Resources/scheme.png)

Google Drive - [Draw.IO scheme](https://drive.google.com/file/d/1V9ktUXEDGq-747UuSGEsqQjPWD_fuzGH/view?usp=sharing)

## Build and start

Before you start building a project, you need to install a [Docker](https://www.docker.com). After installation and configuration. And after the launch of the infrastructure, you can start assembling the project services

### Start infrastructure

You must go to the [configuration/docker/](https://github.com/Winster332/FarmMachine/tree/master/configuration/docker) directive, and then run the following commands:

```powershell
PS> FarmMachine\configuration\docker> docker-compose -f docker-compose.infrastructure.local.yml build
```

Wait for the completion of the containers.
<br>
After that, you can begin to deploy the infrastructure.

```powershell
PS> FarmMachine\configuration\docker> docker-compose -f docker-compose.infrastructure.local.yml up
```

> Also worth noting, docker is not a required link. You can deploy everything on a real host. The minimum list consists of [MongoDB](https://github.com/mongodb/mongo-csharp-driver) and [RabbitMQ](https://www.rabbitmq.com/).

If no errors appear, then the infrastructure is deployed correctly and you can proceed to the next step - to services.

### Start services

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
  },
  "telegram": {
    "enabled": false,
    "apiKey": your_telegram_api_key,
    "apiHash": "your_telegram_api_hash",
    "phoneNumber": "your_phone_number"
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
[Binance.Net](https://github.com/JKorf/Binance.Net) = for integration with Binance API
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
<br>
[TLSharp](https://github.com/sochix/TLSharp) - telegram integrations

LICENCE
-------
[GNU General Public License v3.0](https://github.com/Winster332/FarmMachine/blob/master/LICENSE)
