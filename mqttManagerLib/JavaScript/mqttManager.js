/*
    mqttManager.js a wrapper/helper around MQTT.js for browser.
    - Developed to work with MQTT broker from CloudMqtt.com.
    - Tested in 
        - Chrome Windows and Chrome iOS.
        - Edge and IE 11 Windows.

    Dependencies:
    - mqtt.js
        https://github.com/mqttjs/MQTT.js
        c:\>npm install mqtt --save

    Frederic Torres 2018.10
 */
var mqtt = require('mqtt');

const WEB_SOCKET_DEFAULT_PORT = 30989;

class MqttManager {

    _client = null;
    _options = null;

    constructor(mqttUrl, username, password, webSocketPort = WEB_SOCKET_DEFAULT_PORT, clientId) {

        if (!clientId)
            clientId = 'mqttManager.js_' + Math.random().toString(16).substr(2, 8);

        this._options = {
            port: webSocketPort, // Required for WebSocket
            useSSL: true,        // Required for WebSocket
            host: mqttUrl,
            clientId,
            username,
            password,
            keepalive: 60,
            reconnectPeriod: 1000,
            protocolId: 'MQIsdp',
            protocolVersion: 3,
            clean: true,
            encoding: 'utf8'
        };
    }
    trace(m) {
        console.log(`[MqttMgr]${m}`);
    }
    parseMessage(rawMessage, topic) {
        const o = JSON.parse(rawMessage);
        const oo = { ...o, topic };
        console.log('PARSE MESSAGE', JSON.stringify(oo));
        return oo;
    }
    onReceivedMessage = (topic, message) => {
        const parsedMessage = this.parseMessage(message, topic);
        if (parsedMessage.clientId === this._options.clientId) {
            return; // ignore message sent by this instance
        }
        this.trace(`RECEIVED:${parsedMessage.message}, ${parsedMessage.clientId}, ${parsedMessage.topic}`);
        if (this.messageArrived) {
            this.messageArrived(parsedMessage);
        }
    }
    subscribe(channel) {
        return new Promise((resolve, reject) => {
            this.trace(`Subscribe ${channel}`);
            this._client.subscribe(channel, (err) => {
                if (err) {
                    console.log(`Subscribe err:${err}`);
                    reject(channel);
                }
                else {
                    this.trace(`Subscribed ${channel}`);
                    this._client.on('message', this.onReceivedMessage);
                    resolve(channel);
                }
            });
        });
    }
    buildMessage(message) {
        const s = JSON.stringify({ message, clientId: this._options.clientId });
        console.log('BUILD MESSAGE', s);
        return s;
    }
    publish(channel, message) {
        return new Promise((resolve, reject) => {
            this.trace(`Publishing message:${message}`);
            // https://github.com/mqttjs/MQTT.js#publish
            this._client.publish(channel, this.buildMessage(message), undefined, (err) => {
                if (err) {
                    this.trace(`Publishing failed message:${message}`);
                    reject(message);
                }
                else {
                    resolve(message);
                }
            });
        });
    }
    start() {
        return new Promise((resolve, reject) => {

            const optionsOkToTrace = Object.assign({ password: 'ok' }, this._options);
            optionsOkToTrace.password = null;
            this.trace(`Connecting options:${JSON.stringify(optionsOkToTrace)}`);

            this._client = mqtt.connect(this._options.host, this._options);

            this._client.on('error', (err) => {
                console.log(err);
                reject(false);
            });
            this._client.on('connect', () => {
                this.trace(`MQTT Connected to ${this._options.host}`);
                resolve(true);
            });
        });
    }
    stop() {

    }
}

export default MqttManager;
