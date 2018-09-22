using System;
using System.Security.Cryptography;
using System.Text;

namespace Models
{
    public class Block
    {
        public long TimeStamp { get; set; }
        public string LastHash { get; set; }
        public string Hash { get; set; }
        public object Data { get; set; }
        public Block(long timestamp, string lastHash, string hash, object data)
        {
            TimeStamp = timestamp;
            Hash = hash;
            LastHash = lastHash;
            Data = data;
        }

        public override string ToString()
        {
            var lastHash = this.LastHash;
            var hash = this.Hash;

            if (hash.Length > 10)
            {
                hash = hash.Substring(0, 10);
            }

            if (lastHash.Length > 10)
            {
                lastHash = lastHash.Substring(0, 10);
            }

            return $@"  Block -                     
                        Timestamp: {TimeStamp}
                        LastHash: {LastHash}
                        Hash    : {Hash}
                        Data    :{Data}                      
                        ";
        }

        internal static string BlockHash(Block currentBlock)
        {
            return ComputeHash(currentBlock.TimeStamp, currentBlock.LastHash, currentBlock.Data);
        }

        public override bool Equals(object obj)
        {
            Block block = (Block)obj;
            return this.TimeStamp == block.TimeStamp && this.Hash == block.Hash && this.LastHash == block.LastHash && this.Data == block.Data;
        }

        public static Block Genesis()
        {
            var timestamp = GenerateTimestamp();

            return new Block(0, "-----", "f1r57-h45h", "Genesis Block");
        }

        public static Block MineBlock(Block lastBlock, object data)
        {
            var timestamp = GenerateTimestamp();
            var lastHash = lastBlock.Hash;
            var hash = ComputeHash(timestamp, lastHash, data);
            return new Block(timestamp, lastHash, hash, data);

        }

        public static string ComputeHash(long timeStamp, string lastHash, object data)
        {
            SHA256 sha = SHA256.Create();
            StringBuilder builder = new StringBuilder();

            byte[] sha256Bytes = sha.ComputeHash(Encoding.UTF8.GetBytes($"{timeStamp}{lastHash}{data}"));
            for (int i = 0; i < sha256Bytes.Length; i++)
            {
                builder.Append(sha256Bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public static long GenerateTimestamp()
        {
            var timestamp = (DateTime.Now - new DateTime(1970, 01, 01)).TotalMilliseconds;

            timestamp = Math.Ceiling(timestamp);

            return Convert.ToInt64(timestamp);

        }










    }
}
