using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransmissionSimulation.Helpers;

namespace TransmissionSimulationTests.Helpers
{
    [TestClass]
    public class HammingHelperTests
    {

        [TestMethod]
        public void TestEncryptDecryptManager()
        {
            // #1 case 10000010 (8 bits)
            BitArray bitArray = new BitArray(8)
            {
                [0] = true,
                [6] = true
            };

            BitArray bitArrayEncrypt = HammingHelper.EncryptManager(bitArray, HammingHelper.Mode.CORRECT).Item1;
            BitArray bitArrayDecrypt = HammingHelper.DecryptManager(bitArrayEncrypt, HammingHelper.Mode.CORRECT).Item1;

            Assert.AreEqual(HammingHelper.BitArrayToDigitString(bitArray), HammingHelper.BitArrayToDigitString(bitArrayDecrypt));


            // # Random case
            bitArray = new BitArray(32)
            {
                [0] = false,
                [6] = true,
                [8] = true,
                [18] = true,
                [20] = true,
                [29] = true,
            };

            Tuple<BitArray, HammingHelper.Status> tupleEncrypt = HammingHelper.EncryptManager(bitArray, HammingHelper.Mode.CORRECT);
            Tuple<BitArray, HammingHelper.Status> tupleDecrypt = HammingHelper.DecryptManager(tupleEncrypt.Item1, HammingHelper.Mode.CORRECT);

            Assert.AreEqual(HammingHelper.Status.OK, tupleEncrypt.Item2);
            Assert.AreEqual(HammingHelper.Status.OK, tupleDecrypt.Item2);
            Assert.AreEqual(HammingHelper.BitArrayToDigitString(bitArray), HammingHelper.BitArrayToDigitString(tupleDecrypt.Item1));

            // # Random case with 1 error (CORRECTED)
            bitArray = new BitArray(8)
            {
                [0] = true,
                [3] = true,
                [5] = true,
            };

            tupleEncrypt = HammingHelper.EncryptManager(bitArray, HammingHelper.Mode.CORRECT);
            tupleEncrypt.Item1[9] = !tupleEncrypt.Item1[9]; // Inject error here
            tupleDecrypt = HammingHelper.DecryptManager(tupleEncrypt.Item1, HammingHelper.Mode.CORRECT);

            Assert.AreEqual(HammingHelper.Status.OK, tupleEncrypt.Item2);
            Assert.AreEqual(HammingHelper.Status.CORRECTED, tupleDecrypt.Item2);
            Assert.AreEqual(HammingHelper.BitArrayToDigitString(bitArray), HammingHelper.BitArrayToDigitString(tupleDecrypt.Item1));


            // # Random case with 2 error (DETECTED)
            bitArray = new BitArray(8)
            {
                [0] = true,
                [3] = true,
                [5] = true,
            };

            tupleEncrypt = HammingHelper.EncryptManager(bitArray, HammingHelper.Mode.DETECT);
            tupleEncrypt.Item1[1] = !tupleEncrypt.Item1[1]; // Inject error here
            tupleEncrypt.Item1[9] = !tupleEncrypt.Item1[9]; // Inject error here
            tupleDecrypt = HammingHelper.DecryptManager(tupleEncrypt.Item1, HammingHelper.Mode.DETECT);

            Assert.AreEqual(HammingHelper.Status.OK, tupleEncrypt.Item2);
            Assert.AreEqual(HammingHelper.Status.DETECTED, tupleDecrypt.Item2);

            // # Random case with 3 error (1 CORRECTED and 2 DETECTED)
            bitArray = new BitArray(16)
            {
                [0] = true,
                [1] = true,
                [5] = true,
                [7] = true,
                [13] = true,
            };

            tupleEncrypt = HammingHelper.EncryptManager(bitArray, HammingHelper.Mode.CORRECT);
            tupleEncrypt.Item1[1] = !tupleEncrypt.Item1[1]; // Inject error here (first 13 bits)
            tupleEncrypt.Item1[20] = !tupleEncrypt.Item1[20]; // Inject error here (second 13 bits)
            tupleEncrypt.Item1[25] = !tupleEncrypt.Item1[25]; // Inject error here (second 13 bits)
            tupleDecrypt = HammingHelper.DecryptManager(tupleEncrypt.Item1, HammingHelper.Mode.CORRECT);

            Assert.AreEqual(HammingHelper.Status.OK, tupleEncrypt.Item2);
            Assert.AreEqual(HammingHelper.Status.DETECTED, tupleDecrypt.Item2);


            // # Random case with 3 error (NOT SUPPORTED)
            bitArray = new BitArray(8)
            {
                [0] = true,
                [1] = true,
                [5] = true,
            };

            tupleEncrypt = HammingHelper.EncryptManager(bitArray, HammingHelper.Mode.CORRECT);
            tupleEncrypt.Item1[1] = !tupleEncrypt.Item1[1]; // Inject error here
            tupleEncrypt.Item1[5] = !tupleEncrypt.Item1[5]; // Inject error here
            tupleEncrypt.Item1[9] = !tupleEncrypt.Item1[9]; // Inject error here
            tupleDecrypt = HammingHelper.DecryptManager(tupleEncrypt.Item1, HammingHelper.Mode.CORRECT);

            Assert.AreEqual(HammingHelper.Status.OK, tupleEncrypt.Item2);
            Assert.AreEqual(HammingHelper.Status.DETECTED, tupleDecrypt.Item2);


            // # Random case with 1 (Detect only 1 bit)
            bitArray = new BitArray(8)
            {
                [0] = true,
                [1] = true,
                [5] = true,
            };

            tupleEncrypt = HammingHelper.EncryptManager(bitArray, HammingHelper.Mode.DETECT);
            tupleEncrypt.Item1[1] = !tupleEncrypt.Item1[1]; // Inject error here
            tupleDecrypt = HammingHelper.DecryptManager(tupleEncrypt.Item1, HammingHelper.Mode.DETECT);

            Assert.AreEqual(HammingHelper.Status.OK, tupleEncrypt.Item2);
            Assert.AreEqual(HammingHelper.Status.DETECTED, tupleDecrypt.Item2);

            for (int num = 0; num < 256; ++num)
            {

                for (int i = 0; i < 13; ++i)
                {
                    for (int j = 0; j < 13; ++j)
                    {
                        if (i == j)
                        {
                            continue;
                        }
                        byte[] bytes = BitConverter.GetBytes(num);
                        bitArray = new BitArray(new byte[]{bytes[0]});

                        tupleEncrypt = HammingHelper.EncryptManager(bitArray, HammingHelper.Mode.CORRECT);
                        tupleEncrypt.Item1[j] = !tupleEncrypt.Item1[j]; // Inject error here
                        tupleEncrypt.Item1[i] = !tupleEncrypt.Item1[i]; // Inject error here
                        tupleDecrypt = HammingHelper.DecryptManager(tupleEncrypt.Item1, HammingHelper.Mode.CORRECT);

                        Assert.AreEqual(HammingHelper.Status.OK, tupleEncrypt.Item2);
                        Assert.AreEqual(HammingHelper.Status.DETECTED, tupleDecrypt.Item2);
                    }
                }
            }

        }

