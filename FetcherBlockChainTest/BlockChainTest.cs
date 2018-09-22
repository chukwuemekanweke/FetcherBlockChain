using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace FetcherBlockChainTest
{
    public class BlockChainTest
    {
        BlockChain blockChain;
        BlockChain blockChain2;
        public BlockChainTest()
        {

        }

        [Fact]
        public void BlockchainStartsWithGenesisBlockTest()
        {
            blockChain = new BlockChain();
            var genesisBlock = Block.Genesis();
            Assert.Equal(genesisBlock, blockChain.Chain.FirstOrDefault());
        }

        [Fact]
        public void voidAddsANewBlockTest()
        {
            blockChain = new BlockChain();
            var data = "foo";
            blockChain.AddBlock(data);

            Assert.Equal(data, blockChain.Chain.LastOrDefault().Data);

        }

        [Fact]
        public void ValidatesAValidChainTest()
        {
            blockChain = new BlockChain();
            blockChain2 = new BlockChain();

            blockChain2.AddBlock("foo");

            Assert.True(blockChain.IsValidChain(blockChain2.Chain));

        }

        [Fact]
        public void InvalidatesChainWithACorruptGenesisBlock()
        {
            blockChain = new BlockChain();
            blockChain2 = new BlockChain();
            blockChain2.Chain.FirstOrDefault().Data = "Bad Data";
            Assert.False(blockChain.IsValidChain(blockChain2.Chain));
        }

        [Fact]
        public void InvalidatesACorruptChain()
        {
            blockChain = new BlockChain();
            blockChain2 = new BlockChain();

            blockChain2.AddBlock("foo");
            blockChain2.Chain.LastOrDefault().Data = "Not Foo";

            Assert.False(blockChain.IsValidChain(blockChain2.Chain));
        }


        [Fact]
        public void ItReplacesTheChainWithAValidChainTest()
        {
            blockChain = new BlockChain();
            blockChain2 = new BlockChain();

            blockChain2.AddBlock("goo");

            blockChain.ReplaceChain(blockChain2.Chain);

            Assert.Equal(blockChain2.Chain.Count, blockChain.Chain.Count);

        }

        [Fact]
        public void ItDoesNotReplaceACahinWithLengththatIsEqualOrLessThanItSelf()
        {
            blockChain = new BlockChain();
            blockChain2 = new BlockChain();

            blockChain.AddBlock("goo");

            blockChain.ReplaceChain(blockChain2.Chain);

            Assert.NotEqual(blockChain2.Chain.Count, blockChain.Chain.Count);
        }

    }
}
