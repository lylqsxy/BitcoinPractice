using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToyBitcoinBlockchain
{
    static class Utility
    {
        public static void WriteHex(byte[] input)
        {
            Console.WriteLine(BitConverter.ToString(input));
        }

        public static void WriteHex(dynamic input)
        {
            Console.WriteLine(BitConverter.ToString(BitConverter.GetBytes(input)));
        }


    }
}
