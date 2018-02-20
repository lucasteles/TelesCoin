using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TelesCoin.Test
{
    public class BlockChainTests
    {
        [Fact]
        public void Should_create_blockchain_with_genesis_block()
        {
            var bc = new BlockChain(Guid.NewGuid(), new NetworkConsense(1, 5, 0));
            bc.Chain.Should().HaveCount(1);
        }

        [Fact]
        public void Should_add_transaction()
        {
            var bc = new BlockChain(Guid.NewGuid(), new NetworkConsense(1, 5, 0));
            bc.CreateTransaction(new Transaction(Guid.NewGuid(), Guid.NewGuid(), 10));
            bc.PenddingTransactions.Should().HaveCount(1);
        }


        [Theory]
        [InlineData(10, 10, 0)]
        [InlineData(10, 5, 0)]
        [InlineData(5, 10, 5)]
        public void Should_remove_transactions_whens_mines(int blockSize, int transactionCount, int remains)
        {

            var factory = MockBlock.CreateFactory();
            var bc = new BlockChain(Guid.NewGuid(), new NetworkConsense(1, blockSize, 0), factory);
            const int transactionRewardCount = 1;

            var transactions =
                Enumerable.Repeat(new Transaction(Guid.NewGuid(), Guid.NewGuid(), 1), transactionCount)
                .ToArray();

            bc.CreateTransaction(transactions);
            bc.MiningPedingTransactions();
            bc.PenddingTransactions.Should().HaveCount(remains + transactionRewardCount);

        }


        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(100)]
        public void Should_natural_transactions_be_valid(int transactionCount)
        {
            const int blockSize = 5;
            var consense = new NetworkConsense(1, blockSize, 0);
            var bc = new BlockChain(Guid.NewGuid(), consense);

            var transactions =
                Enumerable.Repeat(new Transaction(Guid.NewGuid(), Guid.NewGuid(), 1), transactionCount)
                .ToArray();

            bc.CreateTransaction(transactions);

            while (bc.PenddingTransactions.Count > 1)
                bc.MiningPedingTransactions();

            bc.IsChainValid().Should().BeTrue();

        }

        [Theory()]
        [InlineData(true)]
        [InlineData(false)]
        public void Should_validate_a_chain(bool validChain)
        {
            const int size = 5;
            var consense = new NetworkConsense(1, size, 0);


            var blockGenesis = new MockBlock(size, Enumerable.Empty<Transaction>(), string.Empty);
            var block1 = new MockBlock(size, Enumerable.Empty<Transaction>(), string.Empty);
            var block2 = new MockBlock(size, Enumerable.Empty<Transaction>(), string.Empty);
            var block3 = new MockBlock(size, Enumerable.Empty<Transaction>(), string.Empty);

            IEnumerable<IBlock> GetNext(string previosHash)
            {
                blockGenesis.PreviousHash = previosHash;
                yield return blockGenesis;

                block1.PreviousHash = blockGenesis.Hash;
                yield return block1;

                block2.PreviousHash = validChain ? block1.Hash : "INCORRECT";
                yield return block2;

                block3.PreviousHash = block2.Hash;
                yield return block3;

            };

            IEnumerator<IBlock> factoryState = null;
            IBlock factory(DateTime created, string previousHash, int blockSize, IEnumerable<Transaction> pendingTransactions)
            {
                if (factoryState == null)
                    factoryState = GetNext(previousHash).GetEnumerator();

                factoryState.MoveNext();
                return factoryState.Current;
            }

            var bc = new BlockChain(Guid.NewGuid(), consense, factory);
            var transactions =
                Enumerable
                .Repeat(new Transaction(Guid.NewGuid(), Guid.NewGuid(), 1), size * 2)
                .ToArray();

            bc.CreateTransaction(transactions);

            while (bc.PenddingTransactions.Count > 1)
                bc.MiningPedingTransactions();

            if (validChain) // lazy test :<
                bc.IsChainValid().Should().BeTrue();
            else
                bc.IsChainValid().Should().BeFalse();

        }

        [Fact]
        public void Should_add_fi_for_a_founded_block()
        {
            var minerAddress = Guid.NewGuid();

            var factory = MockBlock.CreateFactory();
            var bc = new BlockChain(minerAddress, new NetworkConsense(1, 5, 42), factory);

            var transaction =
                new Transaction(Guid.NewGuid(), Guid.NewGuid(), 1);

            bc.CreateTransaction(transaction);
            bc.MiningPedingTransactions();

            bc.PenddingTransactions.Should().HaveCount(1);
            bc.PenddingTransactions.First().ToAdress.Should().Be(minerAddress);
            bc.PenddingTransactions.First().Amount.Should().Be(42);
        }


        [Fact]
        public void Should_calc_correct_address_balance()
        {
            var address1 = Guid.NewGuid();
            var address2 = Guid.NewGuid();

            const int reward = 50;

            var factory = MockBlock.CreateFactory();
            var bc = new BlockChain(address1, new NetworkConsense(1, 5, reward), factory);

            // 50 0
            bc.CreateTransaction(new Transaction(address1, address2, 20)); // 30 20
            bc.CreateTransaction(new Transaction(address2, address1, 10)); // 40 10
            bc.CreateTransaction(new Transaction(address1, address2, 5));  // 35 15

            bc.MiningPedingTransactions();

            var address1balance = bc.GetAddressBalance(address1);
            var address2balance = bc.GetAddressBalance(address2);

            address1balance.Should().Be(35);
            address2balance.Should().Be(15);

        }
    }
}
