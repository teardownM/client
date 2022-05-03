# TeardownM (client)
Server-authoritative multiplayer in Teardown! This mod is the client needed to join servers and for you to play online with others.
We are using the open-source game server [Nakama](https://heroiclabs.com/docs/nakama/getting-started/docker-quickstart/) as a base for our client-server communication.

To run your own server, please see [here](#).

## Build requirements:
* [Sledge](https://github.com/44lr/sledge/tree/refactor-and-update)
* [Visual Studio](https://visualstudio.microsoft.com/)
* C# SDK (.NET 6)

## How to build
1. Build Sledge git clone -b refactor-and-update --recurse-submodules https://github.com/44lr/sledge/
2. Create an environment variable called `SLEDGE_ROOT_DIR`, it should be set to your sledge dir where mods, sledge.exe, etc.. is`
3. Clone this repository 
4. Run `Build.ps1` located inside `client\` (Do `.\Build.ps1 -Help` for all available options)

## Contribution:
There should be an active todo list of issues and tasks.
I'll be in the [Sledge discord](https://discord.gg/mgMtkGNt) most of the time @alexandargyurov#1220

## Notice!
This is in veryyy early stages :) 