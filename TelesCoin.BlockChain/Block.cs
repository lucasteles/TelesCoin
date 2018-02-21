using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TelesCoin
{
    public class Block : IBlock
    {
        public string PreviousHash { get; }
        public string Hash { get; private set; }
        public int BlockSize { get; }

        public string MerkleRootHash { get; set; }

        public MerkleTree<Transaction> Transactions { get; }

        DateTime Created;
        int nonce;

        double Timestamp =>
            (new DateTimeOffset(Created)).ToUnixTimeSeconds();


        public Block(DateTime created, int blockSize, IEnumerable<Transaction> pendingTransactions, string previousHash)
        {
            Created = created;
            BlockSize = blockSize;
            Transactions = new MerkleTree<Transaction>(pendingTransactions.ToArray());
            PreviousHash = previousHash;

            if (Transactions.Count > blockSize)
                throw new Exception("Invalid transaction size");

            MerkleRootHash = Transactions.GetMarkleRootHash();
            CalculateHash();
        }

        public string CalculateHash()
        {
            using (var algorithm = SHA256.Create())
            {
                var contentBody = $"{PreviousHash}{Timestamp}{MerkleRootHash}{nonce}";
                var hash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(contentBody));
                Hash = BitConverter.ToString(hash).Replace("-", "");
                return Hash;
            }
        }

        public override string ToString() => $"{PreviousHash}:{Hash}|{Timestamp} -> {MerkleRootHash}";

        public void MineBlock(int difficulty)
        {
            //proof of work
            while (Hash.Substring(0, difficulty) != new string('0', difficulty))
            {
                nonce++;
                Hash = CalculateHash();
            }
            Console.WriteLine($"Block mined: {Hash}");
        }


    }
}