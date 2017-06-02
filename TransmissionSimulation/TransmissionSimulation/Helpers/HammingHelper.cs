using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TransmissionSimulation.Helpers
{
    public static class HammingHelper
    {
        public static BitArray Encrypt(BitArray bitArrayInput)
        {
            int m = bitArrayInput.Length;
            int r = 0;
            int indiceInput = 0;

            // Compute the number of bit of controle
            while (m + r + 1 > Math.Pow(2, r))
                r++;

            BitArray bitArrayOutput = new BitArray(m + r);

            // Fill the bitArrayOutput with the value of the bitArrayInput with spaces for bit of controle (power of 2)
            for (int i = 0; i < bitArrayOutput.Length; i++)
            {
                if (!IsPowerOf2(i + 1)) // i+1 because indices start to 0 instead of 1
                {
                    bitArrayOutput[i] = bitArrayInput[indiceInput];
                    indiceInput++;
                }
            }

            // Fill the bit of controle (power of 2)
            for (int i = 0; i < bitArrayOutput.Length; i++)
            {
                if (IsPowerOf2(i + 1)) // i+1 because indices start to 0 instead of 1
                {
                    int parityCount = 0;
                    Console.WriteLine("Is Power of 2 : {0}", i+1);

                    for (int j = i + 1; j < bitArrayOutput.Length; j++)
                    {
                        if (IsBitSet(j+1, i+1))
                        {
                            Console.WriteLine(" -> Is Bit Set : {0}", j+1);
                            if (bitArrayOutput[j])
                                parityCount++;
                        }
                    }
                    Console.WriteLine("ParityCount {0}", parityCount);
                    // Write 1 for odd and 0 for even
                    bitArrayOutput[i] = (parityCount & 1) == 1;
                    Console.WriteLine("ParityBool {0}", bitArrayOutput[i]);
                }
            }
            Console.Write(BitArrayToDigitString(bitArrayOutput));
            return bitArrayOutput;
        }

        public static BitArray Decrypt(BitArray bitArrayInput)
        {
            BitArray bitArrayOutput = new BitArray(GetDataSize(bitArrayInput.Length));
            
            // Check the bit of controle (power of 2) to verify integrity
            for (int i = 0; i < bitArrayOutput.Length; i++)
            {
                if (IsPowerOf2(i + 1)) // i+1 because indices start to 0 instead of 1
                {
                    //TODO
                }
                    
            }
            return null;
        }

        public static int GetDataSize(int totalSize)
        {
            int r = 0;
            while (totalSize + 1 > Math.Pow(2, r))
                r++;
            return totalSize - r;
        }

        public static bool IsPowerOf2(int i)
        {
            //https://graphics.stanford.edu/~seander/bithacks.html#DetermineIfPowerOf2
            return (i & (i - 1)) == 0;
        }

        private static bool IsBitSet(int j, int i)
        {
            return (j & i) != 0;
        }
        public static string BitArrayToDigitString(BitArray bitArray)
        {
            var builder = new StringBuilder();
            foreach (bool bit in bitArray)
                builder.Append(bit ? "1" : "0");
            return builder.ToString();
        }
    }
}
