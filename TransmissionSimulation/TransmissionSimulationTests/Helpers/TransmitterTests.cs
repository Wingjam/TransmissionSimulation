using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransmissionSimulation.Helpers;
using TransmissionSimulation.Components;
using TransmissionSimulation.Ressources;
using System.Threading;

namespace TransmissionSimulationTests.Helpers
{
    [TestClass]
    public class TransmitterTests
    {
        Transmitter t;
        BitArray data;

        [TestInitialize]
        public void SetUp()
        {
            t = new Transmitter();
            data = new BitArray(7);
        }

        [TestMethod]
        public void When_data_is_sent_then_it_is_received()
        {
            bool before = t.DataReceivedDest;
            Assert.IsTrue(t.TransmitterReady(Constants.Station.Source));
            t.SendData(data, Constants.Station.Source);
            Assert.AreNotEqual(before, t.DataReceivedDest);
        }

        [TestMethod]
        public void When_data_is_sent_but_not_obtained_then_transmitter_is_not_ready()
        {
            Assert.IsTrue(t.TransmitterReady(Constants.Station.Source));
            t.SendData(data, Constants.Station.Source);
            Assert.IsFalse(t.TransmitterReady(Constants.Station.Source));
        }

        [TestMethod]
        public void When_data_is_sent_and_obtained_then_transmitter_is_ready()
        {
            Assert.IsTrue(t.TransmitterReady(Constants.Station.Source));
            t.SendData(data, Constants.Station.Source);
            var dataDest = t.GetData(Constants.Station.Dest);
            Assert.IsTrue(t.TransmitterReady(Constants.Station.Source));
        }
    }
}
