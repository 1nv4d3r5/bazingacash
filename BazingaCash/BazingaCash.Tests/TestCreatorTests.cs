using Moq;
using NUnit.Framework;

namespace BazingaCash.Tests
{
    [TestFixture]
    public class TestCreatorTests : BaseFixture
    {
        #region CanCreateObjectWithEmptyConstructor
        class ObjectWithEmptyConstructor { }
        [Test]
        public void CanCreateObjectWithEmptyConstructor()
        {
            // Arrange and Act
            var obj =
                (ObjectWithEmptyConstructor)
                   Create<ObjectWithEmptyConstructor>();
            // Assert
            Assert.That(obj, Is.Not.Null);
            Assert.That(obj.GetType(), Is.EqualTo(typeof(ObjectWithEmptyConstructor)));
        }
        #endregion

        #region CanMockDependentMethod
        // tested type:
        class Abc1
        {
            private readonly IDep1 _dep;
            public Abc1(IDep1 dep)
            {
                _dep = dep;
            }
            public string AbcFormat(string str)
            {
                return "ABC->" + _dep.DepFormat(str);
            }
        }
        // dependency:
        public interface IDep1
        {
            string DepFormat(string str);
        }
        [Test]
        public void CanMockDependentMethodByUsingMockMethod()
        {
            // Arrange
            Abc1 obj = Create<Abc1>()
                        .Mock<IDep1>(m => m.Setup(dep =>
                            dep.DepFormat(It.IsAny<string>()))
                                .Returns<string>(s =>
                                    "DEP->" + s));
            // Act
            var result = obj.AbcFormat("123");
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.StringMatching("ABC->DEP->123"));
        }
        [Test]
        public void CanMockDependentMethodByUsingMockAndSetupMethod()
        {
            // Arrange
            Abc1 obj = Create<Abc1>()
                        .MockAndSetup<IDep1, string>(dep =>
                            dep.DepFormat(It.IsAny<string>()), _ => _
                                .Returns<string>(s =>
                                    "DEP->" + s));
            // Act
            var result = obj.AbcFormat("123");
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.StringMatching("ABC->DEP->123"));
        }
        #endregion

        #region CanMockDependentProperty
        // tested type:
        class Abc2
        {
            private readonly IDep2 _dep;
            public Abc2(IDep2 dep)
            {
                _dep = dep;
            }
            public string AbcFormat()
            {
                return "ABC->" + _dep.Property;
            }
        }
        // dependency:
        public interface IDep2
        {
            string Property { get; }
        }
        [Test]
        public void CanMockDependentProperty()
        {
            // Arrange
            Abc2 obj = Create<Abc2>()
                        .Mock<IDep2>(m => m.SetupGet(dep =>
                            dep.Property)
                                .Returns("DEP"));
            // Act
            var result = obj.AbcFormat();
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.StringMatching("ABC->DEP"));
        }
        #endregion

        #region CanReplaceVoidMethodWithCallBack
        // tested type:
        class Abc3
        {
            private readonly IDep3 _dep;
            public Abc3(IDep3 dep)
            {
                _dep = dep;
            }
            public void CallXyzVoidMethod(int state)
            {
                _dep.VoidMethod(state);
            }
        }
        // dependency:
        public interface IDep3
        {
            void VoidMethod(int state);
        }
        [Test]
        public void CanReplaceVoidMethodWithCallBack()
        {
            // Arrange
            int calledState = 0;
            Abc3 obj = Create<Abc3>()
                        .Mock<IDep3>(m => m.Setup(dep =>
                            dep.VoidMethod(It.IsAny<int>()))
                                .Callback<int>(i => calledState = i));
            const int expectedState = 11;
            // Act
            obj.CallXyzVoidMethod(expectedState);
            // Assert
            Assert.That(calledState, Is.EqualTo(expectedState));
        }
        #endregion

        #region CanSetDependentProperties
        // tested type:
        class Abc4
        {
            private readonly IDep4 _dep;
            public Abc4(IDep4 dep)
            {
                _dep = dep;
            }
            public string GetSummary()
            {
                return _dep.Name + ";" + _dep.Description;
            }
        }
        // dependency:
        public interface IDep4
        {
            string Name { get; }
            string Description { get; }
        }
        [Test]
        public void CanSetDependentProperties()
        {
            // Arrange
            const string name = "n";
            const string description = "d";
            Abc4 obj = Create<Abc4>()
                        .Set<IDep4>(new { Name = name, Description = description });
            // Act
            var summary = obj.GetSummary();
            // Assert
            Assert.That(summary, Is.Not.Null);
            Assert.That(summary, Is.EqualTo(name + ";" + description));
        }
        #endregion
    }
}