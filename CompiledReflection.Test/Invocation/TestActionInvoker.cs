using System;
using System.Security.Permissions;
using CompiledReflection.Invocation;
using NUnit.Framework;

namespace CompiledReflection.Test.Invocation
{
    [TestFixture]
    public class TestActionInvoker
    {
        private class Tester
        {
            public static bool StaticHasInvoked;

            public static void SetInvoked(bool value)
            {
                StaticHasInvoked = value;
            }

            public bool HasInvoked;

            public void Zero()
            {
                HasInvoked = true;
            }

            public void One(int _)
            {
                HasInvoked = true;
            }

            public void Two(int x, string y)
            {
                HasInvoked = true;
            }
        }

        private Tester _tester;

        [SetUp]
        public void SetUp()
        {
            _tester = new Tester();
        }

        [Test]
        public void TestInvoke0()
        {
            var one = ActionInvoker.Create(typeof(Tester).GetMethod("Zero"));
            one.Invoke(_tester);
            Assert.That(_tester.HasInvoked, Is.True);
        }

        [Test]
        public void TestInvoke1()
        {
            var one = ActionInvoker.Create(typeof(Tester).GetMethod("One"));
            one.Invoke(_tester, 0);
            Assert.That(_tester.HasInvoked, Is.True);
        }

        [Test]
        public void TestInvoke2()
        {
            var one = ActionInvoker.Create(typeof(Tester).GetMethod("Two"));
            one.Invoke(_tester, 0, string.Empty);
            Assert.That(_tester.HasInvoked, Is.True);
        }

        [Test]
        public void TestStaticInvoke1()
        {
            var setStaticInvoked = ActionInvoker.Create(typeof(Tester).GetMethod("SetInvoked"));
            setStaticInvoked.Invoke(true);
            Assert.That(Tester.StaticHasInvoked, Is.True);
        }
    }
}