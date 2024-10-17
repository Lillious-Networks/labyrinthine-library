using MelonLoader;
using Il2CppRandomGeneration.Contracts;
using Il2CppValkoGames.Labyrinthine.Monsters;
using Il2CppValkoGames.Labyrinthine.Saves;

namespace labyrinthine_library.Saves;

public class Saves : MelonMod
{

    // Add experience to Player
    public int AddExperience(int experience)
    {
        PlayerSave.AddExperience(experience, true);
        return GetExperience();
    }

    // Removes experience from the Player
    public int RemoveExperience(int experience)
    {
        PlayerSave.AddExperience(-experience, true);
        return GetExperience();
    }

    // Adds the number of levels to the Player dictated by "level"
    public int AddLevel(int level)
    {
        // Gives the Player the necessary experience for the selected levels added
        PlayerSave.AddExperience(PlayerSave.GetExperienceForLevel(PlayerSave.GetLevel() + level), true);
        return GetLevel();
    }

    // Returns the current experience of the Player
    public int GetExperience()
    {
        return PlayerSave.GetExperienceForLevel(PlayerSave.GetLevel());
    }

    // Returns the current level of the Player
    public int GetLevel()
    {
        return PlayerSave.GetLevel();
    }

    // Returns if the Player skipped the tutorial
    public bool SkippedTutorial()
    {
        return PlayerSave.SkippedTutorial;
    }

    // Returns if the Player completed the tutorial
    public bool CompletedTutorial()
    {
        return PlayerSave.CompletedTutorial;
    }

    // Changes whether the Player has skipped the tutorial
    public bool SkipTutorial(bool skip)
    {
        return PlayerSave.SkippedTutorial = skip;
    }

    // Changes whether the Player has completed the tutorial
    public bool CompleteTutorial(bool complete)
    {
        return PlayerSave.CompletedTutorial = complete;
    }

    // Returns if the monster type is unlocked
    public bool MonsterTypeUnlocked(MonsterType type)
    {
        return EquipmentSave.IsMonsterTypeUnlocked(type);
    }

    public bool UnlockMonsterType(MonsterType type)
    {
        EquipmentSave.UnlockMonsterType(type);
        return EquipmentSave.IsMonsterTypeUnlocked(type);
    }

    // Returns if the maze type is unlocked
    public bool MazeTypeUnlocked(MazeType type)
    {
        return EquipmentSave.IsMazeTypeUnlocked(type);
    }

    public bool UnlockMazeType(MazeType type)
    {
        EquipmentSave.UnlockMazeType(type);
        return EquipmentSave.IsMazeTypeUnlocked(type);
    }

    // Adds the rare tokens based on parameter "amount"
    public int AddRareTokens(int amount)
    {
        return EquipmentSave.custom_RareTokens += amount;
    }

    public int RemoveRareTokens(int amount)
    {
        return EquipmentSave.custom_RareTokens -= amount;
    }

    public int SetRareTokens(int amount)
    {
        return EquipmentSave.custom_RareTokens = amount;
    }

    public int AddHardcoreTokens(int amount)
    {
        return EquipmentSave.custom_HardcoreTokens += amount;
    }

    public int RemoveHardcoreTokens(int amount)
    {
        return EquipmentSave.custom_HardcoreTokens -= amount;
    }

    public int SetHardcoreTokens(int amount)
    {
        return EquipmentSave.custom_HardcoreTokens = amount;
    }
}