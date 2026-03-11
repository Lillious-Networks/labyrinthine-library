using Il2CppValkoGames.Labyrinthine.Saves;
namespace labyrinthine_library.Modules;

// Complete

public class Levels
{
    public static int Increase(int levels)
    {
        return Set(Current() + levels);
    }

    public static int Decrease(int levels)
    {
        return Set(Current() - levels);
    }

    public static int Set(int level)
    {
        Experience.Decrease(Experience.Current());
        return Experience.Increase(Experience.RequiredForLevel(level));
    }

    public static int Current()
    {
        return Convert.ToInt32(PlayerSave.GetLevel());
    }
}
