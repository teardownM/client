# Teardown Nakama (client)
Server-authoritative multiplayer in Teardown! This mod is the client needed to join servers and for you to play online with others.
We are using the open-source game server [Nakama](https://heroiclabs.com/docs/nakama/getting-started/docker-quickstart/) as a base for our client-server communication.

To run your own server, please see [here](#).

## Build requirements:
* [Sledge](https://github.com/44lr/sledge)
* [Visual Studio](https://visualstudio.microsoft.com/)
* [Nakama .NET Client](https://github.com/heroiclabs/nakama-dotnet)
* Newtonsoft.Json (Install from package manager)
* C# SDK (.NET 6)

## How to build
1. Clone the repository
2. Open the solution
3. On the Solution Explorer, right click Dependencies, and click "Add Project Reference", a window will open
4. On that window, click "Browse", locate sledgelib.dll (located in the mods folder of Sledge), add it
4. On the same window, locate Nakama.dll (in the current directory), add it and click okay
5. Build the project
6. Create a new directory called "TeardownNakama" in the mods directory of your Sledge installation.
7. Put TeardownNakama.dll into that folder.
8. Create a "dependancies" folder inside the "TeardownNakama" folder.
9. Place Nakama.dll and Newtonsoft.Json dll inside there
10. Ensure you have a local instance of Docker running with the Nakama server. See [here](#) for more detail. 

## Contribution:
There should be an active todo list of issues and tasks.
I'll be in the [Sledge discord](https://discord.gg/mgMtkGNt) most of the time @alexandargyurov#1220

## Notice!
This is in veryyy early stages :) 