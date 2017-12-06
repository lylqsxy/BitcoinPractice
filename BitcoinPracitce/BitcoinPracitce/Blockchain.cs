using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpToyBitcoinBlockchain
{
    class Blockchain
    {
        const int PERIOD = 5;
        const int TARGET_TIME_SPAN = 5000;

        // a chain has many blocks
        private List<Block> _chain;

        private int _difficulty;
        private int _version;
        private int _index = 0;

        public Blockchain()
        {
            this._difficulty = 56155;
            this._version = 2;

            Console.WriteLine("Init Blockchain......");
            this._chain = new List<Block>();
            Console.WriteLine("Creating Genesis Block......");

            AddBlock("Genesis block");
        }

        public Block GetLatestBlock()
        {
            return this._chain.Last();
        }

        public void AddBlock(string data)
        {
            var previousHash = this._chain.Any() ? this.GetLatestBlock().GetHash() : new byte[32];
            this.CalculateDifficulty();
            var newBlock = new Block(_index, _version, previousHash, DateTime.Now, _difficulty, data);
            this._index++;
            this._chain.Add(newBlock);
            newBlock.Print();
        }

        public void CalculateDifficulty()
        {
            var count = this._chain.Count;
            if ( count >= PERIOD + 1 && count % PERIOD == 1)
            {
                var totalTimeSpan = new TimeSpan();
                for (int i = 1; i <= PERIOD; i++)
                {
                    var timeSpan = this._chain[count - i].GetTimestamp() - this._chain[count - i - 1].GetTimestamp();
                    totalTimeSpan += timeSpan;
                }
                var avgTimeSpan = totalTimeSpan.TotalMilliseconds / PERIOD;
                _difficulty = Convert.ToInt32(_difficulty * (TARGET_TIME_SPAN / avgTimeSpan));
            }
           
        }

        public void ValidateChain()
        {

            for (var i = 1; i < this._chain.Count; i++)
            {
                var currentBlock = this._chain[i];
                var previousBlock = this._chain[i - 1];

                // Check if the current block hash is consistent with the hash calculated
                if (!currentBlock.GetHash().SequenceEqual(currentBlock.CalculateHash()))
                {
                    throw new Exception("Chain is not valid! Current hash is incorrect!");
                }

                // Check if the Previous hash match the hash of previous block
                if (!currentBlock.GetPreviousHash().SequenceEqual(previousBlock.GetHash()))
                {
                    throw new Exception("Chain is not valid! PreviousHash isn't pointing to the previous block's hash!");
                }

            }

            Console.WriteLine("The current block hash is consistent with the hash calculated");
            Console.WriteLine("The previous hash match the hash of previous block");

        }


    }
}
