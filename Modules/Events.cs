using Il2Cpp;

namespace labyrinthine_library.Modules;

public class Events
{
    public static Events Instance { get; } = new();

    public event Action<PlayerNetworkSync>? OnPlayerDeath;

    public void InvokePlayerDeath(PlayerNetworkSync player) => OnPlayerDeath?.Invoke(player);
}
