using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Dynamic;
using Microsoft.Extensions.Configuration;
using FetcherP2P;

namespace FetcherBlockchainAPI.APILayer
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlockController : ControllerBase
    {
        BlockChain blockChain { get; set; }
        P2PServer p2pServer { get; set; }
        IConfiguration configuration { get; set; }
        public BlockController(BlockChain _blockChain, P2PServer _p2pServer, IConfiguration _configuration)
        {
            this.blockChain = _blockChain;
            this.p2pServer = _p2pServer;
            this.configuration = _configuration;
          
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