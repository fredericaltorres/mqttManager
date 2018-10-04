
# MQTT Manager

## Overview

The MQTTManagerLib.MQTTManager is a helper class to publish or listen to
mqtt channel.


## MQTT Provider

The company cloudmqtt.com can be use to create an MQTT server

Non secure connection string
```
	connectionString = "tcp://m15.cloudmqtt.com:10989";
```
[Cloudmqtt Documentation](https://www.cloudmqtt.com/docs-dotnet.html)


## Cloudmqtt Authorization

* http basic auth: username:empty, password:api-key
* https://docs.cloudmqtt.com/cloudmqtt_api.html?shell#

get list of user
	https://api.cloudmqtt.com/api/user
https://api.cloudmqtt.com/api/user/user1