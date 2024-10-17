using MelonLoader;
using HarmonyLib;
using Il2CppValkoGames.Labyrinthine.Saves;
using UnityEngine;
using Il2CppValkoGames.Labyrinthine.Monsters;
using Il2CppRandomGeneration.Contracts;
using Il2CppValkoGames.Labyrinthine.Store;
using static MelonLoader.MelonLogger;
using Il2CppValkoGames.Labyrinthine.Misc;
using Il2Cpp;

namespace Labyrinthine_Library;

public class SharedData
{
    public static bool IsInGame { get; set; }
    public static bool IsInMainMenu { get; set; }
    public static bool IsNoClip { get; set; }
    public static bool IsInitialized { get; set; }
    public static bool HasInitialized { get; set; }
    public static float NoclipShiftSpeed { get; set; } = 0;
    public static GameObject? Player { get; set; }
    public static CharacterController? CharacterController { get; set; }
    public static PlayerController? PlayerController { get; set; }
}

public class LabyrinthineLibrary : MelonMod
{
    public bool GetIsInGame() { return SharedData.IsInGame; }
    public bool GetIsInMainMenu() {  return SharedData.IsInMainMenu; }
    public bool SetIsNoClip(bool value) { return SharedData.IsNoClip = value; }
    public bool GetIsNoClip() {  return SharedData.IsNoClip; }
    public bool GetIsInitialized() { return SharedData.IsInitialized; }
    public bool GetHasInitialized() {  return SharedData.HasInitialized; }
    public float SetNoclipShiftSpeed(float value) { return SharedData.NoclipShiftSpeed = value; }
    public float GetNoclipShiftSpeed() {  return SharedData.NoclipShiftSpeed; }
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
        SharedData.IsInMainMenu = (sceneName == "MainMenu");

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
            } else
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
        } else
        {
            SharedData.IsInGame = false;
        }
    }

    //  Player Save

    // Add experience to Player
    public static int AddExperience(int experience)
    {
        PlayerSave.AddExperience(experience, true);
        return GetExperience();
    }

    // Removes experience from the Player
    public static int RemoveExperience(int experience)
    {
        PlayerSave.AddExperience(-experience, true);
        return GetExperience();
    }

    // Adds the number of levels to the Player dictated by "level"
    public static int AddLevel(int level)
    {
        // Gives the Player the necessary experience for the selected levels added
        PlayerSave.AddExperience(PlayerSave.GetExperienceForLevel(PlayerSave.GetLevel() + level), true);
        return GetLevel();
    }

    // Returns the current experience of the Player
    public static int GetExperience()
    {
        return PlayerSave.GetExperienceForLevel(PlayerSave.GetLevel());
    }

    // Returns the current level of the Player
    public static int GetLevel()
    {
        return PlayerSave.GetLevel();
    }

    // Returns if the Player skipped the tutorial
    public static bool SkippedTutorial()
    {
        return PlayerSave.SkippedTutorial;
    }

    // Returns if the Player completed the tutorial
    public static bool CompletedTutorial()
    {
        return PlayerSave.CompletedTutorial;
    }

    // Changes whether the Player has skipped the tutorial
    public static bool SkipTutorial(bool skip)
    {
        PlayerSave.SkippedTutorial = skip;
        return SkippedTutorial();
    }

    // Changes whether the Player has completed the tutorial
    public static bool CompleteTutorial(bool complete)
    {
        PlayerSave.CompletedTutorial = complete;
        return CompletedTutorial();
    }

    // EquipmentSave

    // Returns if the monster type is unlocked
    public static bool MonsterTypeUnlocked(MonsterType type)
    {
        return EquipmentSave.IsMonsterTypeUnlocked(type);
    }

    public static bool UnlockMonsterType(MonsterType type)
    {
        EquipmentSave.UnlockMonsterType(type);
        return EquipmentSave.IsMonsterTypeUnlocked(type);
    }

    // Returns if the maze type is unlocked
    public static bool MazeTypeUnlocked(MazeType type)
    {
        return EquipmentSave.IsMazeTypeUnlocked(type);
    }

    public static bool UnlockMazeType(MazeType type)
    {
        EquipmentSave.UnlockMazeType(type);
        return EquipmentSave.IsMazeTypeUnlocked(type);
    }

    // Adds the rare tokens based on parameter "amount"
    public static int AddRareTokens(int amount)
    {
        return EquipmentSave.custom_RareTokens += amount;
    }

    public static int RemoveRareTokens(int amount)
    {
        return EquipmentSave.custom_RareTokens -= amount;
    }

    public static int SetRareTokens(int amount)
    {
        return EquipmentSave.custom_RareTokens = amount;
    }

    public static int AddHardcoreTokens(int amount)
    {
        return EquipmentSave.custom_HardcoreTokens += amount;
    }

    public static int RemoveHardcoreTokens(int amount)
    {
        return EquipmentSave.custom_HardcoreTokens -= amount;
    }

    public static int SetHardcoreTokens(int amount)
    {
        return EquipmentSave.custom_HardcoreTokens = amount;
    }

    public static int AddCurrency(int amount)
    {
        CurrencyManager.AddCurrency(amount, true);
        return CurrencyManager.AvailableCurrency;
    }

    public static int RemoveCurrency(int amount)
    {
        CurrencyManager.RemoveCurrency(amount, true);
        return CurrencyManager.AvailableCurrency;
    }

    [HarmonyPatch(typeof(CheatingDetector), "isCheating")]
    private static class Patch_CheatDetection
    {
        public static bool PostFix(ref bool __result)
        {
            __result = false;
            return false;
        }
    }

    [HarmonyPatch(typeof(CheatingDetector), "CheatingDetected")]
    private static class Patch_CheatDetection2
    {
        public static bool PostFix(ref bool __result)
        {
            __result = false;
            return false;
        }
    }

    public override void OnUpdate()
    {
        // Game has not initialized yet
        if (!SharedData.IsInitialized) return;

        // Disable Anti Cheat
        if (GameObject.Find("Anti-Cheat Toolkit") != null && GameObject.Find("Anti-Cheat Toolkit").activeInHierarchy)
        {
            Msg("Anti Cheat Disabled!");
            GameObject.Find("Anti-Cheat Toolkit").SetActive(false);
        }
        if (CheatingDetector.CheatingDetected)
        {
            Msg("Cheating Detected");
            Application.Quit();
        }
    }
}
