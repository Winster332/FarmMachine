## FarmMachine.ExchangeBroker

For this service, you need to configure the appsettings.json file. Then you can start. Since this project works on a .net crust, it can be put in a docker container.
<br>
This service is responsible for the integration of the local system with BITTREX.API. He receives messages on RabbitMQ, from FarmMachine.MonitorStrategy, and makes decisions on further processing of the request.
