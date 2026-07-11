using Il2Cpp;
using UnityEngine;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppValkoGames.Labyrinthine.Monsters;

namespace labyrinthine_library.Modules;

public class RemoteControl
{
    public static void ChangeBody(PlayerNetworkSync playerNetworkSync, PlayerBody playerBody)
    {
        playerNetworkSync.ChangeBody(playerBody, true, true);
    }

    public static void ThrowGlowstick(PlayerNetworkSync playerNetworkSync, byte strength)
    {
        playerNetworkSync.ThrowGlowstick(strength);
    }

    public static void KillPlayer(PlayerNetworkSync playerNetworkSync, bool instantRespawn, MonsterType monster, byte blackscreenDuration)
    {
        playerNetworkSync.DoDeath(instantRespawn, monster, blackscreenDuration);
    }

    public static void SendVoiceMessage(PlayerNetworkSync playerNetworkSync, ushort bufferLen, Il2CppStructArray<byte> data)
    {
        playerNetworkSync.SendVoice(bufferLen, data);
    }

    public static void CycleGlowstick(PlayerNetworkSync playerNetworkSync)
    {
        playerNetworkSync.CycleGlowstick();
    }

    public static void TeleportPlayer(PlayerNetworkSync playerNetworkSync, Vector3 position, Quaternion rotation = default)
    {
        playerNetworkSync.MoveToPosition(position, rotation);
    }

    public static void CameraShake(PlayerNetworkSync playerNetworkSync, float duration, float strength = 90f, int vibrato = 10, float randomness = 90f, bool fadeOut = true)
    {
        playerNetworkSync.ShakeCamera(duration, strength, vibrato, randomness, fadeOut);
    }

    public static void ServerLoadLobby(PlayerNetworkSync playerNetworkSync)
    {
        playerNetworkSync.ServerLoadLobby();
    }
}