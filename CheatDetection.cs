using Il2CppValkoGames.Labyrinthine.Misc;
using MelonLoader;
using HarmonyLib;

namespace labyrinthine_library.CheatDetection
{
    internal class CheatDetection : MelonMod
    {
        [HarmonyPatch(typeof(CheatingDetector), "isCheating")]
        private static class Patch_CheatDetection
        {
            public static bool PostFix(ref bool __result)
            {
                __result = false;
                return false;
            }
        }

        [HarmonyPatch(typeof(CheatingDetector), "CheatingDetected")]
        private static class Patch_CheatDetection2
        {
            public static bool PostFix(ref bool __result)
            {
                __result = false;
                return false;
            }
        }
    }
}
