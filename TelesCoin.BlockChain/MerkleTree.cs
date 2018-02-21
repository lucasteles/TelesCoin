using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TelesCoin
{
    // kinda of, isn't a real tree, for simplicity, just calculate the MerkleRoot Hash
    public class MerkleTree<T> : IEnumerable<T>, IReadOnlyCollection<T>
    {
        private readonly List<T> Items;
        private List<string> TempHashList;

        public int Count => Items.Count;

        public string GetMarkleRootHash() => CalcRootHash();

        public MerkleTree()
        {
            Items = new List<T>();
        }

        public MerkleTree(IEnumerable<T> items)
        {
            Items = items.ToList();
        }

        public void Add(T item) => Items.Add(item);

        private static string GetSHA256(string data)
        {
            using (var alg = SHA256.Create())
            {
                var hash = alg.ComputeHash(Encoding.ASCII.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        private string CalcRootHash()
        {
            TempHashList = Items.Select(e => GetSHA256(e.ToString())).ToList();
            return CalcRootHashRecursive();
        }

        private string CalcRootHashRecursive()
        {

            var hashs = new List<string>();
            for (int i = 0; i < TempHashList.Count; i += 2)
            {
                var left = TempHashList[i];

                if ((i + 1) >= TempHashList.Count)
                    hashs.Add(GetSHA256(left));
                else
                {
                    var right = TempHashList[i + 1];
                    hashs.Add(GetSHA256(left + right));
                }
            }

            TempHashList = hashs;
            if (TempHashList.Count == 1)
                return TempHashList.First();

            if (TempHashList.Count == 0)
                return string.Empty;

            return CalcRootHashRecursive();

        }

        public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
    }


}
