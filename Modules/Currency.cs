using Il2CppValkoGames.Labyrinthine.Saves;
using Il2CppValkoGames.Labyrinthine.Store;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace labyrinthine_library.Modules;

// Completed

public enum CurrencyType
{
    HardcoreTokens,
    RareTokens,
    Tickets
}

public class Currency
{
    private static StoreNetworkSync? Store = null; // Cache StoreNetworkSync object
    public static int Increase(CurrencyType currency, int amount)
    {
        if (currency == CurrencyType.Tickets)
        {
            CurrencyManager.AddCurrency(amount, true);
            return CurrencyManager.AvailableCurrency;
        }
        
        if (currency == CurrencyType.HardcoreTokens)
        {
            return EquipmentSave.custom_HardcoreTokens += amount;
        }

        if (currency == CurrencyType.RareTokens)
        {
            return EquipmentSave.custom_RareTokens += amount;
        }

        return 0;
    }

    public static int Decrease(CurrencyType currency, int amount)
    {
        if (currency == CurrencyType.Tickets)
        {
            CurrencyManager.RemoveCurrency(amount, true);
            return CurrencyManager.AvailableCurrency;
        }

        if (currency == CurrencyType.HardcoreTokens)
        {
            return EquipmentSave.custom_HardcoreTokens -= amount;
        }

        if (currency == CurrencyType.RareTokens)
        {
            return EquipmentSave.custom_RareTokens -= amount;
        }

        return 0;
    }

    public static int Current(CurrencyType currency)
    {
        if (currency == CurrencyType.Tickets) { return CurrencyManager.AvailableCurrency; }

        if (currency == CurrencyType.HardcoreTokens) { return EquipmentSave.custom_HardcoreTokens; }

        if (currency == CurrencyType.RareTokens) { return EquipmentSave.custom_RareTokens;  }

        return 0;
    }

    // Tickets are not supported
    public static int Set(CurrencyType currency, int amount)
    {
        if (currency == CurrencyType.HardcoreTokens)
        {
            return EquipmentSave.custom_HardcoreTokens = amount;
        }

        if (currency == CurrencyType.RareTokens)
        {
            return EquipmentSave.custom_RareTokens = amount;
        }

        return 0;
    }

    public static bool Afforable(int amount)
    {
        return CurrencyManager.CanAfford(amount);
    }

    public static void DonateCurrency(int amount)
    {
        // Must be in lobby where Store Network Sync is available
        if (!Library.IsInLobby()) return;
        // Attempt to assign store if undefined
        if (Store == null) Store = GameObject.Find("- Store Network Sync")?.GetComponent<StoreNetworkSync>();
        if (Store == null) return; // Could not find store after attempt
        // Host only
        if (!Store.authority) return;
        Store.DonateCurrencyToHost(amount);
    }
}
