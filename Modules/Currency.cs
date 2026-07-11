using Il2CppValkoGames.Labyrinthine.Saves;
using Il2CppValkoGames.Labyrinthine.Store;
using UnityEngine;
namespace labyrinthine_library.Modules;


public enum CurrencyType
{
    HardcoreTokens,
    RareTokens,
    Tickets
}

public class Currency
{
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
        GameObject Store = GameObject.Find("- Store Network Sync");
        if (Store == null)
        {
            Logs.Error("Unable to locate store. Are you in the game lobby?");
            return;
        }

        StoreNetworkSync StoreNetwork = Store.GetComponent<StoreNetworkSync>();

        // Host only
        if (!StoreNetwork.authority)
        {
            Logs.Error("You do not have permission to donate currency. Are you the host?");
            return;
        }

        StoreNetwork.DonateCurrencyToHost(amount);
    }
}
