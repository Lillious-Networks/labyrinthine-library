using UnityEngine;
using Il2CppMirror;
using Il2CppCharacterCustomization;

namespace labyrinthine_library.Modules;

public enum CosmeticType
{
    None = 0,

    // Head
    Head_Default,
    Head_Beanie,
    Head_CowboyHat,
    Head_TopHat,
    Head_ClownWig,
    Head_Crown,
    Head_BucketHat,
    Head_Fedora,
    Head_VikingHelmet,
    Head_Pumpkin,
    Head_SantaHat,
    Head_WitchHat,
    Head_BunnyEars,
    Head_PropellerHat,
    Head_CatEars,
    Head_ZombieMask,
    Head_PigMask,
    Head_TVHead,
    Head_Bandana,
    Head_BaseballCap,
    Head_HardHat,
    Head_PirateHat,
    Head_WizardHat,

    // Clothing
    Clothing_Default,
    Clothing_TShirt,
    Clothing_Hoodie,
    Clothing_Jacket,
    Clothing_Vest,
    Clothing_Suit,
    Clothing_Dress,
    Clothing_Overalls,
    Clothing_LabCoat,

    // Wrist
    Wrist_Default,
    Wrist_Watch,
    Wrist_Bracelet,

    // Flashlight
    Flashlight_Default,
    Flashlight_Police,
    Flashlight_Military,
    Flashlight_Old,

    // Lantern
    Lantern_Default,
    Lantern_Pumpkin,
    Lantern_Christmas,
    Lantern_Skull,
    Lantern_Gothic,

    // Glowstick
    Glowstick_Default,

    // Face
    Face_Default,
    Face_ClownPaint,
    Face_SkullPaint,
    Face_TigerPaint,
    Face_DiaDeLosMuertos,
    Face_Stitches,
    Face_Tears,
    Face_Zombie,

    // Finger
    Finger_Default,
    Finger_Ring,
    Finger_Clown,
    Finger_Skeletal,

    // Music
    Music_Default,
    Music_Eerie,
    Music_Epic,
    Music_Spooky,
    Music_Chill,
    Music_Retro
}

public class Cosmetics
{
    private static readonly Dictionary<CosmeticType, ushort> idMap = new();
    private static CustomizationPickup[] pickups = Array.Empty<CustomizationPickup>();

    public static void Map(CosmeticType type, ushort id)
    {
        idMap[type] = id;
    }

    public static void ClearMap()
    {
        idMap.Clear();
    }

    public static Dictionary<ushort, string> ListAvailable()
    {
        var result = new Dictionary<ushort, string>();
        var collection = CustomizationManager.Instance?.Collection;
        if (collection == null)
        {
            Logs.Error("ItemsCollectionSO not found");
            return result;
        }

        foreach (var item in collection.collection)
        {
            if (item == null)
                continue;

            var name = item.Name;
            if (string.IsNullOrEmpty(name))
                name = item.localisationKey;

            var label = $"{item.BodyPart} | {name}";
            result[item.ItemID] = label;
        }

        return result;
    }

    public static CustomizationPickup[] List()
    {
        pickups = UnityEngine.Object.FindObjectsOfType<CustomizationPickup>();
        return pickups;
    }

    public static GameObject[] GetPrefabs()
    {
        var manager = NetworkManager.singleton;
        if (manager == null)
            return Array.Empty<GameObject>();

        var prefabs = new List<GameObject>();
        foreach (var prefab in manager.spawnPrefabs)
        {
            if (prefab != null && prefab.GetComponent<CustomizationPickup>() != null)
                prefabs.Add(prefab);
        }
        return prefabs.ToArray();
    }

    public static void SpawnPickup(ushort customizationId, Vector3 position)
    {
        if (!NetworkServer.active)
        {
            Logs.Error("Item spawning requires server authority");
            return;
        }

        var prefabs = GetPrefabs();
        if (prefabs.Length == 0)
        {
            Logs.Error("No CustomizationPickup prefabs found");
            return;
        }

        var prefab = prefabs[0];
        var instance = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
        var pickup = instance.GetComponent<CustomizationPickup>();
        if (pickup != null)
            pickup.itemID = customizationId;

        NetworkServer.Spawn(instance);

        Logs.Info($"Spawned cosmetic #{customizationId}");
    }

    public static void SpawnPickup(CosmeticType type, Vector3 position)
    {
        if (idMap.TryGetValue(type, out var id))
        {
            SpawnPickup(id, position);
            return;
        }

        id = (ushort)type;
        if (id == 0)
        {
            Logs.Error($"No ID mapped for {type}. Call Cosmetics.Map() or set its enum value to a valid item ID.");
            return;
        }

        SpawnPickup(id, position);
    }

    public static void SpawnPickupNearPlayer(GameObject player, ushort customizationId, float offsetDistance = 2f)
    {
        if (player == null)
        {
            Logs.Error("Player reference is null");
            return;
        }

        Vector3 position = player.transform.position + player.transform.forward * offsetDistance;
        SpawnPickup(customizationId, position);
    }

    public static void SpawnPickupNearPlayer(GameObject player, CosmeticType type, float offsetDistance = 2f)
    {
        if (idMap.TryGetValue(type, out var id))
        {
            SpawnPickupNearPlayer(player, id, offsetDistance);
            return;
        }

        id = (ushort)type;
        if (id == 0)
        {
            Logs.Error($"No ID mapped for {type}. Call Cosmetics.Map() or set its enum value to a valid item ID.");
            return;
        }

        SpawnPickupNearPlayer(player, id, offsetDistance);
    }

    public static void SpawnPickupNearPlayer(ushort customizationId, float offsetDistance = 2f)
    {
        var player = Library.Player();
        if (player == null)
        {
            Logs.Error("Local player not found");
            return;
        }

        SpawnPickupNearPlayer(player, customizationId, offsetDistance);
    }

    public static void SpawnPickupNearPlayer(CosmeticType type, float offsetDistance = 2f)
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
