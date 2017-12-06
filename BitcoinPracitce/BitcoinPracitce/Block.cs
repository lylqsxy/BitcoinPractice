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
        private byte[] MAX_TARGET_VALUE = { 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
        private int _difficulty;

        private Int32 _index;
        private byte[] _hash = new byte[32];

        private Int32 _version;
        private byte[] _previousHash = new byte[32];
        private Int32 _timestamp;
        private Int32 _nonce = -1;
        private byte[] _bits = new byte[4];
        private byte[] _merkelHash = new byte[32];

        private string _data = "";

        public Block(int index, int version, byte[] previousHash, DateTime timestamp, int difficulty, string data)
        {
            this._version = 2;
            this._index = index;
            this._previousHash = previousHash;
            this._timestamp = Convert.ToInt32((timestamp - new DateTime(1970,1,1,0,0,0)).TotalSeconds);
            this._merkelHash = SHA256_hash(Encoding.UTF8.GetBytes(data));
            this._difficulty = difficulty;
            this._data = data;

            this.Mine(this._difficulty);
        }


        // Create the hash of the current block.
        public byte[] CalculateHash()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(this._version).ToList());
            bytes.AddRange(this._previousHash.ToList());
            bytes.AddRange(this._merkelHash.ToList());
            bytes.AddRange(BitConverter.GetBytes(this._timestamp).ToList());
            bytes.AddRange(this._bits.ToList());
            bytes.AddRange(BitConverter.GetBytes(this._nonce).ToList());

            return SHA256_hash(bytes.ToArray());
        }

        // This is how the mining works!
        public void Mine(int difficulty)
        {
            var maxTarget = new BigInteger(HexToLittleEndianSwap(MAX_TARGET_VALUE));
            var diff = new BigInteger(difficulty);
            var target = BigInteger.Divide(maxTarget, diff);
            Console.WriteLine(string.Format("Target: (Difficulty: {0})", this._difficulty));
            Utility.WriteHex(HexToBigEndianSwap(target.ToByteArray(), 32));
            this._bits = CompBits(HexToBigEndianSwap(target.ToByteArray(), 32));

            var value = new BigInteger();
            do
            {
                this._nonce++;
                this._hash = this.CalculateHash();
                value = new BigInteger(HexToLittleEndianSwap(this._hash));
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

        public Int32 GetTimestamp()
        {
            return this._timestamp;
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

        static byte[] CompBits(byte[] input)
        {
            int i;
            for (i = 0; i < input.Length; i++)
            {
                if (input[i] != 0x00)
                {
                    if (input[i] >= 0x80)
                        i--;
                    break;
                }
            }

            byte byteExp = Convert.ToByte(input.Length - i);
            byte[] output = { byteExp, input[i], input[i + 1], input[i + 2] };

            return output;
        }

        static byte[] HexToBigEndianSwap(byte[] input, int bit)
        {
            var tmp = input.ToList();
            while(tmp.Count < bit)
            {
                tmp.Add(0x00);
            }
            tmp.Reverse();

            return tmp.ToArray();
        }

        static byte[] HexToLittleEndianSwap(byte[] input)
        {
            var tmp = input.ToList();
            tmp.Reverse();
            tmp.Add(0x00);
            return tmp.ToArray();
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
            Utility.WriteHex(this._merkelHash);
            bytes.AddRange(this._merkelHash.ToList());

            Console.WriteLine("\r\nTime Stamp");
            Console.WriteLine(this._timestamp);
            bytes.AddRange(BitConverter.GetBytes(this._timestamp).ToList());

            Console.WriteLine("\r\nBits");
            Utility.WriteHex(this._bits);
            bytes.AddRange(this._bits.ToList());

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
