using System;
using System.Collections.Generic;
using System.Linq;

namespace TelesCoin
{
    public class BlockChain
    {
        public Guid MinerAdress { get; }
        public IReadOnlyCollection<IBlock> Chain => _chain.ToList();
        public IReadOnlyCollection<Transaction> PenddingTransactions => _peddingTransactions.ToList();

        readonly IList<IBlock> _chain;
        readonly NetworkConsense _consense;
        readonly CreateBlockDelegate _blockFactory;
        Stack<Transaction> _peddingTransactions;

        public BlockChain(Guid miningAdress, NetworkConsense networkConsense, CreateBlockDelegate createBlock = null)
        {
            _consense = networkConsense;
            _blockFactory = createBlock ?? BlockFactory.CreateBlock;
            _peddingTransactions = new Stack<Transaction>();
            MinerAdress = miningAdress;

            _chain = new List<IBlock> { CreateGenesisBlock() };
        }

        private IBlock CreateGenesisBlock()
        {
            var genesisTransaction = new[] { new Transaction(Guid.Empty, MinerAdress, _consense.MiningReward) };
            var block = _blockFactory(new DateTime(2017, 01, 01), "0", 1, genesisTransaction);
            block.MineBlock(_consense.Difficulty);
            return block;
        }

        public void MiningPedingTransactions()
        {
            var transactionsToAdd = new List<Transaction>();
            for (int i = 0; i < _consense.BlockSize; i++)
                if (_peddingTransactions.Any())
                    transactionsToAdd.Add(_peddingTransactions.Pop());

            var lastBlock = Chain.Last();
            var block = _blockFactory(DateTime.UtcNow, lastBlock.Hash, _consense.BlockSize, transactionsToAdd);

            block.MineBlock(_consense.Difficulty);
            Console.WriteLine("Block successfully mined");
            _chain.Add(block);

            CreateTransaction(Transaction.FromNetwork(_consense, MinerAdress));

        }

        public void CreateTransaction(params Transaction[] transactions) =>
            transactions.ForEach(t => _peddingTransactions.Push(t));


        public double GetAddressBalance(Guid adress) =>
            _chain
                .SelectMany(b => b.Transactions)
                .Where(t => t.ToAdress == adress || t.FromAddress == adress)
                .Select(t => new { t.Amount, IsCredit = t.ToAdress == adress })
                .Select(t => t.Amount * (t.IsCredit ? 1 : -1))
                .Sum();

        public bool IsChainValid()
        {
            for (int i = 1; i < _chain.Count; i++)
            {
                var currentBlock = _chain[i];
                var previousBlock = _chain[i - 1];

                if (currentBlock.Hash != currentBlock.CalculateHash()
                || currentBlock.PreviousHash != previousBlock.Hash)
                    return false;

            }

            return true;
        }


    }


}