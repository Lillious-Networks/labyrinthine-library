using Il2Cpp;
using UnityEngine;

namespace labyrinthine_library.SharedData
{
    public class SharedData
    {
        public static bool IsInGame { get; set; }
        public static bool IsInMainMenu { get; set; }
        public static bool IsNoClip { get; set; }
        public static bool IsInitialized { get; set; }
        public static bool HasInitialized { get; set; }
        public static float NoclipShiftSpeed { get; set; } = 0;
        public static GameObject? Player { get; set; }
        public static CharacterController? CharacterController { get; set; }
        public static PlayerController? PlayerController { get; set; }
    }
}
