using UnityEngine;
using Il2Cpp;
using MelonLoader;

namespace labyrinthine_library.Modules;

public class Initializer
{
    public SharedData SharedData = new();
    private readonly static ToastNotification Toast = new();
    private static bool LoadedMessage = false;

    // Controls the initialization of the shared data
    public static async Task OnSceneWasInitialized(int buildIndex, string sceneName)
    {

        MelonLogger.Msg($"Initialized {sceneName} at index: {buildIndex}");

        // Initialize the shared data when the "Init" scene is loaded
        if (sceneName == "Init") {
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
        }

        // TESTING START

        if (SharedData.IsInMainMenu)
        {
            Levels.Set(1337);
        }

        // TESTING END


        // Player is in game if the build index is greater than 5 (assuming build index 0-5 are non-game scenes)
        if (buildIndex > 6 && !SharedData.IsInLobby && !SharedData.IsInMainMenu) { SharedData.IsInGame = true; }

        if (SharedData.IsInMainMenu && !LoadedMessage)
        {
            SharedData.Toast?.Show($"Loaded Labyrinthine Library", ToastType.Success);
            LoadedMessage = true;
        }

        if (SharedData.IsInGame && !SharedData.IsInLoadingScreen)
        {
            SharedData.Toast?.Show($"Entered game scene: {sceneName}", ToastType.Info);
        }

        // Set up a loop to check for the player object and its components when in game
        if (SharedData.IsInGame)
        {
            // Wait at least 1 second to allow the player object to be created
            await Task.Delay(1000);

            // We only need to check if the player object is referenced
            while (SharedData.Player == null)
            {
                // Find the player object by its tag
                GameObject? Player = GameObject.FindGameObjectWithTag("LocalPlayer")?.gameObject;
                PlayerControl[]? playerComponents = UnityEngine.Object.FindObjectsOfType<PlayerControl>();
                GameObject[]? Players = new GameObject[playerComponents.Length];

                for (int i = 0; i < playerComponents.Length; i++)
                {
                    Players[i] = playerComponents[i].gameObject;
                }

                SharedData.Players = Players;

                if (Player != null)
                {
                    // Set references to the player and its components in the shared data
                    SharedData.Player = Player;
                    SharedData.CharacterController = Player.GetComponent<CharacterController>();
                    SharedData.PlayerController = Player.GetComponent<PlayerController>();

                    // If we successfully found the player and its components, we can mark initialization as complete
                    if (SharedData.Player != null)
                    {
                        SharedData.HasInitialized = true;
                        break;
                    }
                }
                 // Wait a short time before checking again to avoid excessive CPU usage
                 await Task.Delay(500);
            }
        }
    }

}
