using System;
using System.Collections.Generic;
using System.Linq;

namespace TelesCoin.Test
{
    public class MockBlock : IBlock
    {

        public MerkleTree<Transaction> Transactions { get; set; }
        public int BlockSize { get; set; }
        public string Hash { get; set; }
        public string PreviousHash { get; set; }
        public string CalculateHash() => GetHashCode().ToString();

        public MockBlock(int blockSize, IEnumerable<Transaction> pendingTransactions, string previousHash)
        {
            BlockSize = blockSize;
            Transactions = new MerkleTree<Transaction>(pendingTransactions.ToArray());
            PreviousHash = previousHash;
            Hash = CalculateHash();
        }

        public void MineBlock(int difficulty) { }

        public static CreateBlockDelegate CreateFactory() =>
            (DateTime created, string previousHash, int blockSize, IEnumerable<Transaction> pendingTransactions) =>
                new MockBlock(blockSize, pendingTransactions.ToList(), previousHash);

    }
}
