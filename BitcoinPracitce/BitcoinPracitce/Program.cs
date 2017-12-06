using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpToyBitcoinBlockchain
{
    class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();

            var myCoinBlockchain = new Blockchain();

            // Received a block from the P2P network.
            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                Console.WriteLine("\r\n==============================\r\n");
                Console.WriteLine("Mining a block...");
                myCoinBlockchain.AddBlock(string.Format("Transfer {0} Bitcoins", random.Next(0, 100)));

                // Validate the chain
                myCoinBlockchain.ValidateChain();
                Thread.Sleep(2000);
            }

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }


}
