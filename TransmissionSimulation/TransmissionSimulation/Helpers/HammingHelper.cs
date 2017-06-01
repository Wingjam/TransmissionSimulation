using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransmissionSimulation.Helpers
{
    class HammingHelper
    {
        public BitArray Encrypt(BitArray bitArrayInput)
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
            return bitArrayOutput;
        }

        public BitArray Decrypt(BitArray bitArrayOutput)
        {
            return null;
        }

        public int GetDataSize(int totalSize)
        {
            int r = 0;
            while (totalSize + 1 > Math.Pow(2, r))
                r++;
            return totalSize - r;
        }

        private bool IsPowerOf2(int i)
        {
            //https://graphics.stanford.edu/~seander/bithacks.html#DetermineIfPowerOf2
            return (i & (i - 1)) == 0;
        }
    }
}
