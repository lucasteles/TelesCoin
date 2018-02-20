using System;
using System.Security.Cryptography;
using System.Text;

namespace TelesCoin
{
    public class Block 
    {
        int index;
        DateTime created;

        int nonce = 0;

        public string Data {get;set;}
        public string PreviousHash {get;set;}
        public string Hash {get;set;} 

        double timestamp => 
            (new DateTimeOffset(this.created))
            .ToUnixTimeSeconds();
        

        public Block(int index, DateTime created, string data, string previousHash = "")
        {
            this.index = index;
            this.created = created;
            this.Data = data;
            this.PreviousHash = previousHash;
            CalculateHash();
        }

        public string CalculateHash()
        {
            using (var algorithm = SHA256.Create())
            {         
              var contentBody = $"{this.index}{this.PreviousHash}{this.timestamp}{Data}{this.nonce}";
              var hash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(contentBody));
              this.Hash = BitConverter.ToString(hash).Replace("-","");
              return this.Hash;
            }
        } 

        public override string ToString() => $"({this.index}) {this.PreviousHash}:{this.Hash}|{this.timestamp} -> {Data}";
        
        public void MineBlock(int difficulty)
        {
            while(this.Hash.Substring(0,difficulty) != new string('0', difficulty))
            {
                this.nonce++;
                this.Hash = CalculateHash();
            }
            Console.WriteLine($"Block mined: {this.Hash}");
        }
        
    }
}