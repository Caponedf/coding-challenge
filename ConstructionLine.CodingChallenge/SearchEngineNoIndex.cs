using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    /// <summary>
    /// Search engine implementation that always iterate all shirts without any index.
    /// </summary>
    public class SearchEngineNoIndex : ISearchEngine
    {
        private readonly List<Shirt> _shirts;

        public SearchEngineNoIndex(List<Shirt> shirts)
        {
            _shirts = shirts ?? throw new ArgumentException($"Parameter shirts is mandatory.", nameof(shirts));
        }


        public SearchResults Search(SearchOptions options)
        {
            if (options == null)
            {
                throw new ArgumentException($"Parameter option is mandatory.", nameof(options));
            }

            var colorCounts = Color.All.ToDictionary(k => k.Id, s => new ColorCount {Color = s});
            var sizeCounts = Size.All.ToDictionary(k => k.Id, s => new SizeCount { Size = s });
            var shirts = new List<Shirt>();

            foreach (var shirt in _shirts)
            {
                var colorMath = !options.Colors.Any() || options.Colors.Any(a => a.Id == shirt.Color.Id);
                var sizeMath = !options.Sizes.Any() || options.Sizes.Any(a => a.Id == shirt.Size.Id);

                if (colorMath && sizeMath)
                {
                    shirts.Add(shirt);
                    sizeCounts[shirt.Size.Id].Count++;
                    colorCounts[shirt.Color.Id].Count++;
                }
            }

            return new SearchResults
            {
                ColorCounts = colorCounts.Values.ToList(),
                SizeCounts = sizeCounts.Values.ToList(),
                Shirts =  shirts,
            };
        }
    }
}