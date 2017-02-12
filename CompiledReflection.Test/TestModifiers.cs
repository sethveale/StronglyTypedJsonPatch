using System;
using NUnit.Framework;
using SethrysTestDomain;

namespace CompiledReflection.Test
{
    [TestFixture]
    public class TestModifiers
    {
        private Book _book;

        [SetUp]
        public void SetUp()
        {
            _book = new Book();
        }

        #region AsAction

        [Test]
        public void TestPropertyAsAction()
        {
            var author = Modifiers<Book>.AsAction<string>("Author");
            author(_book, "Me!");
            Assert.That(_book.Author, Is.EqualTo("Me!"));
        }

        [Test]
        public void TestFieldAsAction()
        {
            var name = Modifiers<Book>.AsAction<string>("Name");
            name(_book, "Such Book");
            Assert.That(_book.Name, Is.EqualTo("Such Book"));
        }

        [Test]
        public void TestAsActionNameNotFound()
        {
            Assert.That(
                () => Modifiers<Book>.AsAction<string>("Author5"),
                Throws.InstanceOf<MissingMemberException>()
            );
        }

        [Test]
        public void TestAsActionTypeNotFound()
        {
            Assert.That(
                () => Modifiers<Book>.AsAction<long>("Author"),
                Throws.InstanceOf<MissingMemberException>()
            );
        }

        #endregion

        #region AsLambda

        [Test]
        public void TestPropertyAsLambda()
        {
            var author = Modifiers<Book>.AsLambda<string>("Author");
            author.Compile()(_book, "Me!");
            Assert.That(_book.Author, Is.EqualTo("Me!"));
        }

        [Test]
        public void TestFieldAsLambda()
        {
            var name = Modifiers<Book>.AsLambda<string>("Name");
            name.Compile()(_book, "Such Book");
            Assert.That(_book.Name, Is.EqualTo("Such Book"));
        }


        [Test]
        public void TestAsLambdaNameNotFound()
        {
            Assert.That(
                () => Modifiers<Book>.AsLambda<string>("Author5"),
                Throws.InstanceOf<MissingMemberException>()
            );
        }

        [Test]
        public void TestAsLambdaTypeNotFound()
        {
            Assert.That(
                () => Modifiers<Book>.AsLambda<long>("Author"),
                Throws.InstanceOf<MissingMemberException>()
            );
        }

        #endregion
    }
}