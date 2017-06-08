using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace TransmissionSimulation.Helpers
{
    public static class HammingHelper
    {
        public enum Mode
        {
            CORRECT,
            DETECT,
        }
        public enum ReturnType
        {
            OK,
            CORRECTED,
            DETECTED,
        }
        public static Tuple<BitArray, ReturnType> EncryptManager(BitArray bitArrayInput, Mode mode, int padding = 0)
        {
            Tuple<BitArray, ReturnType> tuple = HammingManager(bitArrayInput, mode, 8, GetTotalSize, Encrypt);
            return Tuple.Create(ResizeBitArray(tuple.Item1, tuple.Item1.Length + padding), tuple.Item2);
        }

        public static Tuple<BitArray, ReturnType> DecryptManager(BitArray bitArrayInput, Mode mode, int padding = 0)
        {
            BitArray bitArrayOutput = ResizeBitArray(bitArrayInput, bitArrayInput.Length - padding);
            return HammingManager(bitArrayOutput, mode, GetTotalSize(8), GetDataSize, Decrypt);
        }

        private static Tuple<BitArray, ReturnType> HammingManager(BitArray bitArrayInput, Mode mode, int magicNumber, Func<int, int> outputSize, Func<BitArray, Mode, Tuple<BitArray, ReturnType>> hamming)
        {
            ReturnType returnType = ReturnType.OK;
            if (bitArrayInput.Length % magicNumber != 0)
                throw new ArgumentException("BitArray size must be multiple of " + magicNumber);

            Boolean[] arrayOuput = new Boolean[outputSize(bitArrayInput.Length)];

            for (int i = 0; i < bitArrayInput.Length / magicNumber; i++)
            {
                // Create a new BitArray of size magicNumber
                BitArray subBitsArray = ResizeBitArray(bitArrayInput, magicNumber, i * magicNumber);

                // Encrypt/Decrypt it
                Tuple<BitArray, ReturnType> tuple = hamming(subBitsArray, mode);

                // Get the returnType
                if (tuple.Item2 > returnType)
                    returnType = tuple.Item2;

                // Add the output (part i) to the arrayOutput
                tuple.Item1.CopyTo(arrayOuput, i * tuple.Item1.Length);
            }

            return Tuple.Create(new BitArray(arrayOuput), returnType);
        }

        private static Tuple<BitArray, ReturnType> Encrypt(BitArray bitArrayInput, Mode mode)
        {
            int indiceInput = 0;

            BitArray bitArrayOutput = new BitArray(GetTotalSize(bitArrayInput.Length));

            // Fill the bitArrayOutput with the value of the bitArrayInput with spaces for bit of controle (power of 2)
            for (int i = 0; i < bitArrayOutput.Length; i++)
            {
                if (!IsPowerOf2(i))
                {
                    bitArrayOutput[i] = bitArrayInput[indiceInput];
                    indiceInput++;
                }
            }

            int masterParityCount = 0;
            // Fill the bit of controle (power of 2)
            for (int i = 1; i < bitArrayOutput.Length; i++)
            {
                // index 0 is reserved to the master bit of control
                if (IsPowerOf2(i))
                {
                    int parityCount = GetParityCount(i, bitArrayOutput);

                    // Write 1 for odd and 0 for even
                    bitArrayOutput[i] = (parityCount & 1) == 1;
                }

                if (bitArrayOutput[i])
                    masterParityCount++;
            }

            // Fill the master bit of controle
            // Write 1 for odd and 0 for even
            bitArrayOutput[0] = (masterParityCount & 1) == 1;
            
            return Tuple.Create(bitArrayOutput, ReturnType.OK);
        }

        private static Tuple<BitArray, ReturnType> Decrypt(BitArray bitArrayInput, Mode mode)
        {
            int errorSyndrome = 0;
            int masterParityCount = 0;

            // Check the bit of controle (power of 2) to verify integrity
            for (int i = 1; i < bitArrayInput.Length; i++)
            {
                //Except master bit of controle (index 0)
                if (IsPowerOf2(i))
                {
                    int parityCount = GetParityCount(i, bitArrayInput);

                    //If parityCount is odd
                    if ((parityCount & 1) == 1)
                    {
                        errorSyndrome += i;
                    }
                }

                if (bitArrayInput[i])
                    masterParityCount++;
            }

            //masterParityCount : 1 for odd and 0 for even
            bool isMasterParityCorrect = ((masterParityCount & 1) == 1) == bitArrayInput[0];
            ReturnType returnType = ReturnType.OK;

            //DETECT (all mode)
            // 2 bit detection -> there is an errorSyndrome and masterParityCount is correct
            if (errorSyndrome != 0 && isMasterParityCorrect)
            {
                returnType = ReturnType.DETECTED;
            }

            // CORRECT
            // 1 bit error -> there is an errorSyndrome and masterParityCount is wrong
            else if (errorSyndrome != 0 && !isMasterParityCorrect)
            {
                if (mode == Mode.CORRECT)
                {
                    // The error to correct is at errorSyndrome index
                    // If the errorSyndrome is valid, correct the value in question
                    if (errorSyndrome < bitArrayInput.Length)
                    {
                        bitArrayInput[errorSyndrome] = !bitArrayInput[errorSyndrome];
                        returnType = ReturnType.CORRECTED;
                    }
                    // Else, there is 3 errors or more that we can detect in the bitArray, don't correct it
                    else
                    {
                        returnType = ReturnType.DETECTED;
                    }
                }
            }
            //Assumption: The probability of >= 3 bits being in error negligible.

            BitArray bitArrayOutput = new BitArray(GetDataSize(bitArrayInput.Length));
            int indiceOutput = 0;

            // Fill the bitArrayOutput without the bit of controle (power of 2), only data
            for (int i = 1; i < bitArrayInput.Length; i++)
            {
                if (!IsPowerOf2(i))
                {
                    bitArrayOutput[indiceOutput] = bitArrayInput[i];
                    indiceOutput++;
                }
            }

            return Tuple.Create(bitArrayOutput, returnType);
        }

        /// <summary>
        /// Get the maximum data size (m)
        /// </summary>
        /// <param name="totalSize"></param>
        /// <returns></returns>
        public static int GetDataSize(int totalSize)
        {
            return totalSize / 13 * 8;
            if (totalSize == 8)
                return 4;
            if (totalSize == 12)
                return 7;
            if (totalSize == 13)
                return 8;

            // Insert Magic Here
            int r = 0;
            while (totalSize % 13 + 1 > Math.Pow(2, r))
                r++;

            // Return Magic Here
            if (r == 0)
                return totalSize - totalSize / 13 * 5;
            return totalSize - (totalSize / 13 * 5 + r + 1);
        }

        public static int GetTotalSize(int dataSize)
        {
            return dataSize / 8 * 13;
            int m = dataSize;
            int r = 0;

            // Compute the number of bit of controle
            while (m + r + 1 > Math.Pow(2, r))
                r++;

            // Add 1 because of the SECDED (master bit of controle at index 0)
            r++;

            return m + r;
        }

        public static bool IsPowerOf2(int i)
        {
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

        private static int GetParityCount(int i, BitArray bitArray)
        {
            int parityCount = 0;
            // For each number where j is in decomposition of power of 2 (Tanenbaum book p. 222)
            // e.g. 11 = 8 + 2 + 1
            // e.g. 29 = 16 + 8 + 4 + 1
            for (int j = i; j < bitArray.Length; j++)
            {
                if (IsBitSet(j, i))
                {
                    if (bitArray[j])
                        parityCount++;
                }
            }

            return parityCount;
        }

        private static BitArray ResizeBitArray(BitArray bitArray, int newArraySize, int startIndex = 0)
        {
            Boolean[] outputBits = new Boolean[newArraySize];
            // Same Array
            if (bitArray.Length == newArraySize)
                return bitArray;

            // Bigger Array
            if (bitArray.Length < newArraySize)
                bitArray.CopyTo(outputBits, 0);

            //Smaller Array
            else
            {
                Boolean[] allBits = new Boolean[bitArray.Length];
                bitArray.CopyTo(allBits, 0);
                Array.Copy(allBits, startIndex, outputBits, 0, newArraySize);
            }

            return new BitArray(outputBits);
        }
    }
}
