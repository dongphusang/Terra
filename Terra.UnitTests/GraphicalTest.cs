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
        public void TempAndHumid_NumbOfLinesTest()
        {
            var viewmodel = new GraphicalPlantViewModel();
            Assert.AreEqual(2, viewmodel.TempAndHumid.Count);
        }
    }
}



