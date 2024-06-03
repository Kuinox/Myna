// See https://aka.ms/new-console-template for more information
using Mocked;
using Mocker.API;

var notMocked = new ClassToMock();
notMocked.MethodToMock();

var mocked = new ClassToMock();
var type = typeof(ClassToMock);
var field = type.GetField("mockProxy")!;
mocked.MethodToMock();