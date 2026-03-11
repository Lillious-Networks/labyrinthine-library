using UnityEngine;
using UnityEngine.AI;
using Il2CppValkoGames.Labyrinthine.Monsters;

namespace labyrinthine_library.Modules;

public class Monsters
{
    private static AIController[] monsters = Array.Empty<AIController>();

    public static AIController[] List()
    {
        if (!Library.IsInGame())
            return Array.Empty<AIController>();

        monsters = UnityEngine.Object.FindObjectsOfType<AIController>();
        return monsters;
    }

    public static void NavigateToTarget(GameObject? player)
    {
        if (!Library.IsInGame())
            return;

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

            Library.ShowToast($"Navigating all monsters to target player");
        }
    }
}