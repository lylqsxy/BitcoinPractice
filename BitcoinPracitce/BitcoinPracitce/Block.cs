using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace CSharpToyBitcoinBlockchain
{
    class Block
    {
        private byte[] MAX_TARGET_VALUE = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
        private DateTime _time;

        private Int32 _index;
        private byte[] _hash = new byte[32];

        private Int32 _version;
        private byte[] _previousHash = new byte[32];
        private Int32 _timestamp;
        private Int32 _nonce = -1;
        private Int32 _difficulty;
        private byte[] _merkel = new byte[32];

        private string _data = "";



        public Block(int index, int version, byte[] previousHash, DateTime timestamp, int difficulty, string data)
        {
            this._version = 2;
            this._index = index;
            this._previousHash = previousHash;
            this._timestamp = Int32.Parse(timestamp.ToString("MMddHHmmss"));
            this._merkel = SHA256_hash(Encoding.UTF8.GetBytes(data));
            this._difficulty = difficulty;
            this._data = data;

            this._time = timestamp;

            this.Mine(this._difficulty);
        }


        // Create the hash of the current block.
        public byte[] CalculateHash()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(this._version).ToList());
            bytes.AddRange(this._previousHash.ToList());
            bytes.AddRange(this._merkel.ToList());
            bytes.AddRange(BitConverter.GetBytes(this._timestamp).ToList());
            bytes.AddRange(BitConverter.GetBytes(this._difficulty).ToList());
            bytes.AddRange(BitConverter.GetBytes(this._nonce).ToList());

            return SHA256_hash(bytes.ToArray());
        }

        // This is how the mining works!
        public void Mine(int difficulty)
        {
            // You mined successfully if you found a hash with certain number of leading '0's
            // difficulty defines the number of '0's required 
            // e.g. if difficulty is 2, if you found a hash like  00338500000x..., it means you mined it.
            var maxTarget = new BigInteger(MAX_TARGET_VALUE.Concat(new byte[] { 0 }).ToArray());
            var diff = new BigInteger(difficulty);
            var target = BigInteger.Divide(maxTarget, diff);
            var value = new BigInteger();
            do
            {
                this._nonce++;
                this._hash = this.CalculateHash();
                value = new BigInteger(this._hash.Concat(new byte[] { 0 }).ToArray());
                //Console.WriteLine("Mining: " + BitConverter.ToString(this._hash));
            }
            while (BigInteger.Compare(value, target) > 0);
            Console.WriteLine("Block has been mined: ");
            Utility.WriteHex(this._hash);
        }

        public byte[] GetHash()
        {
            return this._hash;
        }

        public byte[] GetPreviousHash()
        {
            return this._previousHash;
        }

        public DateTime GetTimestamp()
        {
            return this._time;
        }

        // Create a hash string from stirng
        static byte[] SHA256_hash(byte[] value)
        {
            byte[] result = { };

            using (SHA256 hash = SHA256Managed.Create())
            {
                result = hash.ComputeHash(value);
            }

            return result;
        }

        public void Print()
        {
            var bytes = new List<byte>();

            Console.WriteLine("\r\n-------Here is the new block to be added--------");
            Console.WriteLine("\r\nIndex");
            Console.WriteLine(this._index);

            Console.WriteLine("\r\nHash");
            Utility.WriteHex(this._hash);

            Console.WriteLine("\r\nVersion");
            Console.WriteLine(this._version);
            bytes.AddRange(BitConverter.GetBytes(this._version).ToList());

            Console.WriteLine("\r\nPrevious Hash");
            Utility.WriteHex(this._previousHash);
            bytes.AddRange(this._previousHash.ToList());

            Console.WriteLine("\r\nMerkel Root");
            Utility.WriteHex(this._merkel);
            bytes.AddRange(this._merkel.ToList());

            Console.WriteLine("\r\nTime Stamp");
            Console.WriteLine(this._timestamp);
            bytes.AddRange(BitConverter.GetBytes(this._timestamp).ToList());

            Console.WriteLine("\r\nDifficulty");
            Console.WriteLine(this._difficulty);
            bytes.AddRange(BitConverter.GetBytes(this._difficulty).ToList());

            Console.WriteLine("\r\nNonce");
            Console.WriteLine(this._nonce);
            bytes.AddRange(BitConverter.GetBytes(this._nonce).ToList());

            Console.WriteLine("\r\nData");
            Console.WriteLine(this._data);

            Console.WriteLine("\r\n80 Bytes Block Head");
            Utility.WriteHex(bytes.ToArray());
            Console.WriteLine("\r\n---------End----------");
        }
    }

}
