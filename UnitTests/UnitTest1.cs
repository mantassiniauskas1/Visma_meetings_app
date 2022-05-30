using System.Linq;
namespace UnitTests
{
    public class UnitTesting
    {
        [Fact]
        public void GetData()
        {
            try
            {
                Meeting m = new Meeting("a", "a", "a", "a", "a", "a", "a");
                m.GetData();
                Assert.True(true);
            }
            catch
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void Filter()
        {
            try
            {
                Assert.True(true);
            }
            catch
            {
                Assert.True(false);
            }
        }



    }
}