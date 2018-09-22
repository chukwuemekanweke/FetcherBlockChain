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
    }
}
