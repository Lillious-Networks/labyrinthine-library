using Il2CppRandomGeneration.Contracts;
using Il2CppValkoGames.Labyrinthine.Monsters;
using Il2CppValkoGames.Labyrinthine.Saves;

namespace labyrinthine_library.Modules;

public class Saves
{
    public static bool SkippedTutorial()
    {
        return PlayerSave.SkippedTutorial;
    }

    public static bool CompletedTutorial()
    {
        return PlayerSave.CompletedTutorial;
    }

    public static bool SkipTutorial(bool skip)
    {
        return PlayerSave.SkippedTutorial = skip;
    }

    public static bool CompleteTutorial(bool complete)
    {
        return PlayerSave.CompletedTutorial = complete;
    }

    public static bool MonsterTypeUnlocked(MonsterType type)
    {
        return EquipmentSave.IsMonsterTypeUnlocked(type);
    }

    public static bool UnlockMonsterType(MonsterType type)
    {
        EquipmentSave.UnlockMonsterType(type);
        return EquipmentSave.IsMonsterTypeUnlocked(type);
    }

    public static bool MazeTypeUnlocked(MazeType type)
    {
        return EquipmentSave.IsMazeTypeUnlocked(type);
    }

    public static bool UnlockMazeType(MazeType type)
    {
        EquipmentSave.UnlockMazeType(type);
        return EquipmentSave.IsMazeTypeUnlocked(type);
    }
}