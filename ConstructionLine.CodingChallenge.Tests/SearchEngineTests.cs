using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ConstructionLine.CodingChallenge.Tests
{
    [TestFixture]
    public class SearchEngineTests : SearchEngineTestsBase
    {
        [Test]
        [TestCase(typeof(SearchEngineNoIndex))]
        [TestCase(typeof(SearchEngineWithIndex))]
        public void Test(Type searchEngineType)
        {
            var shirts = new List<Shirt>
            {
                new Shirt(Guid.NewGuid(), "Red - Small", Size.Small, Color.Red),
                new Shirt(Guid.NewGuid(), "Black - Medium", Size.Medium, Color.Black),
                new Shirt(Guid.NewGuid(), "Blue - Large", Size.Large, Color.Blue),
            };

            var searchEngine = GetSearchEngines(shirts)[searchEngineType];

            var searchOptions = new SearchOptions
            {
                Colors = new List<Color> {Color.Red}
            };

            var results = searchEngine.Search(searchOptions);

            AssertResults(results.Shirts, searchOptions);
            AssertSizeCounts(shirts, searchOptions, results.SizeCounts);
            AssertColorCounts(shirts, searchOptions, results.ColorCounts);
        }

        [Test]
        [TestCase(typeof(SearchEngineNoIndex))]
        [TestCase(typeof(SearchEngineWithIndex))]
        public void TestEmptyCriteria(Type searchEngineType)
        {
            var shirts = new List<Shirt>
            {
                new Shirt(Guid.NewGuid(), "Red - Small", Size.Small, Color.Red),
                new Shirt(Guid.NewGuid(), "Black - Medium", Size.Medium, Color.Black),
                new Shirt(Guid.NewGuid(), "Blue - Large", Size.Large, Color.Red),
            };

            var searchEngine = GetSearchEngines(shirts)[searchEngineType];

            var searchOptions = new SearchOptions
            {
            };

            var results = searchEngine.Search(searchOptions);

            Assert.IsTrue(shirts.Select(s => s.Id).SequenceEqual(results.Shirts.Select(s => s.Id)));
            Assert.AreEqual(2, results.ColorCounts.First(f => f.Color.Id == Color.Red.Id).Count);
            Assert.AreEqual(1, results.ColorCounts.First(f => f.Color.Id == Color.Black.Id).Count);
            Assert.True(results.SizeCounts.Any());
            Assert.True(results.SizeCounts.Any(a => a.Count == 1));

            AssertResults(results.Shirts, searchOptions);
            AssertSizeCounts(shirts, searchOptions, results.SizeCounts);
            AssertColorCounts(shirts, searchOptions, results.ColorCounts);
        }

        [Test]
        [TestCase(typeof(SearchEngineNoIndex))]
        [TestCase(typeof(SearchEngineWithIndex))]
        public void TestWrongCtor(Type searchEngineType)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var x = GetSearchEngines(null)[searchEngineType];
            });
        }

        [Test]
        [TestCase(typeof(SearchEngineNoIndex))]
        [TestCase(typeof(SearchEngineWithIndex))]
        public void TestWrongOption(Type searchEngineType)
        {
            var shirts = new List<Shirt>
            {
                new Shirt(Guid.NewGuid(), "Red - Small", Size.Small, Color.Red),
                new Shirt(Guid.NewGuid(), "Black - Medium", Size.Medium, Color.Black),
                new Shirt(Guid.NewGuid(), "Blue - Large", Size.Large, Color.Red),
            };

            Assert.Throws<ArgumentException>(() =>
            {
                var x = GetSearchEngines(shirts)[searchEngineType].Search(null);
            });
        }
    }
}