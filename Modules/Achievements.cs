using Il2Cpp;
namespace labyrinthine_library.Modules;

public class Achievements
{
    public static void Add (EAchievement achievement)
    {
        SaveSystem.TryToUnlockAchievement(achievement);
    }

    public static IEnumerable<EAchievement> List()
    {
        return Enum.GetValues(typeof(EAchievement)).Cast<EAchievement>();
    }

    public static void UnlockAll()
    {
        foreach (var achievement in List())
        {
            Add(achievement);
        }
    }

    public static void ServerUnlockAll(PlayerNetworkSync playerNetworkSync)
    {
        foreach (var achievement in List())
        {
            ServerAdd(playerNetworkSync, achievement);
        }
    }

    public static void ServerAdd(PlayerNetworkSync playerNetworkSync, EAchievement achievement)
    {
        playerNetworkSync.ServerUnlockAchievement(achievement);
    }
}
