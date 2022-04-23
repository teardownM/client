# TeardownM (client)
Server-authoritative multiplayer in Teardown! This mod is the client needed to join servers and for you to play online with others.
We are using the open-source game server [Nakama](https://heroiclabs.com/docs/nakama/getting-started/docker-quickstart/) as a base for our client-server communication.

To run your own server, please see [here](#).

## Build requirements:
* [Sledge](https://github.com/44lr/sledge/tree/refactor-and-update)
* [Visual Studio](https://visualstudio.microsoft.com/)
* C# SDK (.NET 6)

## How to build
1. Create an environment variable called `SLEDGE_ROOT_DIR`, it should be set to your sledge dir where mods, sledge.exe, etc.. is`
2. Navigate to your sledge/mods folder
3. Clone this repository inside
4. Create a folder in `mods` called `TeardownM`
5. Copy `client/TeardownM/dep/*.dll` into `mods/TeardownM/dependencies`
6. Run `build.bat`
7. Run sledge.exe

## Contribution:
There should be an active todo list of issues and tasks.
I'll be in the [Sledge discord](https://discord.gg/mgMtkGNt) most of the time @alexandargyurov#1220

## Notice!
This is in veryyy early stages :) 