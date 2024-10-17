using MelonLoader;
using UnityEngine;
using static MelonLoader.MelonLogger;
using Il2Cpp;
using labyrinthine_library.Shared;

namespace labyrinthine_library;

public class Library : MelonMod
{
    // Create a new instance of SharedData
    public SharedData SharedData = new();
    public bool GetIsInGame() { return SharedData.IsInGame; }
    public bool GetIsInMainMenu() { return SharedData.IsInMainMenu; }
    public bool SetIsNoClip(bool value) { return SharedData.IsNoClip = value; }
    public bool GetIsNoClip() { return SharedData.IsNoClip; }
    public bool GetIsInitialized() { return SharedData.IsInitialized; }
    public bool GetHasInitialized() { return SharedData.HasInitialized; }
    public float SetNoclipShiftSpeed(float value) { return SharedData.NoclipShiftSpeed = value; }
    public float GetNoclipShiftSpeed() { return SharedData.NoclipShiftSpeed; }
    public GameObject? GetPlayer() { return SharedData.Player; }
    public CharacterController? GetCharacterController() { return SharedData.CharacterController; }
    public PlayerController? GetPlayerController() { return SharedData.PlayerController; }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName) { Msg($"[SCENE LOADED] Name: {sceneName} Index: {buildIndex}"); }

    public override void OnSceneWasUnloaded(int buildIndex, string sceneName) { Msg($"[SCENE UNLOADED] Name: {sceneName} Index: {buildIndex}"); }

    public override async void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        Msg($"[SCENE INITIALIZED] Name: {sceneName} Index: {buildIndex}");

        // Set the initialized flag once the game starts
        if (sceneName == "Init") { SharedData.IsInitialized = true; }

        // Set the main menu flag accordingly
        SharedData.IsInMainMenu = sceneName == "MainMenu";

        // Maps are typically after the 5th index - does not work
        // Trigger the in game flag and initialize other items
        // TODO: Fix
        if (buildIndex > 5)
        {
            SharedData.IsInGame = true;
            await Task.Delay(1000); // Required otherwise player will be null
            GameObject? Player = GameObject.FindGameObjectWithTag("LocalPlayer").gameObject;

            if (Player == null)
            {
                Msg("Unable to locate Player object!");
                // Uninitialize if we can't find the player object to prevent errors
                SharedData.HasInitialized = false;
            }
            else
            {
                PlayerController? PlayerController = Player.GetComponent<PlayerController>();
                CharacterController? CharacterController = Player.GetComponent<CharacterController>();

                // Assign SharedData values
                SharedData.Player = Player;
                SharedData.PlayerController = PlayerController;
                SharedData.CharacterController = CharacterController;

                // Initialize
                SharedData.HasInitialized = true;
            }
        }
        else
        {
            SharedData.IsInGame = false;
        }
    }

    public override void OnUpdate()
    {
        // Game has not initialized yet
        if (!SharedData.IsInitialized) return;
    }
}
