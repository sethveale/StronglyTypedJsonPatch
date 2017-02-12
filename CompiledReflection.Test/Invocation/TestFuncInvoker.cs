using System;
using CompiledReflection.Invocation;
using NUnit.Framework;
using SethrysTestDomain;

namespace CompiledReflection.Test.Invocation
{
    [TestFixture]
    public class TestFuncInvoker
    {
        private string _string;

        [SetUp]
        public void SetUp()
        {
            _string = "fubar";
        }

        [Test]
        public void TestInvoke0()
        {
            var getLength = FuncInvoker.Create(typeof(string).GetProperty("Length").GetMethod);
            Assert.That((int) getLength.Invoke(_string), Is.EqualTo(_string.Length));
        }

        [Test]
        public void TestInvoke1()
        {
            var contains = FuncInvoker.Create(typeof(string).GetMethod("Contains", new[] { typeof(string) }));
            Assert.That((bool) contains.Invoke(_string, "uba"), Is.True);
        }

        [Test]
        public void TestInvoke2()
        {
            var endsWith = FuncInvoker.Create(typeof(string).GetMethod("EndsWith", new[] { typeof(string), typeof(StringComparison) }));
            Assert.That((bool) endsWith.Invoke(_string, "BAR", StringComparison.OrdinalIgnoreCase), Is.True);
        }
    }
}