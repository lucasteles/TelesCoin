
using System;
using System.Collections.Generic;

namespace TelesCoin
{
    public delegate IBlock CreateBlockDelegate(DateTime created, string previousHash, int blockSize, IEnumerable<Transaction> pendingTransactions);
    public class BlockFactory
    {
        public static Block CreateBlock(DateTime created, string previousHash, int blockSize, IEnumerable<Transaction> pendingTransactions) =>
                                         new Block(created, blockSize, pendingTransactions, previousHash);
    }
}
