using System;
using System.Collections.Generic;
using System.Diagnostics;
using ConstructionLine.CodingChallenge.Tests.SampleData;
using NUnit.Framework;

namespace ConstructionLine.CodingChallenge.Tests
{
    [TestFixture]
    public class SearchEnginePerformanceTests : SearchEngineTestsBase
    {
        private List<Shirt> _shirts;
        private Dictionary<Type, ISearchEngine> _searchEngine;

        [SetUp]
        public void Setup()
        {
            var dataBuilder = new SampleDataBuilder(50000);

            _shirts = dataBuilder.CreateShirts();

            _searchEngine = GetSearchEngines(_shirts);
        }


        [Test]
        [TestCase(typeof(SearchEngineNoIndex))]
        [TestCase(typeof(SearchEngineWithIndex))]
        public void PerformanceTest_Red_Color(Type searchEngineType)
        {
            var options = new SearchOptions
            {
                Colors = new List<Color> { Color.Red }
            };

            PerformanceTest(searchEngineType, options);
        }

        [Test]
        [TestCase(typeof(SearchEngineNoIndex))]
        [TestCase(typeof(SearchEngineWithIndex))]
        public void PerformanceTest_Large_Size(Type searchEngineType)
        {
            var options = new SearchOptions
            {
                Sizes = new List<Size> { Size.Large }
            };

            PerformanceTest(searchEngineType, options);
        }

        [Test]
        [TestCase(typeof(SearchEngineNoIndex))]
        [TestCase(typeof(SearchEngineWithIndex))]
        public void PerformanceTest_Red_Color_Small(Type searchEngineType)
        {
            var options = new SearchOptions
            {
                Colors = new List<Color> { Color.Red },
                Sizes = new List<Size> { Size.Small}
            };

            PerformanceTest(searchEngineType, options);
        }

        [Test]
        [TestCase(typeof(SearchEngineNoIndex))]
        [TestCase(typeof(SearchEngineWithIndex))]
        public void PerformanceTest_Red_Color_Small_And_Medium(Type searchEngineType)
        {
            var options = new SearchOptions
            {
                Colors = new List<Color> { Color.Red },
                Sizes = new List<Size> { Size.Small, Size.Medium }
            };

            PerformanceTest(searchEngineType, options);
        }

        [Test]
        [TestCase(typeof(SearchEngineNoIndex))]
        [TestCase(typeof(SearchEngineWithIndex))]
        public void PerformanceTest_Red_Color_And_White_Small_And_Medium(Type searchEngineType)
        {
            var options = new SearchOptions
            {
                Colors = new List<Color> { Color.Red, Color.White },
                Sizes = new List<Size> { Size.Small, Size.Medium }
            };

            PerformanceTest(searchEngineType, options);
        }

        private void PerformanceTest(Type searchEngineType, SearchOptions options)
        {
            var sw = new Stopwatch();
            sw.Start();

            var results = _searchEngine[searchEngineType].Search(options);

            sw.Stop();
            Console.WriteLine($"Test fixture finished in {sw.ElapsedMilliseconds} milliseconds");

            AssertResults(results.Shirts, options);
            AssertSizeCounts(_shirts, options, results.SizeCounts);
            AssertColorCounts(_shirts, options, results.ColorCounts);
        }
    }
}
