# AblerexUPS
Ablerex RS-RT Series configuration program

![Program Screenshot](https://raw.githubusercontent.com/nProtect/AblerexUPS/master/screenshot.png)

This program connect to Ablerex RS-RT Series UPS to monitor UPS status and change UPS Paramter via RS232 interface.

# Requirement
- RS232 to USB cable

Since offical Ablerex program (Emily2 Monitor Software) didn't provide output voltage adjustment.
so I created own and decide to share to public.

# This program may work with other Ablerex UPS model, Please use at your own risk!

Tested on Ablerex RS-RT 2000 and working.
- Change output voltage
- Change UPS mode from Normal to ECO (and ECO back to Normal)

# Step to use
1. Connect RS232 header to back panel and connect USB to your PC
2. Start program, choose COM port and click "Connect"
3. UPS Status such as Input/Output voltage will show up and also UPS information

# UPS Adjustment
1. Click on "Change UPS Settings"
2. Wait program to read current settings for a while
3. You can change UPS settings and click "Write"
4. You will hear 2 short beep on UPS device, turn it off and turn on again to take effect.

**CF50, CF60 means convert frequency to 50/60 Hz

**Normal Mode = Full True-Online double conversation

**ECO Mode = While AC avaiable, UPS will change to Bypass mode, if AC loss UPS switch to normal mode until AC power back. (aka. Line Interactive Mode)

This app created on Visual Studio 2017 and using .NET Framework 4.5

Pull requrest are welcome!
