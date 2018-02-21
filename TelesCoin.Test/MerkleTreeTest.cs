using FluentAssertions;
using Xunit;

namespace TelesCoin.Test
{
    public class MerkleTreeTest
    {

        private class Teste
        {
            public string Value { get; set; }
            public Teste(string value) => Value = value;
            public override string ToString() => Value;
        }


        [Fact]
        public void Should_generate_hash_even()
        {
            var tree = new MerkleTree<Teste>
            {
                new Teste("a"),
                new Teste("b"),
                new Teste("c"),
                new Teste("d")
            };

            var rootHash = tree.GetMarkleRootHash();
            rootHash.Should().Be("58c89d709329eb37285837b042ab6ff72c7c8f74de0446b091b6a0131c102cfd");

        }

        [Fact]
        public void Should_generate_hash_odd()
        {
            var tree = new MerkleTree<Teste>
            {
                new Teste("a"),
                new Teste("b"),
                new Teste("c"),
                new Teste("d"),
                new Teste("e")
            };

            var rootHash = tree.GetMarkleRootHash();
            rootHash.Should().Be("d6246621103b5050cf32df614c5017e91853d47a19fe5d3e7c68a8f4588f5b66");

        }

    }
}
