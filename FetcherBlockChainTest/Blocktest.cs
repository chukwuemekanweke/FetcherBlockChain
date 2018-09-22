using Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace FetcherBlockChainTest
{
    public class BlockTest
    {

        Block block;
        Block lastBlock;
        string data;

        public BlockTest()
        {
            data = "bar";
            lastBlock = Block.Genesis();
            block = Block.MineBlock(lastBlock, data);
        }

        [Fact]
        public void SetsDatatoMatchInputTest()
        {
            Assert.Equal(data, block.Data);
        }

        [Fact]
        public void SetsTheLastHashToMatchTheHashOfTheLastBlock()
        {

            Assert.Equal(block.LastHash, lastBlock.Hash);
        }
    }
}
