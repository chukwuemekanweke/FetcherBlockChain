
using Models;
using System;

namespace FetcherBlockChain
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine();

            //var block = new Block("Foo", "bar", "zoo", "baz");
            //Console.WriteLine(block.ToString());
            //Console.WriteLine();
            //Console.WriteLine(Block.genesis().ToString());

            var fooBlock = Block.MineBlock(Block.Genesis(), "foo");
            Console.WriteLine(fooBlock);

            Console.ReadLine();
        }
    }
}
