P3bble
======

P3bble is a framework for connecting to Pebble SmartWatches from Windows Phone 8.

## Donate
[![Donate with Bitcoin](https://en.cryptobadges.io/badge/micro/1Ck4XgAxys3aBjdesKQQ62zx7m4vozUest)](https://en.cryptobadges.io/donate/1Ck4XgAxys3aBjdesKQQ62zx7m4vozUest)

Donate DogeCoin: DPVz6RSAJrXZqTF4sGXpS1dqwvU36hSaAQ



Getting Started:
================

* Install [P3bble](https://www.nuget.org/packages/P3bble) from Nuget or clone this repo and build from source.
* Add `ID_CAP_PROXIMITY` to WMAppManifest.xml to allow connections from your app.
* Detect and connect to the first Pebble found: 

To use the library with Windows Phone 8.1 and WinRT (WinRT not finished currently), you need to add this 

```
    <m2:DeviceCapability Name="bluetooth.rfcomm">
      <m2:Device Id="any">
        <m2:Function Type="serviceId:00000000-deca-fade-deca-deafdecacaff"/>
      </m2:Device>
    </m2:DeviceCapability>
```
in your Package.appxmanifest beetween the "Capabilities" Tag.

```
var pebbles = await P3bble.Pebble.DetectPebbles();
if (pebbles != null && pebbles.Count > 0)
{
    bool connected = await pebbles[0].ConnectAsync();
    if (connected)
    {
        MessageBox.Show("connected to " + pebbles[0].DisplayName);
    }
    else
    {
        MessageBox.Show("could not connect");
    }
}
```
Version 2.2 features:
=====================
* Added support for Windows Phone Silverlight 8.1
* Added support for Windows Phone 8.1



Version 2.0 features:
=====================

* Get and Set Time
* List installed apps
* Remove app by index
* Install app from PBW
* Launch app
* Install firmware
* Music control works from foreground app
* Refactored methods to be async/await instead of using events
* Patched FindPebbles to only include items starting "Pebble"
* Added interface for P3bble to allow mocking
* Changed from WebClient to [HttpClient](https://www.nuget.org/packages/Microsoft.Net.Http) to make code more portable
* Changed from SharpGISto [sharpcompress](https://www.nuget.org/packages/sharpcompress/) to make code more portable

Version 2.0 features were added by [Steve Robbins](https://twitter.com/sr_gb) - but they were only possible because of the work that Patrik did originally as well as the [Python reverse engineering work](https://github.com/Hexxeh/libpebble).

*TODO: WinRT not working yet - unable to connect to Bluetooth paired devices - any help getting this working appreciated!* 

Version 1.0
===========
Version 1.0 created by [Patrik Pfaffenbauer](https://twitter.com/p3root) - see <https://github.com/p3root/P3bble>


License
=======

P3bble is released under the 3-clause license ("New BSD License" or "Modified BSD License") - see https://raw.githubusercontent.com/p3root/P3bble/master/LICENSE.txt

