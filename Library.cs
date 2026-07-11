using UnityEngine;
using MelonLoader;
using Il2Cpp;
using labyrinthine_library.Modules;
namespace labyrinthine_library;

public class Library : MelonMod
{
    public override void OnInitializeMelon()
    {
        HarmonyInstance.PatchAll();
        Logs.Info("Ran harmony patches");
    }

    public override async void OnSceneWasInitialized(int buildIndex, string sceneName)
        => await Initializer.OnSceneWasInitialized(buildIndex, sceneName);

    public static bool IsInGame() { return SharedData.IsInGame; }
    public static bool IsInLoadingScreen() { return SharedData.IsInLoadingScreen; }
    public static bool IsInMainMenu() { return SharedData.IsInMainMenu; }
    public static bool IsInLobby() { return SharedData.IsInLobby; }
    public static bool IsInitialized() { return SharedData.IsInitialized; }
    public static bool HasInitialized() { return SharedData.HasInitialized; }
    public static void ShowToast(string message, ToastType type = ToastType.Info) { SharedData.Toast?.Show(message, type); }
    public static GameObject? Player() { return SharedData.Player; }
    public static List<PlayerNetworkSync>? Players() { return SharedData.Players; }
    public static CharacterController? CharacterController() { return SharedData.CharacterController; }
    public static PlayerController? PlayerController() { return SharedData.PlayerController; }
    public static Monsters Monsters => Monsters;
    public static Levels Levels => Levels;
    public static Currency Currency => Currency;
    public static Contracts Contracts => Contracts;
    public static Saves  Saves => Saves;
    public static Experience Experience => Experience;
    public static Achievements Achievements => Achievements;
    public static Events Events => Modules.Events.Instance;
    public static RemoteControl RemoteControl => RemoteControl;
    public static Logs Logs => Logs;
    public static Items Items => Items;
    public static Cosmetics Cosmetics => Cosmetics;
}