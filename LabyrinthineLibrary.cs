using MelonLoader;
using HarmonyLib;
using Il2CppValkoGames.Labyrinthine.AI;
using Il2CppValkoGames.Labyrinthine.Saves;
using UnityEngine;
using Il2CppValkoGames.Labyrinthine.Monsters;
using Il2CppRandomGeneration.Contracts;
using Il2CppValkoGames.Labyrinthine.Store;
using Il2Cpp;
using static MelonLoader.MelonLogger;
using Il2CppValkoGames.Labyrinthine.Misc;

namespace Labyrinthine_Library;

public class LabyrinthineLibrary : MelonMod
{

    public bool isInMainMenu = false;
    public bool isInitialized = false;
    public bool hasInitialized = false;
    public bool isInGame = false;
    public bool isNoclip = false;
    public float noclipShiftSpeed = 0;
    public GameObject? player;
    public CharacterController? characterController;

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        Msg($"[SCENE LOADED] Name: {sceneName} Index: {buildIndex}");
    }

    public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
    {
        Msg($"[SCENE UNLOADED] Name: {sceneName} Index: {buildIndex}");
    }

    public override async void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        Msg($"[SCENE INITIALIZED] Name: {sceneName} Index: {buildIndex}");

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
            await Task.Delay(5000);
            hasInitialized = true;
            isInGame = true;
            player = GameObject.FindGameObjectWithTag("LocalPlayer").gameObject;
            characterController = player.GetComponent<CharacterController>();
            if (player == null)
            {
                Msg("Unable to locate player object!");
                hasInitialized = false;
            }
        }
    }

    //  Player Save

    // Add experience to player
    public static int AddExperience(int experience)
    {
        PlayerSave.AddExperience(experience, true);
        return GetExperience();
    }

    // Removes experience from the player
    public static int RemoveExperience(int experience)
    {
        PlayerSave.AddExperience(-experience, true);
        return GetExperience();
    }

    // Adds the number of levels to the player dictated by "level"
    public static int AddLevel(int level)
    {
        // Gives the player the necessary experience for the selected levels added
        PlayerSave.AddExperience(PlayerSave.GetExperienceForLevel(PlayerSave.GetLevel() + level), true);
        return GetLevel();
    }

    // Returns the current experience of the player
    public static int GetExperience()
    {
        return PlayerSave.GetExperienceForLevel(PlayerSave.GetLevel());
    }

    // Returns the current level of the player
    public static int GetLevel()
    {
        return PlayerSave.GetLevel();
    }

    // Returns if the player skipped the tutorial
    public static bool SkippedTutorial()
    {
        return PlayerSave.SkippedTutorial;
    }

    // Returns if the player completed the tutorial
    public static bool CompletedTutorial()
    {
        return PlayerSave.CompletedTutorial;
    }

    // Changes whether the player has skipped the tutorial
    public static bool SkipTutorial(bool skip)
    {
        PlayerSave.SkippedTutorial = skip;
        return SkippedTutorial();
    }

    // Changes whether the player has completed the tutorial
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

    [HarmonyPatch(typeof(AIUtils), "IsInSafezone")]
    public static bool Prefix(ref bool __result)
    {
        __result = true;
        return false;
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
        // Return if not initialized
        if (!isInitialized) return;

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

        // Return if not in game or is uninitialized due to an error of some sort
        if (!isInitialized || !isInGame) return;

        /* PLAYER SPECIFIC CODE BEYOND HERE */

        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            // What isNoClip is set to, do the opposite
            if (characterController == null) return;
            isNoclip = !isNoclip;
            characterController.enableOverlapRecovery = !isNoclip;
            characterController.GetComponent<Collider>().enabled = !isNoclip;
        }

        if (isNoclip)
        {
            if (player == null) return;
            if (Input.GetKey(KeyCode.LeftShift)) {
                noclipShiftSpeed = 0.5f;
            } else
            {
                noclipShiftSpeed = 0;
            }

            // Up
            if (Input.GetKey(KeyCode.Space))
            {
                // Up Left
                if (Input.GetKey(KeyCode.A))
                {
                    player.transform.position += player.transform.up * (0.07f + noclipShiftSpeed);
                    player.transform.position -= player.transform.right * (0.07f + noclipShiftSpeed);
                }
                // Up Right
                else if (Input.GetKey(KeyCode.D))
                {
                    player.transform.position += player.transform.up * (0.07f + noclipShiftSpeed);
                    player.transform.position += player.transform.right * (0.07f + noclipShiftSpeed);
                }
                // Up Forward
                else if (Input.GetKey(KeyCode.W))
                {
                    player.transform.position += player.transform.up * (0.07f + noclipShiftSpeed);
                    player.transform.position += player.transform.forward * (0.07f + noclipShiftSpeed);
                }
                // Up Backwards
                else if (Input.GetKey(KeyCode.S))
                {
                    player.transform.position += player.transform.up * (0.07f + noclipShiftSpeed);
                    player.transform.position -= player.transform.forward * (0.07f + noclipShiftSpeed);
                }
                else
                {
                    player.transform.position += player.transform.up * (0.1f + noclipShiftSpeed);
                }
            }
            // Down
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                // Down Left
                if (Input.GetKey(KeyCode.A))
                {
                    player.transform.position -= player.transform.up * (0.07f + noclipShiftSpeed);
                    player.transform.position -= player.transform.right * (0.07f + noclipShiftSpeed);
                }
                // Down Right
                else if (Input.GetKey(KeyCode.D))
                {
                    player.transform.position -= player.transform.up * (0.07f + noclipShiftSpeed);
                    player.transform.position += player.transform.right * (0.07f + noclipShiftSpeed);
                }
                // Down Forward
                else if (Input.GetKey(KeyCode.W))
                {
                    player.transform.position -= player.transform.up * (0.07f + noclipShiftSpeed);
                    player.transform.position += player.transform.forward * (0.07f + noclipShiftSpeed);
                }
                // Down Backwards
                else if (Input.GetKey(KeyCode.S))
                {
                    player.transform.position -= player.transform.up * (0.07f + noclipShiftSpeed);
                    player.transform.position -= player.transform.forward * (0.07f + noclipShiftSpeed);
                }
                else
                {
                    player.transform.position -= player.transform.up * (0.1f + noclipShiftSpeed);
                }
            }
            // Left
            else if (Input.GetKey(KeyCode.A))
            {
                // Left and Right
                if (Input.GetKey(KeyCode.D))
                {
                    player.transform.position -= player.transform.right * (0.07f + noclipShiftSpeed);
                    player.transform.position += player.transform.right * (0.07f + noclipShiftSpeed);
                }
                // Left and Forward
                else if (Input.GetKey(KeyCode.W))
                {
                    player.transform.position -= player.transform.right * (0.07f + noclipShiftSpeed);
                    player.transform.position += player.transform.forward * (0.07f + noclipShiftSpeed);
                }
                // Left and Backwards
                else if (Input.GetKey(KeyCode.S))
                {
                    player.transform.position -= player.transform.right * (0.07f + noclipShiftSpeed);
                    player.transform.position -= player.transform.forward * (0.07f + noclipShiftSpeed);
                }
                else
                {
                    player.transform.position -= player.transform.right * (0.1f + noclipShiftSpeed);
                }
            }
            // Right
            else if (Input.GetKey(KeyCode.D))
            {
                // Right and Left
                if (Input.GetKey(KeyCode.A))
                {
                    player.transform.position += player.transform.right * (0.07f + noclipShiftSpeed);
                    player.transform.position -= player.transform.right * (0.07f + noclipShiftSpeed);
                }
                // Right and Forward
                else if (Input.GetKey(KeyCode.W))
                {
                    player.transform.position += player.transform.right * (0.07f + noclipShiftSpeed);
                    player.transform.position += player.transform.forward * (0.07f + noclipShiftSpeed);
                }
                // Right and Backwards
                else if (Input.GetKey(KeyCode.S))
                {
                    player.transform.position += player.transform.right * (0.07f + noclipShiftSpeed);
                    player.transform.position -= player.transform.forward * (0.07f + noclipShiftSpeed);
                }
                else
                {
                    player.transform.position += player.transform.right * (0.1f + noclipShiftSpeed);
                }
            }
            // Forward
            else if (Input.GetKey(KeyCode.W))
            {
                // Forward and Left
                if (Input.GetKey(KeyCode.A))
                {
                    player.transform.position += player.transform.forward * (0.07f + noclipShiftSpeed);
                    player.transform.position -= player.transform.right * (0.07f + noclipShiftSpeed);
                }
                // Forward and Right
                else if (Input.GetKey(KeyCode.D))
                {
                    player.transform.position += player.transform.forward * (0.07f + noclipShiftSpeed);
                    player.transform.position += player.transform.right * (0.07f + noclipShiftSpeed);
                }
                else
                {
                    player.transform.position += player.transform.forward * (0.1f + noclipShiftSpeed);
                }
            }
            // Backwards
            else if (Input.GetKey(KeyCode.S))
            {
                // Backwards and Left
                if (Input.GetKey(KeyCode.A))
                {
                    player.transform.position -= player.transform.forward * (0.07f + noclipShiftSpeed);
                    player.transform.position -= player.transform.right * (0.07f + noclipShiftSpeed);
                }
                // Backwards and Right
                else if (Input.GetKey(KeyCode.D))
                {
                    player.transform.position -= player.transform.forward * (0.07f + noclipShiftSpeed);
                    player.transform.position += player.transform.right * (0.07f + noclipShiftSpeed);
                }
                else
                {
                    player.transform.position -= player.transform.forward * (0.1f + noclipShiftSpeed);
                }
            }
    } else
        {

        }
    }

}
