using Il2Cpp;
using Il2CppRandomGeneration.Contracts;
using Il2CppValkoGames.Labyrinthine;

namespace labyrinthine_library.Modules;

public class Contracts
{
    public static void SetContract(Contract contract, string scene)
    {
        GameValues.instance.SetContract(contract, scene);
    }

    public static void SetRandomContact()
    {
        SetContract(GameValues.instance.RandomMazeContract, GameValues.instance.scene);
    }

    public static int Seed()
    {
        return GameValues.GetSeedFromLobbyID();
    }

    public static int Lives()
    {
        return GameValues.instance.Lives;
    }

    public static NetworkID LobbyId()
    {
        return GameValues.instance.LobbyID;
    }

    public static bool InfiniteLives()
    {
        return GameValues.instance.InfiniteLives;
    }

    public static Contract RandomMazeContract()
    {
        return GameValues.instance.RandomMazeContract;
    }

    public static string SceneName()
    {
        return GameValues.instance.scene;
    }

    public static bool HasLantern()
    {
        return GameValues.instance.hasLantern;
    }

    public static bool HasFlashlight()
    {
        return GameValues.instance.hasFlashlight;
    }

    public static bool HasGlowstick()
    {
        return GameValues.instance.hasGlowstick;
    }

    public static bool HasRadio()
    {
        return GameValues.instance.hasRadio;
    }

    public static bool HasCompass()
    {
        return GameValues.instance.hasCompass;
    }

    public static bool HasTapePlayer()
    {
        return GameValues.instance.hasTapePlayer;
    }
}