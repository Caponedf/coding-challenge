﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ConstructionLine.CodingChallenge.Tests
{
    public class SearchEngineTestsBase
    {
        /// <summary>
        /// Simple factory for different implementations of <see cref="ISearchEngine"/>. 
        /// </summary>
        protected static Dictionary<Type, ISearchEngine> GetSearchEngines(List<Shirt> shirts)
        {
            return new Dictionary<Type, ISearchEngine>
            {
                {typeof(SearchEngineNoIndex), new SearchEngineNoIndex(shirts)},
                {typeof(SearchEngineWithIndex), new SearchEngineWithIndex(shirts)},
            };
        }

        protected static void AssertResults(List<Shirt> shirts, SearchOptions options)
        {
            Assert.That(shirts, Is.Not.Null);

            var resultingShirtIds = shirts.Select(s => s.Id).ToList();
            var sizeIds = options.Sizes.Select(s => s.Id).ToList();
            var colorIds = options.Colors.Select(c => c.Id).ToList();
            
            foreach (var shirt in shirts)
            {
                if ((!options.Sizes.Any() || sizeIds.Contains(shirt.Size.Id)) && (!options.Colors.Any() || colorIds.Contains(shirt.Color.Id)))
                {
                    if (!resultingShirtIds.Contains(shirt.Id))
                    {
                        Assert.Fail(
                            $"'{shirt.Name}' with Size '{shirt.Size.Name}' and Color '{shirt.Color.Name}' not found in results, " +
                            $"when selected sizes where '{string.Join(",", options.Sizes.Select(s => s.Name))}' " +
                            $"and colors '{string.Join(",", options.Colors.Select(c => c.Name))}'");
                    }
                }
                else
                {
                    // This would check false positive filter results.
                    if (resultingShirtIds.Contains(shirt.Id))
                    {
                        Assert.Fail(
                            $"'{shirt.Name}' with Size '{shirt.Size.Name}' and Color '{shirt.Color.Name}' found in results, " +
                            $"when selected sizes where '{string.Join(",", options.Sizes.Select(s => s.Name))}' " +
                            $"and colors '{string.Join(",", options.Colors.Select(c => c.Name))}'");
                    }
                }
            }
        }

        protected static void AssertSizeCounts(List<Shirt> shirts, SearchOptions searchOptions, List<SizeCount> sizeCounts)
        {
            Assert.That(sizeCounts, Is.Not.Null);

            foreach (var size in Size.All)
            {
                var sizeCount = sizeCounts.SingleOrDefault(s => s.Size.Id == size.Id);
                Assert.That(sizeCount, Is.Not.Null, $"Size count for '{size.Name}' not found in results");

                var expectedSizeCount = shirts
                    // I believe this Where is necessary in order to get correct number of shirts for filtered results.
                    .Where(c => !searchOptions.Sizes.Any() || searchOptions.Sizes.Select(s => s.Id).Contains(c.Size.Id))
                    .Where(s => s.Size.Id == size.Id)
                    .Count(s => !searchOptions.Colors.Any() || searchOptions.Colors.Select(c => c.Id).Contains(s.Color.Id));

                Assert.That(sizeCount.Count, Is.EqualTo(expectedSizeCount), 
                    $"Size count for '{sizeCount.Size.Name}' showing '{sizeCount.Count}' should be '{expectedSizeCount}'");
            }
        }


        protected static void AssertColorCounts(List<Shirt> shirts, SearchOptions searchOptions, List<ColorCount> colorCounts)
        {
            Assert.That(colorCounts, Is.Not.Null);
            
            foreach (var color in Color.All)
            {
                var colorCount = colorCounts.SingleOrDefault(s => s.Color.Id == color.Id);
                Assert.That(colorCount, Is.Not.Null, $"Color count for '{color.Name}' not found in results");

                var expectedColorCount = shirts
                    // I believe this Where is necessary in order to get correct number of shirts for filtered results.
                    .Where(s => !searchOptions.Colors.Any() || searchOptions.Colors.Select(c => c.Id).Contains(s.Color.Id))
                    .Where(c => c.Color.Id == color.Id)
                    .Count(c => !searchOptions.Sizes.Any() || searchOptions.Sizes.Select(s => s.Id).Contains(c.Size.Id));

                Assert.That(colorCount.Count, Is.EqualTo(expectedColorCount),
                    $"Color count for '{colorCount.Color.Name}' showing '{colorCount.Count}' should be '{expectedColorCount}'");
            }
        }
    }
}