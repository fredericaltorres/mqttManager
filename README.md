# MQTT Manager

## Overview

The MQTTManagerLib.MQTTManager is a helper class to publish or listen to MQTT channel.

[Source on Github](https://github.com/fredericaltorres/mqttManager)

## MQTT Library

MqttDotNet by stevenlovegrove

[Mqtt lib](https://github.com/stevenlovegrove/MqttDotNet)

## MQTT Provider

The company cloudmqtt.com can be used to create an free MQTT server
Non secure connection string
```
	connectionString = "tcp://m15.cloudmqtt.com:10989";
```
[Cloudmqtt Documentation](https://www.cloudmqtt.com/docs-dotnet.html)

## Cloudmqtt Authorization

Cloudmqtt allow to create multiple `User` and `ACLs`.
A REST API allow to manager the users and ACL.

* http basic auth: username:empty, password:api-key
* https://docs.cloudmqtt.com/cloudmqtt_api.html?shell#

* Sample
	- Get list of user
		https://api.cloudmqtt.com/api/user
	- Get the ACL assigned to a user 
		https://api.cloudmqtt.com/api/user/user1

