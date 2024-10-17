using Il2CppValkoGames.Labyrinthine.Store;
using MelonLoader;

namespace labyrinthine_library.Currency
{
    public class Currency : MelonMod
    {
        public int AddCurrency(int amount)
        {
            CurrencyManager.AddCurrency(amount, true);
            return CurrencyManager.AvailableCurrency;
        }

        public int RemoveCurrency(int amount)
        {
            CurrencyManager.RemoveCurrency(amount, true);
            return CurrencyManager.AvailableCurrency;
        }
    }
}
