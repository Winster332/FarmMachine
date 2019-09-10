## FarmMachine.MonitorStrategy

This service launches a remote TradeView in the chromium engine. After that it executes some JavaScript, instructions, is parsed, 
and the necessary data is extracted. The presence of a signal is checked. 
<br>
If there is no signal, we are waiting for it to appear. If there is a signal - send it to the RabbitMQ bus.
