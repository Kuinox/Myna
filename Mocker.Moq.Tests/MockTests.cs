using Moq;

namespace Mocker.Moq.Tests
{
    public class MockTests
    {
        [Test]
        public void can_mock_non_virtual_method()
        {
            var myClass = new MyClass();
            var mocker = new Mock<MyClass>(myClass);
            mocker.Setup(x => x.IsMocked).Returns(true);
            Assert.IsTrue(myClass.IsMocked);
        }
    }


    class MyClass{
        public bool IsMocked => false;
    }
}