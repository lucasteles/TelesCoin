using System;

namespace TelesCoin
{
    public class Transaction
    {
        public Guid FromAddress { get; set; }
        public Guid ToAdress { get; set; }
        public double Amount { get; set; }

        public Transaction(Guid fromAddress, Guid toAdress, double amount)
        {
            FromAddress = fromAddress;
            ToAdress = toAdress;
            Amount = amount;
        }

        public override string ToString() => $"{FromAddress}|{ToAdress}|{Amount}";

        public static Transaction FromNetwork(NetworkConsense networkConsense, Guid address) =>
            new Transaction(Guid.Empty, address, networkConsense.MiningReward);
    }
}
