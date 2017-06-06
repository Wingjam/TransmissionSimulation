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

            // #1 case 1000001 -> [1]00100001001
            BitArray bitArray = new BitArray(7)
            {
                [0] = true,
                [6] = true
            };

            BitArray bitArrayExpected = new BitArray(12)
            {
                [0] = true,
                [3] = true,
                [8] = true,
                [11] = true
            };

            BitArray bitArrayoutput = HammingHelper.Encrypt(bitArray);
            Assert.AreEqual(HammingHelper.BitArrayToDigitString(bitArrayExpected), HammingHelper.BitArrayToDigitString(bitArrayoutput));

            
            // #2 case 1011 -> [0]0110011
            bitArray = new BitArray(4)
            {
                [0] = true,
                [2] = true,
                [3] = true
            };
            bitArrayExpected = new BitArray(8)
            {
                [2] = true,
                [3] = true,
                [6] = true,
                [7] = true
            };

            bitArrayoutput = HammingHelper.Encrypt(bitArray);
            Assert.AreEqual(HammingHelper.BitArrayToDigitString(bitArrayExpected), HammingHelper.BitArrayToDigitString(bitArrayoutput));
        }

        [TestMethod]
        public void TestDecrypt()
        {
            // #1 case [1]00100001001 -> 1000001
            BitArray bitArray = new BitArray(12)
            {
                [0] = true,
                [3] = true,
                [8] = true,
                [11] = true,
            };

            BitArray bitArrayExpected = new BitArray(7)
            {
                [0] = true,
                [6] = true,
            };

            BitArray bitArrayoutput = HammingHelper.Decrypt(bitArray);
            Assert.AreEqual(HammingHelper.BitArrayToDigitString(bitArrayExpected), HammingHelper.BitArrayToDigitString(bitArrayoutput));

            //// #1 case with 1 error
            //bitArray = new BitArray(11)
            //{
            //    //[2] = true, // this is the error
            //    [7] = true,
            //    [10] = true,
            //};

            //bitArrayExpected = new BitArray(7)
            //{
            //    [0] = true,
            //    [6] = true,
            //};

            //bitArrayoutput = HammingHelper.Decrypt(bitArray);
            //Assert.AreEqual(HammingHelper.BitArrayToDigitString(bitArrayExpected), HammingHelper.BitArrayToDigitString(bitArrayoutput));

            //// #2 case [0]0110011 -> 1011
            //bitArray = new BitArray(8)
            //{
            //    [2] = true,
            //    [3] = true,
            //    [6] = true,
            //    [7] = true
            //};

            //bitArrayExpected = new BitArray(4)
            //{
            //    [0] = true,
            //    [2] = true,
            //    [3] = true
            //};

            //bitArrayoutput = HammingHelper.Decrypt(bitArray);
            //Assert.AreEqual(HammingHelper.BitArrayToDigitString(bitArrayExpected), HammingHelper.BitArrayToDigitString(bitArrayoutput));

        }

        [TestMethod]
        public void TestEncryptDecryptManager()
        {
            // #1 case 10000010 (8 bits)
            BitArray bitArray = new BitArray(8)
            {
                [0] = true,
                [6] = true
            };

            BitArray bitArrayEncrypt = HammingHelper.EncryptManager(bitArray);
            BitArray bitArrayDecrypt = HammingHelper.Decrypt(bitArrayEncrypt);

            Assert.AreEqual(HammingHelper.BitArrayToDigitString(bitArray), HammingHelper.BitArrayToDigitString(bitArrayDecrypt));


            //// # Random case
            //// TODO
            //bitArray = new BitArray(50)
            //{
            //    [0] = true,
            //    [6] = true,
            //    [8] = true,
            //    [18] = true,
            //    [20] = true,
            //    [29] = true,
            //};

            //bitArrayEncrypt = HammingHelper.Encrypt(bitArray);
            //bitArrayDecrypt = HammingHelper.Decrypt(bitArrayEncrypt);

            //Assert.AreEqual(HammingHelper.BitArrayToDigitString(bitArray), HammingHelper.BitArrayToDigitString(bitArrayDecrypt));

        }

        [TestMethod]
        public void TestGetDataSize()
        {
            //Assert.AreEqual(4, HammingHelper.GetDataSize(8));
            //Assert.AreEqual(7, HammingHelper.GetDataSize(12));
            Assert.AreEqual(8, HammingHelper.GetDataSize(13));
            //Assert.AreEqual(512, HammingHelper.GetDataSize(128*8));
        }

        [TestMethod]
        public void TestGetTotalSize()
        {
            //Assert.AreEqual(8, HammingHelper.GetTotalSize(4));
            //Assert.AreEqual(12, HammingHelper.GetTotalSize(7));
            Assert.AreEqual(13, HammingHelper.GetTotalSize(8));
            Assert.AreEqual(26, HammingHelper.GetTotalSize(16));
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
