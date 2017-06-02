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
        public void TestEncrypt()
        {

            // #1 case
            BitArray bitArray = new BitArray(7)
            {
                [0] = true,
                [6] = true
            };

            BitArray bitArrayExpected = new BitArray(11)
            {
                [2] = true,
                [7] = true,
                [10] = true
            };

            BitArray bitArrayoutput = HammingHelper.Encrypt(bitArray);
            Assert.AreEqual(HammingHelper.BitArrayToDigitString(bitArrayExpected), HammingHelper.BitArrayToDigitString(bitArrayoutput));

            
            // #2 case
            bitArray = new BitArray(4)
            {
                [0] = true,
                [2] = true,
                [3] = true
            };
            bitArrayExpected = new BitArray(7)
            {
                [1] = true,
                [2] = true,
                [5] = true,
                [6] = true
            };

            bitArrayoutput = HammingHelper.Encrypt(bitArray);
            Assert.AreEqual(HammingHelper.BitArrayToDigitString(bitArrayExpected), HammingHelper.BitArrayToDigitString(bitArrayoutput));
        }

        [TestMethod]
        public void TestDecrypt()
        {
            // #1 case
            BitArray bitArray = new BitArray(11)
            {
                [2] = true,
                [7] = true,
                [10] = true,
            };

            BitArray bitArrayExpected = new BitArray(7)
            {
                [0] = true,
                [6] = true,
            };

            BitArray bitArrayoutput = HammingHelper.Decrypt(bitArray);
            Assert.AreEqual(HammingHelper.BitArrayToDigitString(bitArrayExpected), HammingHelper.BitArrayToDigitString(bitArrayoutput));

            // #1 case with 1 error
            bitArray = new BitArray(11)
            {
                //[2] = true, // this is the error
                [7] = true,
                [10] = true,
            };

            bitArrayExpected = new BitArray(7)
            {
                [0] = true,
                [6] = true,
            };

            bitArrayoutput = HammingHelper.Decrypt(bitArray);
            Assert.AreEqual(HammingHelper.BitArrayToDigitString(bitArrayExpected), HammingHelper.BitArrayToDigitString(bitArrayoutput));

            // #2 case
            bitArray = new BitArray(7)
            {
                [1] = true,
                [2] = true,
                [5] = true,
                [6] = true
            };

            bitArrayExpected = new BitArray(4)
            {
                [0] = true,
                [2] = true,
                [3] = true
            };

            bitArrayoutput = HammingHelper.Decrypt(bitArray);
            Assert.AreEqual(HammingHelper.BitArrayToDigitString(bitArrayExpected), HammingHelper.BitArrayToDigitString(bitArrayoutput));

        }

        [TestMethod]
        public void TestGetDataSize()
        {
            Assert.AreEqual(7, HammingHelper.GetDataSize(11));
            Assert.AreEqual(4, HammingHelper.GetDataSize(7));
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
