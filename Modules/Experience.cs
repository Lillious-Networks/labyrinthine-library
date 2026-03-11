using Il2CppValkoGames.Labyrinthine.Saves;
namespace labyrinthine_library.Modules;

// Complete

public class Experience
{
    public static int Increase(int experience)
    {
        PlayerSave.AddExperience(experience);
        return Current();
    }

    public static int Decrease(int experience)
    {
        PlayerSave.AddExperience(-experience);
        return Current();
    }

    public static int Set(int experience)
    {
        // Reset level to 0
        Levels.Set(0);
        PlayerSave.AddExperience(experience, true);
        return Current();
    }

    public static int Current()
    {
        return PlayerSave.GetExperienceForLevel(Levels.Current());
    }

    public static int RequiredForLevel(int level)
    {
        return PlayerSave.GetExperienceForLevel(level);
    }
}
