using UnityEngine;
using UnityEngine.AI;
using Il2CppMirror;
using Il2CppValkoGames.Labyrinthine.Monsters;

namespace labyrinthine_library.Modules;

public class Monsters
{
    private static AIController[] monsters = Array.Empty<AIController>();

    public static AIController[] List()
    {
        monsters = UnityEngine.Object.FindObjectsOfType<AIController>();
        return monsters;
    }

    public static void NavigateToTarget(GameObject? player)
    {
        if (player == null) return;

        foreach (var monster in List())
        {
            if (monster == null)
                continue;

            if (!monster.HasNavMeshAgent)
                continue;

            NavMeshAgent agent = monster.NavMeshAgent;

            if (agent == null)
                continue;

            agent.SetDestination(player.transform.position);
        }
    }

    public static MonsterType[] Types()
    {
        return Enum.GetValues(typeof(MonsterType)).Cast<MonsterType>().ToArray();
    }

    public static void Spawn(GameObject player, MonsterType type, float offsetDistance = 5f)
    {
        if (!NetworkServer.active)
        {
            Logs.Error("Server authority not active while attempting to spawn a monster");
            return;
        }

        var manager = NetworkManager.singleton;
        if (manager == null)
        {
            Logs.Error("NetworkManager not found while attempting to spawn a monster");
            return;
        }

        if (player == null)
        {
            Logs.Error("Player reference not found while attempting to spawn a monster");
            return;
        }

        GameObject? prefab = null;
        foreach (var p in manager.spawnPrefabs)
        {
            AIController controller = p.GetComponent<AIController>();
            if (controller != null && controller.monsterType == type)
            {
                prefab = p;
                break;
            }
        }

        if (prefab == null)
        {
            Logs.Error($"No prefab found for monster type: {type}");
            return;
        }

        Vector3 position = player != null
            ? player.transform.position + player.transform.forward * offsetDistance
            : Vector3.zero;

        var instance = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
        NetworkServer.Spawn(instance);
    }
}