using AutoFixture;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Attributes
{
    public class MaxWordsAttributeTests
    {
        private const int MaxWords = 10;

        [Test]

        public void Invalid_String_Returns_False()
        {
            var invalidString = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor.";
            var sut = new MaxWordsAttribute(MaxWords);
            var result = sut.IsValid(invalidString);

            Assert.IsFalse(result);
        }

        [Test]
        public void Null_String_Returns_True()
        {
            var sut = new MaxWordsAttribute(MaxWords);
            var result = sut.IsValid(null);

            Assert.IsTrue(result);
        }

        [Test]
        public void Non_String_Returns_True()
        {
            var sut = new MaxWordsAttribute(MaxWords);
            var result = sut.IsValid(new { });

            Assert.IsTrue(result);
        }

        [Test]

        public void Valid_String_Returns_True()
        {
            var validString = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do";
            var sut = new MaxWordsAttribute(MaxWords);
            var result = sut.IsValid(validString);

            Assert.IsTrue(result);
        }

        [Test]

        public void Invalid_SingleWordPerLine_String_Returns_False()
        {
            var invalidString = "Lorem\r\nipsum\r\ndolor\r\nsit\r\namet\r\nconsectetur\r\nadipiscing\r\nelit\r\nsed\r\ndo\r\neiusmod\r\ntempor\r\nincididunt\r\nut\r\nlabore\r\net\r\ndolore magna\r\naliqua\r\nUt\r\nenim\r\nad\r\\r\nveniam\r\\r\nnostrud\r\\r\nullamco\r\nlaboris\r\\r\nut\r\naliquip\r\nexea\r\nconsequat.";
            var sut = new MaxWordsAttribute(MaxWords);
            var result = sut.IsValid(invalidString);

            Assert.IsFalse(result);
        }
    }
}
