using System;
using System.Linq;

namespace TelesCoin
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var bc = new BlockChain(5);
            bc.AddBlock(new Block(1, new DateTime(2017,01,10), "teste 1"));
            bc.AddBlock(new Block(2, new DateTime(2017,01,15), "teste 2"));

            
            PrintChain(bc);

            Console.WriteLine("Is valid? -> {0}", bc.IsChainValid());

            var tampedBlock = bc.GetChain().ToArray()[1];
            tampedBlock.Data = "altered";
            tampedBlock.CalculateHash();

            Console.WriteLine("Is valid? -> {0}", bc.IsChainValid());
        }


        static void PrintChain(BlockChain blockchain)
        {
            var chain =  blockchain.GetChain();
            foreach (var item in chain)
                Console.WriteLine(item);
        } 
    }
}
