using UnityEngine;
using Il2CppMirror;
using Il2CppValkoGames.Labyrinthine.Cases.Items;
using Il2CppValkoGames.Labyrinthine.Cases.Inventory.Items;
using Il2CppValkoGames.Labyrinthine.Interactions.Story.Zone6;

namespace labyrinthine_library.Modules;

public enum ItemType
{
    Flaregun,
    SprintDrink,
    SprayPaint,
    CompassAttractor,
    RitualDagger,
    Map
}

public class Items
{
    private static RndItemPickup[] itemPickups = Array.Empty<RndItemPickup>();

    public static RndItemPickup[] List()
    {
        itemPickups = UnityEngine.Object.FindObjectsOfType<RndItemPickup>();
        return itemPickups;
    }

    public static GameObject[] GetPrefabs()
    {
        var manager = NetworkManager.singleton;
        if (manager == null)
            return Array.Empty<GameObject>();

        var prefabs = new List<GameObject>();
        foreach (var prefab in manager.spawnPrefabs)
        {
            if (prefab != null && prefab.GetComponent<RndItemPickup>() != null)
                prefabs.Add(prefab);
        }
        return prefabs.ToArray();
    }

    public static GameObject? GetPrefab(ItemType type)
    {
        var manager = NetworkManager.singleton;
        if (manager == null)
            return null;

        foreach (var prefab in manager.spawnPrefabs)
        {
            if (prefab == null)
                continue;

            if (prefab.GetComponent<RndItemPickup>() == null)
                continue;

            if (MatchItemType(prefab, type))
                return prefab;
        }

        return null;
    }

    private static bool MatchItemType(GameObject prefab, ItemType type)
    {
        return type switch
        {
            ItemType.Flaregun          => HasComponent<RndFlaregunItem>(prefab),
            ItemType.SprintDrink       => HasComponent<RndSprintDrinkItem>(prefab),
            ItemType.SprayPaint        => HasComponent<SprayPaintItem>(prefab),
            ItemType.CompassAttractor  => HasComponent<CompassAttractorItem>(prefab),
            ItemType.RitualDagger      => HasComponent<RitualDaggerItem>(prefab),
            _                          => prefab.name.Contains(type.ToString())
        };
    }

    private static bool HasComponent<T>(GameObject prefab) where T : MonoBehaviour
    {
        return prefab.GetComponentInChildren<T>() != null;
    }

    public static void SpawnPickup(ItemType type, Vector3 position)
    {
        if (!NetworkServer.active)
        {
            Logs.Error("Item spawning requires server authority");
            return;
        }

        var prefab = GetPrefab(type);
        if (prefab == null)
        {
            Logs.Error($"Item prefab not found for {type}");
            return;
        }

        var instance = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
        NetworkServer.Spawn(instance);

        Logs.Info($"Spawned {prefab.name}");
    }

    public static void SpawnPickupNearPlayer(GameObject player, ItemType type, float offsetDistance = 2f)
    {
        if (player == null)
        {
            Logs.Error("Player reference is null");
            return;
        }

        var prefab = GetPrefab(type);
        if (prefab == null)
        {
            Logs.Error($"Item prefab not found for {type}");
            return;
        }

        Vector3 position = player.transform.position + player.transform.forward * offsetDistance;
        SpawnPickup(type, position);
    }

    public static void SpawnPickupNearPlayer(ItemType type, float offsetDistance = 2f)
    {
        var player = Library.Player();
        if (player == null)
        {
            Logs.Error("Local player not found");
            return;
        }

        SpawnPickupNearPlayer(player, type, offsetDistance);
    }
}
