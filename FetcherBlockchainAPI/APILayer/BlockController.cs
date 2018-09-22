using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Dynamic;

namespace FetcherBlockchainAPI.APILayer
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlockController : ControllerBase
    {
        BlockChain blockChain { get; set; }
        public BlockController(BlockChain _blockChain)
        {
            this.blockChain = _blockChain;
        }

        [Route("")]
        public IActionResult Get()
        {
            return Ok(blockChain.Chain);
        }


        [Route("")]
        [HttpPost]
        public IActionResult Create(object data)
        {
            try
            {
                blockChain.AddBlock(data);
                return RedirectToAction("Get", "Block");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}