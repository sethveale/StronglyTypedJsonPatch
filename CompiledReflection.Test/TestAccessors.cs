using System;
using NUnit.Framework;
using SethrysTestDomain;

namespace CompiledReflection.Test
{
    [TestFixture]
    public class TestAccessors
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
            var author = Accessors<Book>.AsFunc<string>("Author");
            Assert.That(author(_book), Is.EqualTo(_book.Author));
        }

        [Test]
        public void TestFieldAsFunc()
        {
            var name = Accessors<Book>.AsFunc<string>("Name");
            Assert.That(name(_book), Is.EqualTo(_book.Name));
        }

        [Test]
        public void TestAsFuncNameNotFound()
        {
            Assert.That(
                () => Accessors<Book>.AsFunc<string>("Author5"),
                Throws.InstanceOf<MissingMemberException>()
            );
        }

        [Test]
        public void TestAsFuncTypeNotFound()
        {
            Assert.That(
                () => Accessors<Book>.AsFunc<long>("Author"),
                Throws.InstanceOf<MissingMemberException>()
            );
        }

        #endregion

        #region AsLambda

        [Test]
        public void TestPropertyAsLambda()
        {
            var author = Accessors<Book>.AsLambda<string>("Author");
            Assert.That(author.Compile()(_book), Is.EqualTo(_book.Author));
        }

        [Test]
        public void TestFieldAsLambda()
        {
            var name = Accessors<Book>.AsLambda<string>("Name");
            Assert.That(name.Compile()(_book), Is.EqualTo(_book.Name));
        }

        [Test]
        public void TestAsLambdaNameNotFound()
        {
            Assert.That(
                () => Accessors<Book>.AsLambda<string>("Author5"),
                Throws.InstanceOf<MissingMemberException>()
            );
        }

        [Test]
        public void TestAsLambdaTypeNotFound()
        {
            Assert.That(
                () => Accessors<Book>.AsLambda<long>("Author"),
                Throws.InstanceOf<MissingMemberException>()
            );
        }

        #endregion
    }
}