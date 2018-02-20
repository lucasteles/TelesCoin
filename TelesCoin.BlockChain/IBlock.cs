using System.Collections.Generic;

namespace TelesCoin
{
    public interface IBlock
    {
        int BlockSize { get; }
        string Hash { get; }
        string PreviousHash { get; }
        IReadOnlyCollection<Transaction> Transactions { get; }
        string CalculateHash();
        void MineBlock(int difficulty);
    }
}