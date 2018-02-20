using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace TelesCoin.Test
{
    public class BlockTests
    {
        [Fact]
        public void Should_not_create_a_block_with_more_trasanction_then_limit()
        {
            var dateTime = new DateTime(2017, 01, 01);
            const int blockSize = 1;
            var transactions = new[] {
                new Transaction(Guid.Empty,Guid.Empty,0),
                new Transaction(Guid.Empty,Guid.Empty,0),
            };

            Action action = () => new Block(dateTime, blockSize, transactions, string.Empty);
            action.Should().Throw<Exception>();

        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(55)]
        [InlineData(1000)]
        public void Should_create_a_block_with_same_transactions_limit(int limit)
        {
            var dateTime = new DateTime(2017, 01, 01);
            var transactions = Enumerable.Repeat(new Transaction(Guid.Empty, Guid.Empty, 0), limit);

            var block = new Block(dateTime, limit, transactions, string.Empty);

            block.Transactions.Should().BeEquivalentTo(transactions);
        }


        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 10)]
        [InlineData(9, 10)]
        [InlineData(999, 1000)]
        public void Should_create_a_block_with_less_transactions_then_limit(int transactionsCount, int limit)
        {
            var dateTime = new DateTime(2017, 01, 01);
            var transactions = Enumerable.Repeat(new Transaction(Guid.Empty, Guid.Empty, 0), transactionsCount);

            var block = new Block(dateTime, limit, transactions, string.Empty);

            block.Transactions.Should().BeEquivalentTo(transactions);
        }

        [Fact]
        public void Should_calc_a_hash_on_creation()
        {
            var transactions = new[] { new Transaction(Guid.Empty, Guid.Empty, 0) };

            var block = new Block(DateTime.UtcNow, 10, transactions, string.Empty);

            block.Hash.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Should_mine_a_block_with_dificulty(int difficulty)
        {

            var transactions = Enumerable.Range(1, 10)
                                    .Select(t => new Transaction(Guid.NewGuid(), Guid.NewGuid(), 100));

            var block = new Block(DateTime.UtcNow, 10, transactions, string.Empty);
            block.MineBlock(difficulty);

            block.Hash.Should().StartWith(new string('0', difficulty));
        }

    }
}
