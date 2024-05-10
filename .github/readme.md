This repo is a managed implementation of the native [iCUE SDK](https://github.com/CorsairOfficial/cue-sdk) which is made by [Corsair](https://github.com/CorsairOfficial/cue-sdk/releases).

![Alt](https://repobeats.axiom.co/api/embed/02619cff50e5af0e4a71a1967fa303876e0ac0b8.svg "Repobeats analytics image")

Features
- Device Lighting
    - Lighting layering system
    - `keyboards:` Zone based lighting
    - `Keyboards:` Various lighting effects
    - `Keyboards:` Displaying bitmap images on the keyboard
- Accessing Device specific information

Not everything is implemented right now. Upon request or my own interest I might implement more of the SDK.

The SDK is reliant upon the `iCUE.exe` process, the SDK is reliant upon `iCUESDK.x64_2019.dll` and `iCUESDK_2019.dll`. Both dlls are included on build.

### Samples
- [HeadsetBatteryLevel](../src/Samples/HeadsetBatteryLevel/Program.cs) - A program that will print out your Wireless Headset's battery %.
- [KeyboardColors](h../src/Samples/KeyboardColors/Program.cs) - A demonstration of Keyboard Led Control.
- [KeyboardColors](../src/Samples/KeyboardColorsAsync/Program.cs) - A demonstration of Async Keyboard Led Control.
- [ShowDeviceInformation](../src/Samples/ShowDeviceInformation/Program.cs) - A collection of information from all available devices.

Thanks:
Big thanks to [@ophura](https://github.com/ophura/)!