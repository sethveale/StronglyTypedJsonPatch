using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SethrysTestDomain;

namespace CompiledReflection.Test
{
    [TestFixture]
    public class TestValuesOf
    {
        private Book _book;

        [SetUp]
        public void SetUp()
        {
            _book = new Book
            {
                Isbn = 1234567890123,
                Author = "Me!",
                Name = "Such Text"
            };
        }

        #region AsFunc

        [Test]
        public void TestPropertyAsFunc()
        {
            var author = ValuesOf<Book>.AsFunc<string>("Author");
            Assert.That(author(_book), Is.EqualTo(_book.Author));
        }

        [Test]
        public void TestFieldAsFunc()
        {
            var name = ValuesOf<Book>.AsFunc<string>("Name");
            Assert.That(name(_book), Is.EqualTo(_book.Name));
        }

        [Test]
        public void TestAsFuncNameNotFound()
        {
            Assert.That(
                () => ValuesOf<Book>.AsFunc<string>("Author5"),
                Throws.InstanceOf<MissingMemberException>()
            );
        }

        [Test]
        public void TestAsFuncTypeNotFound()
        {
            Assert.That(
                () => ValuesOf<Book>.AsFunc<long>("Author"),
                Throws.InstanceOf<MissingMemberException>()
            );
        }

        #endregion

        #region AsLambda

        [Test]
        public void TestPropertyAsLambda()
        {
            var author = ValuesOf<Book>.AsLambda<string>("Author");
            Assert.That(author.Compile()(_book), Is.EqualTo(_book.Author));
        }

        [Test]
        public void TestFieldAsLambda()
        {
            var name = ValuesOf<Book>.AsLambda<string>("Name");
            Assert.That(name.Compile()(_book), Is.EqualTo(_book.Name));
        }

        [Test]
        public void TestAsLambdaNameNotFound()
        {
            Assert.That(
                () => ValuesOf<Book>.AsLambda<string>("Author5"),
                Throws.InstanceOf<MissingMemberException>()
            );
        }

        [Test]
        public void TestAsLambdaTypeNotFound()
        {
            Assert.That(
                () => ValuesOf<Book>.AsLambda<long>("Author"),
                Throws.InstanceOf<MissingMemberException>()
            );
        }

        #endregion
    }
}
