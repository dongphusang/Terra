using System;
namespace Terra.UnitTests
{
    [TestFixture]
    public class GraphicalTest
	{
        /// <summary>
        /// Test the number of lines in Temperature and Humidity line chart
        /// </summary>
        [Test]
        public void LineCharts_LineCountTest()
        {
            var viewmodel = new GraphicalPlantViewModel();
            Assert.Multiple(() =>
            {
                Assert.That(viewmodel.TempAndHumid, Has.Count.EqualTo(2));
                Assert.That(viewmodel.SoilMoisture, Has.Count.EqualTo(1));
            });
        }

        /// <summary>
        /// Test chart components for null after initialization in constructor
        /// </summary>
        [Test]
        public void ChartComponentNullTest()
        {
            var viewmodel = new GraphicalPlantViewModel();
            Assert.Multiple(() =>
            {
                Assert.That(viewmodel.LightGauge, Is.Not.Null);
                Assert.That(viewmodel.WaterLevelGauge, Is.Not.Null);
                Assert.That(viewmodel.TempAndHumid, Is.Not.Null);
                Assert.That(viewmodel.SoilMoisture, Is.Not.Null);

                Assert.That(viewmodel.XAxis, Is.Not.Null);
                Assert.That(viewmodel.YAxisTempHumid, Is.Not.Null);
                Assert.That(viewmodel.YAxisSoilMoisture, Is.Not.Null);
            });
        }

        /// <summary>
        /// Test data fetched from InfluxDB. If Unwrap() is working properly, it would wrap null with 'N/A' string
        /// </summary>
        [Test]
        public void InfluxDataFetch_UI_Test()
        {
            var viewmodel = new GraphicalPlantViewModel();
            Assert.That(viewmodel.GetDataFromInflux(), Is.Not.EqualTo("N/A"));
        }
        
    }
}



