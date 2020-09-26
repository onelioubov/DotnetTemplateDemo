using NUnit.Framework;

namespace ServiceDemo.Api.Tests
{
    [TestFixture]
    public class ThisTestShould
    {
        [Test]
        public void AlwaysPass()
        {
            Assert.That(1, Is.EqualTo(1));
        }
    }
}