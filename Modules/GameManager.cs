using Il2Cpp;
using UnityEngine;

namespace labyrinthine_library.Modules;

public class GameManager
{
    public static bool CanRespawn ()
    {
        return Il2Cpp.GameManager.instance.CanRespawn;
    }

    public static Il2CppSystem.Collections.Generic.List<PlayerNetworkSync> Players()
    {
        return Il2Cpp.GameManager.instance.Players;
    }

    public static Il2CppSystem.Collections.Generic.List<PlayerNetworkSync> VRPlayers()
    {
        var vrPlayers = new Il2CppSystem.Collections.Generic.List<PlayerNetworkSync>();
        foreach (var player in Il2Cpp.GameManager.instance.Players)
        {
            if (player.IsUsingVR)
            {
                vrPlayers.Add(player);
            }
        }
        return vrPlayers;
    }

    public static Il2CppSystem.Collections.Generic.List<PlayerNetworkSync> NonVRPlayers()
    {
        var players = new Il2CppSystem.Collections.Generic.List<PlayerNetworkSync>();
        foreach (var player in Il2Cpp.GameManager.instance.Players)
        {
            if (!player.IsUsingVR)
            {
                players.Add(player);
            }
        }
        return players;
    }

    public static GameObject Player ()
    {
        return Il2Cpp.GameManager.instance.mainPlayer;
    }

    public static Il2CppSystem.Collections.Generic.List<PlayerNetworkSync> AlivePlayers()
    {
        return Il2Cpp.GameManager.instance.AlivePlayers;
    }
    public static int AlivePlayersCount()
    {
        return Il2Cpp.GameManager.instance.GetAlivePlayerCount();
    }

    public static int VRPlayersCount()
    {
        return Il2Cpp.GameManager.instance.GetVRPlayersCount();
    }

    public static PlayerNetworkSync GetPlayerNetworkSync(GameObject player)
    {
        return player.GetComponent<PlayerNetworkSync>();
    }
}
