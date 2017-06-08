using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransmissionSimulation.Components;
using TransmissionSimulation.Ressources;
using System.Threading;

namespace TransmissionSimulationTests.Components
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
            Assert.IsFalse(t.DataReceivedStation2);
            Assert.IsTrue(t.TransmitterReady(Constants.StationId.Station1));
            t.SendData(data, Constants.StationId.Station1);
            Thread.Sleep(Constants.DefaultDelay * 200);
            Assert.IsTrue(t.DataReceivedStation2);
        }

        [TestMethod]
        public void When_data_is_sent_but_not_obtained_then_transmitter_is_not_ready()
        {
            Assert.IsTrue(t.TransmitterReady(Constants.StationId.Station1));
            t.SendData(data, Constants.StationId.Station1);
            Thread.Sleep(Constants.DefaultDelay * 200);
            Assert.IsFalse(t.TransmitterReady(Constants.StationId.Station1));
        }

        [TestMethod]
        public void When_data_is_sent_and_obtained_then_transmitter_is_ready()
        {
            Assert.IsTrue(t.TransmitterReady(Constants.StationId.Station1));
            t.SendData(data, Constants.StationId.Station1);
            Thread.Sleep(Constants.DefaultDelay * 200);
            var dataStation2 = t.GetData(Constants.StationId.Station2);
            Assert.IsTrue(t.TransmitterReady(Constants.StationId.Station1));
        }

        [TestMethod]
        public void When_random_errors_are_injected_Then_frame_is_altered()
        {
            t.InsertRandomErrors(1, 1, 3);
            Assert.IsTrue(t.TransmitterReady(Constants.StationId.Station1));
            t.SendData(data, Constants.StationId.Station1);
            Thread.Sleep(Constants.DefaultDelay * 200);
            var dataStation2 = t.GetData(Constants.StationId.Station2);
            Assert.IsTrue(data == dataStation2);
            Assert.IsTrue(dataStation2[1] || dataStation2[2]);
        }

        [TestMethod]
        public void When_fixed_errors_are_injected_Then_frame_is_correctly_altered()
        {
            t.IndicesInversions.Add(1);
            t.IndicesInversions.Add(2);
            Assert.IsTrue(t.TransmitterReady(Constants.StationId.Station1));
            t.SendData(data, Constants.StationId.Station1);
            Thread.Sleep(Constants.DefaultDelay * 200);
            var dataStation2 = t.GetData(Constants.StationId.Station2);
            Assert.IsTrue(data == dataStation2);
            Assert.IsTrue(dataStation2[1] && dataStation2[2]);
        }
    }
}
