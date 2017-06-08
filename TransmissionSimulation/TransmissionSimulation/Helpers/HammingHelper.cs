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
        const int HammingDataSplitNumber = 8;
        public enum Mode
        {
            CORRECT,
            DETECT,
        }

        public enum Status
        {
            OK,
            CORRECTED,
            DETECTED,
        }

        /// <summary>
        /// Public encrypt manager with padding management
        /// </summary>
        /// <param name="bitArrayInput"></param>
        /// <param name="mode"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        public static Tuple<BitArray, Status> EncryptManager(BitArray bitArrayInput, Mode mode, int padding = 0)
        {
            Tuple<BitArray, Status> tuple = HammingManager(bitArrayInput, mode, HammingDataSplitNumber, GetTotalSize, Encrypt);
            return Tuple.Create(ResizeBitArray(tuple.Item1, tuple.Item1.Length + padding), tuple.Item2);
        }

        /// <summary>
        /// Public decrypt manager with padding management
        /// </summary>
        /// <param name="bitArrayInput"></param>
        /// <param name="mode"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        public static Tuple<BitArray, Status> DecryptManager(BitArray bitArrayInput, Mode mode, int padding = 0)
        {
            BitArray bitArrayOutput = ResizeBitArray(bitArrayInput, bitArrayInput.Length - padding);
            return HammingManager(bitArrayOutput, mode, GetTotalSize(HammingDataSplitNumber), GetDataSize, Decrypt);
        }

        /// <summary>
        /// Hamming Manager.
        /// </summary>
        /// <param name="bitArrayInput"></param>
        /// <param name="mode"></param>
        /// <param name="splitNumber"></param>
        /// <param name="outputSize"></param>
        /// <param name="hamming"></param>
        /// <returns></returns>
        private static Tuple<BitArray, Status> HammingManager(BitArray bitArrayInput, Mode mode, int splitNumber, Func<int, int> outputSize, Func<BitArray, Mode, Tuple<BitArray, Status>> hamming)
        {
            Status status = Status.OK;
            if (bitArrayInput.Length % splitNumber != 0)
                throw new ArgumentException("BitArray size must be multiple of " + splitNumber);

            Boolean[] arrayOuput = new Boolean[outputSize(bitArrayInput.Length)];

            for (int i = 0; i < bitArrayInput.Length / splitNumber; i++)
            {
                // Create a new BitArray of size (part i) * splitNumber
                BitArray subBitsArray = ResizeBitArray(bitArrayInput, splitNumber, i * splitNumber);

                // Encrypt/Decrypt it
                Tuple<BitArray, Status> tuple = hamming(subBitsArray, mode);

                // Get the higher status
                if (tuple.Item2 > status)
                    status = tuple.Item2;

                // Add the output (part i) to the arrayOutput
                tuple.Item1.CopyTo(arrayOuput, i * tuple.Item1.Length);
            }

            return Tuple.Create(new BitArray(arrayOuput), status);
        }

        /// <summary>
        /// Encrypt the data bits with Hamming Code.
        /// Each power of 2 is a bit of controle (parity)
        /// The first bit (index 0) is the master bit of controle (overall parity)
        /// </summary>
        /// <param name="bitArrayInput"></param>
        /// <param name="mode"></param>
        /// <returns>New encoded BitArray with a bigger size (data + bit of controle) and an OK status</returns>
        private static Tuple<BitArray, Status> Encrypt(BitArray bitArrayInput, Mode mode)
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
            
            return Tuple.Create(bitArrayOutput, Status.OK);
        }

        /// <summary>
        /// Decrypt the bits encrypt by the hamming code. 
        /// According to the mode, the decrypt function can CORRECT 1 bit error or DETECT 2 bits error.
        /// Sometime, it can detect 3 or more error bits when trying to CORRECT.
        /// </summary>
        /// <param name="bitArrayInput"></param>
        /// <param name="mode"></param>
        /// <returns>New decoded BitArray with a smaller size (only data) and a status (OK, CORRECTED or DETECTED)</returns>
        private static Tuple<BitArray, Status> Decrypt(BitArray bitArrayInput, Mode mode)
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
            Status status = Status.OK;

            //DETECT (all mode)
            // 2 bit detection -> there is an errorSyndrome and masterParityCount is correct
            if (errorSyndrome != 0 && isMasterParityCorrect)
            {
                status = Status.DETECTED;
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
                        status = Status.CORRECTED;
                    }
                    // Else, there is 3 errors or more that we can detect in the bitArray, don't correct it
                    else
                    {
                        status = Status.DETECTED;
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

            return Tuple.Create(bitArrayOutput, status);
        }

        /// <summary>
        /// Get the maximum DATA size (in bits) used by the Hamming Code for a certain TOTAL size (in bits).
        /// </summary>
        /// <param name="totalSize"></param>
        /// <returns></returns>
        public static int GetDataSize(int totalSize)
        {
            return totalSize / GetTotalSize(HammingDataSplitNumber) * HammingDataSplitNumber;
        }

        /// <summary>
        /// Get the TOTAL size (in bits) generated by the Hamming Code with a certain DATA size (in bits).
        /// </summary>
        /// <param name="dataSize"></param>
        /// <returns></returns>
        public static int GetTotalSize(int dataSize)
        {
            // 8 bits of data -> 4 bits of controle + 1 master = 13 bits
            return dataSize / HammingDataSplitNumber * 13;
        }

        /// <summary>
        /// Return true if the number is a power of 2, false otherwise.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static bool IsPowerOf2(int i)
        {
            return (i & (i - 1)) == 0;
        }

        /// <summary>
        /// Return true if i is in the decomposition of j
        /// e.g. j = 11 and i = 2
        /// Decomposition of 11 = 8 + 2 + 1
        /// Return true
        /// </summary>
        /// <param name="j"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private static bool IsBitSet(int j, int i)
        {
            return (j & i) != 0;
        }

        /// <summary>
        /// Return a string representation of a BitArray.
        /// e.g. "010101110"
        /// </summary>
        /// <param name="bitArray"></param>
        /// <returns></returns>
        public static string BitArrayToDigitString(BitArray bitArray)
        {
            var builder = new StringBuilder();
            foreach (bool bit in bitArray)
                builder.Append(bit ? "1" : "0");
            return builder.ToString();
        }

        /// <summary>
        /// Get the parity count for a number.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="bitArray"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Resize the bitArray input with the new array size.
        /// If the new bitArray is bigger, the new added bits are fill with 0.
        /// If the new bitArray is smaller, fill the new bitArray bits with the as maximum bits as possible.
        /// </summary>
        /// <param name="bitArray"></param>
        /// <param name="newArraySize"></param>
        /// <param name="startIndex">Index where to start the copy</param>
        /// <returns></returns>
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
