## Running the iOS app

After cloning the repository, run the following:

```sh
cd Projects/Bakhtawar.Apps.NativeApp
yarn
cd ios && pod install && cd ..
npx react-native run-ios
```

## Running the Android app

After cloning the repository, run the following:

```sh
cd Projects/Bakhtawar.Apps.NativeApp
yarn
npx react-native run-android
```

Note that you have to have the emulator open before running the last command. If you have difficulty getting the emulator to connect, open the project from Android Studio and run it through there.
