using Mocked;
using Moq;

namespace Mocker.Moq.Tests
{
    public class MockTests
    {
        [Test]
        public void can_mock_non_virtual_method()
        {
            //var mock1 = new Mock<Type>();
            var mock = new Mock<ClassToMock>();
            mock.Setup(x => x.MethodToMock()).Returns(true);
            Assert.IsTrue(mock.Object.MethodToMock());
        }

        [Test]
        public void can_run_mock_property()
        {
            var mock = new Mock<ClassToMock>();
            mock.Setup(x => x.PropertyToMock).Returns(true);
            Assert.IsTrue(mock.Object.PropertyToMock);
        }
    }
}