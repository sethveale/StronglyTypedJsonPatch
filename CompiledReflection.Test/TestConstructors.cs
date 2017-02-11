using System;
using System.Globalization;
using NUnit.Framework;

namespace CompiledReflection.Test
{
    [TestFixture]
    public class TestConstructors
    {
        private const long Ticks = 120398120;
        private const DateTimeKind Kind = DateTimeKind.Utc;

        private const int Year = 2017, Month = 4, Day = 20;
        private static readonly Calendar Calendar = new GregorianCalendar();

        #region AsFunc

        [Test]
        public void TestFuncForConstructorThatDoesntExist()
        {
            Assert.That(
                () => Constructors.AsFunc<bool, DateTime>()(true),
                Throws.Exception
            );
        }

        [Test]
        public void TestFunc1()
        {
            Assert.That(
                Constructors.AsFunc<long, DateTime>()(Ticks),
                Is.EqualTo(new DateTime(Ticks))
            );
        }

        [Test]
        public void TestFunc2()
        {
            Assert.That(
                Constructors.AsFunc<long, DateTimeKind, DateTime>()(Ticks, Kind),
                Is.EqualTo(new DateTime(Ticks, Kind))
            );
        }

        [Test]
        public void TestFunc3()
        {
            Assert.That(
                Constructors.AsFunc<int, int, int, DateTime>()(Year, Month, Day),
                Is.EqualTo(new DateTime(Year, Month, Day))
            );
        }

        [Test]
        public void TestFunc4()
        {
            Assert.That(
                Constructors.AsFunc<int, int, int, Calendar, DateTime>()(Year, Month, Day, Calendar),
                Is.EqualTo(new DateTime(Year, Month, Day, Calendar))
            );
        }

        #endregion

        #region AsLambda

        [Test]
        public void TestLambdaForConstructorThatDoesntExist()
        {
            Assert.That(
                () => Constructors.AsLambda<bool, DateTime>().Compile()(true),
                Throws.Exception
            );
        }

        [Test]
        public void TestLambda1()
        {
            Assert.That(
                Constructors.AsLambda<long, DateTime>().Compile()(Ticks),
                Is.EqualTo(new DateTime(Ticks))
            );
        }

        [Test]
        public void TestLambda2()
        {
            Assert.That(
                Constructors.AsLambda<long, DateTimeKind, DateTime>().Compile()(Ticks, Kind),
                Is.EqualTo(new DateTime(Ticks, Kind))
            );
        }

        [Test]
        public void TestLambda3()
        {
            Assert.That(
                Constructors.AsLambda<int, int, int, DateTime>().Compile()(Year, Month, Day),
                Is.EqualTo(new DateTime(Year, Month, Day))
            );
        }

        [Test]
        public void TestLambda4()
        {
            Assert.That(
                Constructors.AsLambda<int, int, int, Calendar, DateTime>().Compile()(Year, Month, Day, Calendar),
                Is.EqualTo(new DateTime(Year, Month, Day, Calendar))
            );
        }

        #endregion
    }
}