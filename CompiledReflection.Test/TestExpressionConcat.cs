using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using SethrysTestDomain;

namespace CompiledReflection.Test
{
    [TestFixture]
    public class TestExpressionConcat
    {
        private Book _book;
        private readonly Expression<Func<Book, Page>> _firstPage = b => b.Pages.First();
        private readonly Expression<Func<Page, Topic>> _firstTopic = p => p.Subjects.First();
        private readonly Expression<Func<Topic, string>> _topicName = t => t.Name;
        private readonly Expression<Func<Page, Book>> _pageBook = p => p.Book;

        [SetUp]
        public void SetUp()
        {
            _book = new Book
            {
                Isbn = 1234567890123,
                Author = "Me!",
                Name = "Such Text"
            };

            _book.Pages = new[]
            {
                new Page
                {
                    Book = _book,
                    Isbn = _book.Isbn,
                    Number = 1,
                    Subjects = new[]
                    {
                        new Topic { Name = "Test Topic" }
                    }
                }
            };
        }

        [Test]
        public void TestConcat2()
        {
            var firstBookTopic = _firstPage.Concat(_firstTopic).Compile();
            Assert.That(firstBookTopic(_book), Is.EqualTo(_book.Pages.First().Subjects.First()));
        }

        [Test]
        public void TestConcat3()
        {
            var firstBookTopicName = _firstPage.Concat(_firstTopic).Concat(_topicName).Compile();
            Assert.That(firstBookTopicName(_book), Is.EqualTo("Test Topic"));
        }

        [Test]
        public void TestConcatWeird()
        {
            var firstBookTopicName = _firstPage
                .Concat(_pageBook)
                .Concat(_firstPage)
                .Concat(_pageBook)
                .Concat(_firstPage)
                .Concat(_pageBook)
                .Concat(_firstPage)
                .Concat(_firstTopic)
                .Concat(_topicName).Compile();
            Assert.That(firstBookTopicName(_book), Is.EqualTo("Test Topic"));
        }
    }
}