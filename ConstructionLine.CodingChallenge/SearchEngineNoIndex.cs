using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngineNoIndex : ISearchEngine
    {
        private readonly List<Shirt> _shirts;

        public SearchEngineNoIndex(List<Shirt> shirts)
        {
            _shirts = shirts;
        }


        public SearchResults Search(SearchOptions options)
        {
            var colorCounts = Color.All.ToDictionary(k => k.Id, s => new ColorCount {Color = s});
            var sizeCounts = Size.All.ToDictionary(k => k.Id, s => new SizeCount { Size = s });
            var shirts = new List<Shirt>();

            foreach (var shirt in _shirts)
            {
                var colorMath = !options.Colors.Any() || options.Colors.Any(a => a.Id == shirt.Color.Id);
                var sizeMath = !options.Sizes.Any() || options.Sizes.Any(a => a.Id == shirt.Size.Id);

                if (colorMath)
                {
                    sizeCounts[shirt.Size.Id].Count++;
                }

                if (sizeMath)
                {
                    colorCounts[shirt.Color.Id].Count++;
                }

                if (colorMath || sizeMath)
                {
                    shirts.Add(shirt);
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