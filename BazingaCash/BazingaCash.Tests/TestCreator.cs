using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Moq;
using Moq.Language.Flow;

namespace BazingaCash.Tests
{
    public class TestCreator<T>
    {
        private readonly List<Delegate> _delegates = new List<Delegate>();
        private readonly MockBehavior _mockBehavior;

        public TestCreator() : this(MockBehavior.Strict) { }
        public TestCreator(MockBehavior mockBehavior)
        {
            _mockBehavior = mockBehavior;
        }

        public static implicit operator T(TestCreator<T> c)
        {
            return c.CreateNewObject();
        }
        public T CreateNewObject()
        {
            try
            {
                var type = typeof(T);
                return (T)Activator.CreateInstance(type, ToParameters(type, _delegates, _mockBehavior));
            }
            catch (TargetInvocationException tiex)
            {
                // Get the _remoteStackTraceString of the Exception class
                FieldInfo remoteStackTraceString = typeof(Exception)
                    .GetField("_remoteStackTraceString",
                        BindingFlags.Instance | BindingFlags.NonPublic); // MS.Net

                // Set the InnerException._remoteStackTraceString
                // to the current InnerException.StackTrace
                if (remoteStackTraceString != null)
                    remoteStackTraceString.SetValue(tiex.InnerException,
                        tiex.InnerException.StackTrace + Environment.NewLine);

                // Throw the new exception
                throw tiex.InnerException;
            }
        }

        private static object[] ToParameters(Type type, IList<Delegate> mockHandlers, MockBehavior mockBehavior)
        {
            var constructor = type.GetConstructors().FirstOrDefault();
            if (constructor == null) throw new InvalidOperationException("Dependency object " + type + " does not have constructor");
            var list = new List<object>();
            foreach (var param in constructor.GetParameters())
            {
                var mockOfParamType = typeof(Mock<>).MakeGenericType(param.ParameterType);
                var actionMockType = typeof(Action<>).MakeGenericType(new[] { mockOfParamType });
                var actionSetType = typeof(Func<,>).MakeGenericType(mockOfParamType, typeof(object));
                var actionSetRawType = typeof(Func<>).MakeGenericType(param.ParameterType);
                var found = false;
                var mock = (Mock)Activator.CreateInstance(mockOfParamType, mockBehavior);
                if (mockHandlers != null)
                {
                    foreach (var candidate in mockHandlers)
                    {
                        var candidateType = candidate.GetType();
                        if (candidateType == actionMockType)
                        {
                            candidate.DynamicInvoke(mock);
                            list.Add(mock.Object);
                            found = true;
                            break;
                        }
                        if (candidateType == actionSetType)
                        {
                            var obj = candidate.DynamicInvoke(mock);
                            SetupGetters(mock, param.ParameterType, obj);
                            list.Add(mock.Object);
                            found = true;
                            break;
                        }
                        if (candidateType == actionSetRawType)
                        {
                            list.Add(candidate.DynamicInvoke());
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    list.Add(mock.Object);
                }
            }
            return list.ToArray();
        }
        private static void SetupGetters(Mock mock, Type type, object obj)
        {
            if (obj == null)
            {
                return;
            }
            foreach (var prop in obj.GetType().GetProperties())
            {
                var origProp = type.GetProperty(prop.Name);
                if (origProp == null || !origProp.CanRead) throw new ArgumentException(String.Format("Type {0} does not have readable property {1}", type, prop.Name));
                SetupMockGetter(mock, type, origProp.PropertyType, prop.Name, prop.GetValue(obj, null));
            }
        }
        private static void SetupMockGetter(Mock mock, Type objType, Type propType, string propName, object value)
        {
            var setupGetMethod = mock.GetType().GetMethod("SetupGet");
            var genericSetupGetMethod = setupGetMethod.MakeGenericMethod(propType);
            var obj = genericSetupGetMethod.Invoke(mock, new object[] { CreateGetterExpression(objType, propType, propName) });
            var isDelegate = value is Delegate;
            if (isDelegate)
            {
                var funcOfPropType = typeof(Func<>).MakeGenericType(new[] { propType });
                obj.GetType().GetMethod("Returns", new[] { funcOfPropType }).Invoke(obj, new[] { value });
            }
            else
            {
                obj.GetType().GetMethod("Returns", new[] { propType }).Invoke(obj, new[] { value });
            }
        }
        public static Expression<Func<TObj, TProp>> CreateGetterExpression<TObj, TProp>(string propName)
        {
            ParameterExpression objParam = Expression.Parameter(typeof(TObj), "obj");
            MemberExpression member = Expression.PropertyOrField(objParam, propName);
            return Expression.Lambda<Func<TObj, TProp>>(member, new[] { objParam });
        }
        public static Expression CreateGetterExpression(Type objType, Type propType, string propName)
        {
            var method = typeof(TestCreator<T>).GetMethod("CreateGetterExpression", new[] { typeof(string) });
            var genericMethod = method.MakeGenericMethod(objType, propType);
            return (Expression)genericMethod.Invoke(null, new object[] { propName });
        }

        public TestCreator<T> MockAndSetup<TDependency, TResult>(Expression<Func<TDependency, TResult>> expression, Action<ISetup<TDependency, TResult>> act)
             where TDependency : class
        {
            var func = new Action<Mock<TDependency>>(m =>
            {
                var setup = m.Setup(expression);
                if (act != null)
                {
                    act(setup);
                }
            });
            _delegates.Add(func);
            return this;
        }


        public TestCreator<T> Mock<TDependency>(Action<Mock<TDependency>> func)
             where TDependency : class
        {
            _delegates.Add(func);
            return this;
        }
        public TestCreator<T> Use<TDependency>(TDependency mocked)
             where TDependency : class
        {
            _delegates.Add(new Func<TDependency>(() => mocked));
            return this;
        }
        public TestCreator<T> Set<TDependency>(object prototype)
             where TDependency : class
        {
            if (prototype != null)
            {
                // verify that the propertie in the prototype have same property in the mocked object
                foreach (var prop in prototype.GetType().GetProperties())
                {
                    var property = typeof(TDependency).GetProperty(prop.Name);
                    if (property == null || !property.CanRead)
                    {
                        throw new ArgumentException(String.Format("Type {0} does not have readable property {1}", typeof(T), prop.Name));
                    }
                }
            }
            _delegates.Add(new Func<Mock<TDependency>, object>(m => prototype));
            return this;
        }

    }

}