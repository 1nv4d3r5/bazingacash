using System;
using Moq;

namespace BazingaCash.Tests
{
    public abstract class BaseFixture
    {
        protected TestCreator<T> Create<T>()
        {
            return new TestCreator<T>();
        }
        protected TestCreator<T> Create<T>(MockBehavior mb)
        {
            return new TestCreator<T>(mb);
        }

        protected static T Mock<T>() where T : class
        {
            return MockAndSetup<T>(m => { }, o => { });
        }
        protected static T Mock<T>(Action<Mock<T>> setupMock) where T : class
        {
            return MockAndSetup(setupMock, o => { });
        }
        protected static T MockAndSetup<T>(Action<T> setupObject) where T : class
        {
            return MockAndSetup(m => { }, setupObject);
        }
        protected static T MockAndSetup<T>(Action<Mock<T>> setupMock, Action<T> setupObject) where T : class
        {
            var mock = new Mock<T>();
            setupMock(mock);
            var obj = mock.Object;
            setupObject(obj);
            return obj;
        }
    }
}