using Moq;

namespace Mocker.Moq.Tests
{
    public class MockTests
    {
        [Test]
        public void can_mock_non_virtual_method()
        {
            var mock1 = new Mock<Type>();
            var mock = new Mock<MyClass>();
            mock.Setup(x => x.IsMocked()).Returns(true);
            Assert.IsTrue(mock.Object.IsMocked());
        }

        class SubClass { }
    }


    class MyClass{
        public bool IsMocked() => false;
    }
}