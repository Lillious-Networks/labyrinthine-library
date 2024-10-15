using MelonLoader;
using HarmonyLib;
using Il2CppValkoGames.Labyrinthine.AI;
using Il2CppValkoGames.Labyrinthine.Saves;
using UnityEngine;
using Il2CppValkoGames.Labyrinthine.Monsters;
using Il2CppRandomGeneration.Contracts;
using Il2CppValkoGames.Labyrinthine.Store;
using Il2Cpp;

namespace Labyrinthine_Library;

public class LabyrinthineLibrary : MelonMod
{

    private bool isInMainMenu = false;
    private bool isInitialized = false;
    private bool hasInitialized = false;
    private bool isInGame = false;

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        MelonLogger.Msg($"[SCENE LOADED] Name: {sceneName} Index: {buildIndex}");
    }

    public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
    {
        MelonLogger.Msg($"[SCENE UNLOADED] Name: {sceneName} Index: {buildIndex}");
    }

    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        MelonLogger.Msg($"[SCENE INITIALIZED] Name: {sceneName} Index: {buildIndex}");

        if (sceneName == "Init")
        {
            isInitialized = true;
        }

        // Determines if the game scene is the MainMenu
        if (sceneName == "MainMenu")
        {
            isInMainMenu = true;
        }
        else
        {
            isInMainMenu = false;
        }

        // Determ
        if (buildIndex > 5)
        {
            isInGame = true;
        } else
        {
            isInGame = false;
        }
    }

                //  Player Save

    // Add experience to player
    public static int AddExperience (int experience)
    {
        PlayerSave.AddExperience(experience, true);
        return GetExperience();
    }

    // Removes experience from the player
    public static int RemoveExperience (int experience)
    {
        PlayerSave.AddExperience(-experience, true);
        return GetExperience();
    }

    // Adds the number of levels to the player dictated by "level"
    public static int AddLevel (int level)
    {
        // Gives the player the necessary experience for the selected levels added
        PlayerSave.AddExperience(PlayerSave.GetExperienceForLevel(PlayerSave.GetLevel() + level), true);
        return GetLevel();
    }

    // Returns the current experience of the player
    public static int GetExperience ()
    {
        return PlayerSave.GetExperienceForLevel(PlayerSave.GetLevel());
    }

    // Returns the current level of the player
    public static int GetLevel ()
    {
        return PlayerSave.GetLevel();
    }

    // Returns if the player skipped the tutorial
    public static bool SkippedTutorial ()
    {
        return PlayerSave.SkippedTutorial;
    }

    // Returns if the player completed the tutorial
    public static bool CompletedTutorial()
    {
        return PlayerSave.CompletedTutorial;
    }

    // Changes whether the player has skipped the tutorial
    public static bool SkipTutorial (bool skip)
    {
        PlayerSave.SkippedTutorial = skip;
        return SkippedTutorial();
    }

    // Changes whether the player has completed the tutorial
    public static bool CompleteTutorial (bool complete)
    {
        PlayerSave.CompletedTutorial = complete;
        return CompletedTutorial();
    }

                // EquipmentSave

    // Returns if the monster type is unlocked
    public static bool MonsterTypeUnlocked (MonsterType type)
    {
        return EquipmentSave.IsMonsterTypeUnlocked(type);
    }

    public static bool UnlockMonsterType (MonsterType type)
    {
        EquipmentSave.UnlockMonsterType(type);
        return EquipmentSave.IsMonsterTypeUnlocked(type);
    }

    // Returns if the maze type is unlocked
    public static bool MazeTypeUnlocked (MazeType type)
    {
        return EquipmentSave.IsMazeTypeUnlocked(type);
    }

    public static bool UnlockMazeType (MazeType type)
    {
        EquipmentSave.UnlockMazeType(type);
        return EquipmentSave.IsMazeTypeUnlocked(type);
    }

    // Adds the rare tokens based on parameter "amount"
    public static int AddRareTokens (int amount)
    {
        return EquipmentSave.custom_RareTokens += amount;
    }

    public static int RemoveRareTokens (int amount)
    {
        return EquipmentSave.custom_RareTokens -= amount;
    }

    public static int SetRareTokens (int amount)
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

    [HarmonyPatch(typeof(AIUtils), "IsInSafezone")]
    public static bool Prefix(ref bool __result)
    {
        __result = true;
        return false;
    }

    private PlayerController? player = null;

    private void init ()
    {
        hasInitialized = true;

        // Get PlayerController

        player = UnityEngine.Object.FindObjectOfType<PlayerController>().gameObject.GetComponent<PlayerController>();
        if (player != null )
        {
            MelonLogger.Msg("Found PlayerController");
        }
        else
        {
            MelonLogger.Msg("PlayerController not found");
        }
    }

    private bool isNoclip = false;

    public override void OnUpdate()
    {
        // Return if not initialized
        if (!isInitialized) return;
        if (!hasInitialized && isInGame)
        {
            init();
        }
        
        if (Input.GetKeyDown(KeyCode.Tilde) && isInGame)
        {
            // What isNoClip is set to, do the opposite
            _ = !isNoclip;
        }

        if (isNoclip)
        {

        }
    }

}
