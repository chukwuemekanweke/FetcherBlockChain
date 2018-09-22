using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class BlockChain
    {
        public List<Block> Chain { get; set; }
        public BlockChain()
        {
            Chain = new List<Block>();
            Chain.Add(Block.Genesis());
        }

        public void AddBlock(object data)
        {
            Block lastBlock = Chain.LastOrDefault();
            Block block = Block.MineBlock(lastBlock, data);
            Chain.Add(block);
        }

        public bool IsValidChain(List<Block> chain)
        {
            var genesisBlock = Block.Genesis();
            if (!genesisBlock.Equals(chain.FirstOrDefault()))
                return false;

            for (int i = 1; i < chain.Count; i++)
            {
                var currentBlock = chain[i];
                var lastBlock = chain[i - 1];

                if (currentBlock.LastHash != lastBlock.Hash ||
                    currentBlock.Hash != Block.BlockHash(currentBlock))
                    return false;


            }

            return true;
        }
    }
}
