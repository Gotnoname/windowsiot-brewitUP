# windowsiot-brewitUP

This is a WIP of a Windows 10 IoT brewing controller using, for example, a Raspberry pi with a touch screen.

# Features
- Define custom brewing profiles (mash, ingredients etc..)
- Touch friendly UI that is currently tailored to 800x480.
- UI for connecting to WiFi networks
- Fully customized Raspberry pi GPIO usage. GPIO-pins are used to control heater, buzzer, pump, drop slots etc...
- Sous Vide capabilities
- Delayed brewing mode
- Supports DS18B20 and DS18S20 temperature controllers thanks to Rinsen's OneWire library

#TODO
- Created a Web API so that the controller can be accessed and controlled by any device on the same network.
- Clean up initial release work

#Screenshots

![Screenshot](https://github.com/Gotnoname/windowsiot-brewitUP/blob/master/NewBrewPi/ImageExamples/1.png)
![Screenshot](https://github.com/Gotnoname/windowsiot-brewitUP/blob/master/NewBrewPi/ImageExamples/2.png)
![Screenshot](https://github.com/Gotnoname/windowsiot-brewitUP/blob/master/NewBrewPi/ImageExamples/3.png)

# External
- https://github.com/Rinsen/OneWire
- https://github.com/ms-iot/pid-controller/tree/master/PidController/PidController
- Icons from: http://flaticons.net/
