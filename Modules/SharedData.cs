using Il2Cpp;
using UnityEngine;

namespace labyrinthine_library.Modules;

public class SharedData
{
    public static bool IsInGame { get; set; }
    public static bool IsInMainMenu { get; set; }
    public static bool IsInLobby {  get; set; }
    public static bool IsInitialized { get; set; }
    public static bool HasInitialized { get; set; }
    public static bool IsInLoadingScreen { get; set; }
    public static ToastNotification? Toast { get; set; }
    public static GameObject? Player { get; set; }
    public static List<PlayerNetworkSync>? Players { get; set; }
    public static CharacterController? CharacterController { get; set; }
    public static PlayerController? PlayerController { get; set; }
}
