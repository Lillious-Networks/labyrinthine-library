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

    public readonly bool IsInGame = SharedData.IsInGame;

    public readonly bool IsInMainMenu = SharedData.IsInMainMenu;

    public readonly bool IsNoClip = SharedData.IsNoClip;

    public readonly bool IsInitialized = SharedData.IsInitialized;

    public readonly bool HasInitialized = SharedData.HasInitialized;

    public readonly float NoclipShiftSpeed = SharedData.NoclipShiftSpeed;

    public readonly GameObject? Player = SharedData.Player;

    public readonly CharacterController? CharacterController = SharedData.CharacterController;

    public readonly PlayerController? PlayerController = SharedData.PlayerController;

    private static bool SetIsInGame (bool value) { return SharedData.IsInGame = value; }
    private static bool SetIsInMainMenu(bool value) { return SharedData.IsInMainMenu = value; }
    public bool SetIsNoClip(bool value) { return SharedData.IsNoClip = value; }
    private static bool SetIsInitialized(bool value) { return SharedData.IsInitialized = value; }
    private static bool SetHasInitialized(bool value) { return SharedData.HasInitialized = value; }
    public float SetNoclipShiftSpeed(float value) { return SharedData.NoclipShiftSpeed = value; }
    private static GameObject? SetPlayer() { return SharedData.Player ?? GameObject.FindGameObjectWithTag("LocalPlayer").gameObject; }
    private static CharacterController? SetCharacterController() { return SharedData.Player?.GetComponent<CharacterController>(); }
    private static PlayerController? SetPlayerController() { return SharedData.Player?.GetComponent<PlayerController>(); }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName) { Msg($"[SCENE LOADED] Name: {sceneName} Index: {buildIndex}"); }

    public override void OnSceneWasUnloaded(int buildIndex, string sceneName) { Msg($"[SCENE UNLOADED] Name: {sceneName} Index: {buildIndex}"); }

    public override async void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        Msg($"[SCENE INITIALIZED] Name: {sceneName} Index: {buildIndex}");

        // Set the IsInitialized flag if the game has initialized
        SetIsInitialized(sceneName == "Init" || false);

        // Set the isInMainMenu flag if in the main menu
        SetIsInMainMenu(sceneName == "MainMenu" || false);

        // Maps are typically after the 5th index - does not work
        // TODO: Fix
        if (buildIndex > 5)
        {
            await Task.Delay(5000);
            SetHasInitialized(true);
            SetIsInGame(true);
            if (SetPlayer() == null)
            {
                Msg("Unable to locate Player object!");
                SetHasInitialized(false);
            } else
            {
                SetPlayerController();
                SetCharacterController();
            }
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
        if (!IsInitialized) return;

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