        [TestMethod]
        public void TestGetDataSize()
        {
            Assert.AreEqual(8, HammingHelper.GetDataSize(13));
            Assert.AreEqual(16, HammingHelper.GetDataSize(26));
        }

        [TestMethod]
        public void TestGetTotalSize()
        {
            Assert.AreEqual(13, HammingHelper.GetTotalSize(8));
            Assert.AreEqual(26, HammingHelper.GetTotalSize(16));
        }

        [TestMethod]
        public void TestResizeBitArrayBigger()
        {
            PrivateType hh = new PrivateType(typeof(HammingHelper));

            BitArray bitArrayInput = new BitArray(10)
            {
                [0] = true,
                [2] = true,
                [8] = true,
                [9] = true,
            };

            BitArray bitArrayExpected = new BitArray(18)
            {
                [0] = true,
                [2] = true,
                [8] = true,
                [9] = true,
            };
            object[] args = { bitArrayInput, 18, 0 };
            var bitArrayOutput = (BitArray)hh.InvokeStatic("ResizeBitArray", args);

            Assert.AreEqual(HammingHelper.BitArrayToDigitString(bitArrayExpected), HammingHelper.BitArrayToDigitString(bitArrayOutput));
        }

        [TestMethod]
        public void TestResizeBitArraySmaller()
        {
            PrivateType hh = new PrivateType(typeof(HammingHelper));

            BitArray bitArrayInput = new BitArray(10)
            {
                [0] = true,
                [2] = true,
                [8] = true,
                [9] = true,
            };

            BitArray bitArrayExpected = new BitArray(4)
            {
                [0] = true,
                [2] = true,
            };
            object[] args = { bitArrayInput, 4, 0 };
            var bitArrayOutput = (BitArray)hh.InvokeStatic("ResizeBitArray", args);

            Assert.AreEqual(HammingHelper.BitArrayToDigitString(bitArrayExpected), HammingHelper.BitArrayToDigitString(bitArrayOutput));
        }

        [TestMethod]
        public void TestResizeBitArraySmallerWithIndex()
        {
            PrivateType hh = new PrivateType(typeof(HammingHelper));

            BitArray bitArrayInput = new BitArray(10)
            {
                [0] = true,
                [2] = true,
                [8] = true,
                [9] = true,
            };

            BitArray bitArrayExpected = new BitArray(4)
            {
                [2] = true,
                [3] = true,
            };
            object[] args = { bitArrayInput, 4, 6 };
            var bitArrayOutput = (BitArray)hh.InvokeStatic("ResizeBitArray", args);

            Assert.AreEqual(HammingHelper.BitArrayToDigitString(bitArrayExpected), HammingHelper.BitArrayToDigitString(bitArrayOutput));
        }

        [TestMethod]
        public void TestIsPowerOf2()
        {
            Assert.AreEqual(true, HammingHelper.IsPowerOf2(0));
            Assert.AreEqual(true, HammingHelper.IsPowerOf2(1));
            Assert.AreEqual(true, HammingHelper.IsPowerOf2(2));
            Assert.AreEqual(false, HammingHelper.IsPowerOf2(3));
            Assert.AreEqual(true, HammingHelper.IsPowerOf2(4));
            Assert.AreEqual(false, HammingHelper.IsPowerOf2(5));
            Assert.AreEqual(true, HammingHelper.IsPowerOf2(8));
            Assert.AreEqual(true, HammingHelper.IsPowerOf2(512));
            Assert.AreEqual(true, HammingHelper.IsPowerOf2(1024));
            Assert.AreEqual(false, HammingHelper.IsPowerOf2(1000));
        }

        [TestMethod]
        public void TestBitArrayToDigitString()
        {
            BitArray bitArray = new BitArray(5);
            Assert.AreEqual("00000", HammingHelper.BitArrayToDigitString(bitArray));

            bitArray[0] = true;
            bitArray[2] = true;
            Assert.AreEqual("10100", HammingHelper.BitArrayToDigitString(bitArray));

            bitArray.SetAll(true);
            Assert.AreEqual("11111", HammingHelper.BitArrayToDigitString(bitArray));
        }
    }
}
