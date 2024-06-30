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
    }
}