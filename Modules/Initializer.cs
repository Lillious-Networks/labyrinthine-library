using UnityEngine;
using Il2Cpp;
using MelonLoader;
using System.Collections;

namespace labyrinthine_library.Modules;

public class Initializer
{
    public SharedData SharedData = new();
    private readonly static ToastNotification Toast = new();
    private static bool Loaded = false;
    private static bool CoroutineStarted = false;
    private static readonly Dictionary<string, bool> PlayerDeathState = new();

    public static IEnumerator MonitorPlayers()
    {
        while (SharedData.IsInGame)
        {
            if (SharedData.Players != null)
            {
                for (int i = 0; i < SharedData.Players.Count; i++)
                {
                    var player = SharedData.Players[i];
                    if (player == null)
                        continue;

                    string key = player.name;
                    bool wasDead = PlayerDeathState.TryGetValue(key, out bool prev) && prev;

                    if (player.isDead && !wasDead)
                    {
                        Events.Instance.InvokePlayerDeath(player);
                    }

                    PlayerDeathState[key] = player.isDead;
                }
            }

            yield return new WaitForSeconds(1f);
        }

        CoroutineStarted = false;
        PlayerDeathState.Clear();
    }

    // Controls the initialization of the shared data
    public static async Task OnSceneWasInitialized(int buildIndex, string sceneName)
    {

        // Initialize the shared data when the "Init" scene is loaded
        if (sceneName == "Init")
        {
            SharedData.IsInitialized = true;
            SharedData.Toast = Toast;
        };

        if (!SharedData.IsInitialized) return;

        // Set the main menu flag accordingly
        SharedData.IsInMainMenu = sceneName == "MainMenu";

        // Set the lobby flag accordingly
        SharedData.IsInLobby = sceneName == "Lobby" || sceneName == "Lobby_PC";

        // Set the loading screen flag accordingly
        SharedData.IsInLoadingScreen = sceneName == "LoadingScreen";

        // If in the main menu, reset the game-related shared data
        if (SharedData.IsInMainMenu)
        {
            SharedData.IsInGame = false;
            SharedData.IsInLobby = false;
            SharedData.Player = null;
            SharedData.CharacterController = null;
            SharedData.PlayerController = null;
            SharedData.HasInitialized = false;
            SharedData.IsInLoadingScreen = false;
            CoroutineStarted = false;
        }


        // Player is in game if the build index is greater than 5 (assuming build index 0-5 are non-game scenes)
        if (buildIndex > 6 && !SharedData.IsInLobby && !SharedData.IsInMainMenu) { SharedData.IsInGame = true; }

        if (SharedData.IsInMainMenu && !Loaded)
        {
            Logs.Info("Loaded Labyrinthine Library");
            Loaded = true;
        }

        if (SharedData.IsInGame && !SharedData.IsInLoadingScreen)
        {
            Logs.Info($"Entered game scene: {sceneName}");
        }

        if (SharedData.IsInGame && !CoroutineStarted)
        {
            CoroutineStarted = true;
            await Task.Delay(1000);

            while (SharedData.Player == null)
            {
                GameObject player = Il2Cpp.GameManager.instance.mainPlayer;

                if (player != null)
                {
                    SharedData.Player = player;
                    SharedData.CharacterController = player.GetComponent<CharacterController>();
                    SharedData.PlayerController = player.GetComponent<PlayerController>();
                    SharedData.HasInitialized = true;
                }
                else
                {
                    Logs.Warning("Unable to locate main player in game. Trying again...");
                    await Task.Delay(500);
                }
            }

            while (SharedData.Players == null && SharedData.IsInGame)
            {
                var players = Il2Cpp.GameManager.instance.Players;

                if (players != null)
                {
                    SharedData.Players = new List<PlayerNetworkSync>(players.Count);

                    for (int i = 0; i < players.Count; i++)
                    {
                        SharedData.Players.Add(players[i]);
                    }

                    MelonCoroutines.Start(MonitorPlayers());
                }
                else
                {
                    Logs.Warning("Unable to fetch players in game. Trying again...");
                    await Task.Delay(500);
                }
            }
        }
    }
}
