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

            HttpClient client = new HttpClient();
            string url = "api/visits/v1";
            client.BaseAddress = new Uri("https://my.incapsula.com");

            client.DefaultRequestHeaders.Clear();

            client.DefaultRequestHeaders.Add("Accept", "application/json");


            var dict = new Dictionary<string, string>();


            dict.Add("api_id", "28984");
            dict.Add("api_key", "a609d060-9184-414c-8c35-b23b9cdb2c0e");
            dict.Add("site_id", "1819804");
            var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(dict) };
            HttpResponseMessage responseMessage = client.SendAsync(req).GetAwaiter().GetResult();
            string response = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var statusCode = responseMessage.StatusCode;
            Assert.Equal(block.LastHash, lastBlock.Hash);
        }
    }
}
