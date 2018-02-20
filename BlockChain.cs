using System;
using System.Collections.Generic;
using System.Linq;

namespace TelesCoin {

    public class BlockChain
    {
        IList<Block> chain;

        public int Difficulty { get; set; }

        public BlockChain(int difficulty)
        {
            Difficulty = difficulty;
            this.chain = new List<Block>() { CreateGenesisBlock() };
        }

        private Block CreateGenesisBlock() {
            var block = new Block(0, new DateTime(2017,01,01),"Genesis block", "0");
            block.MineBlock(this.Difficulty);
            return block;
        }
        public Block GetLatestBlock() => chain.Last();

        public void AddBlock(Block block) {
            block.PreviousHash = GetLatestBlock().Hash;
            block.MineBlock(this.Difficulty);
            chain.Add(block);
        }

        public bool IsChainValid()
        {
            for (int i = 1; i < chain.Count; i++)
            {
                var currentBlock = this.chain[i];
                var previousBlock = this.chain[i-1];

                if(currentBlock.Hash != currentBlock.CalculateHash()
                || currentBlock.PreviousHash != previousBlock.Hash )
                    return false;

            }

            return true;
        }

        public IReadOnlyCollection<Block> GetChain() => chain.ToList();

    }


}