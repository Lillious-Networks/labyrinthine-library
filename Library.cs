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
    public static GameObject[]? Players() { return SharedData.Players; }
    public static CharacterController? CharacterController() { return SharedData.CharacterController; }
    public static PlayerController? PlayerController() { return SharedData.PlayerController; }
    public static Monsters Monsters => Monsters;
    public static Levels Levels => Levels;
    public static Currency Currency => Currency;
    public static Saves  Saves => Saves;
    public static Experience Experience => Experience;
    public static Achievements Achievements => Achievements;

}