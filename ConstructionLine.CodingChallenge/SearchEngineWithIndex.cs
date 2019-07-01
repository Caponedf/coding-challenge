using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngineWithIndex : ISearchEngine
    {
        private readonly List<Shirt> _shirts;

        private readonly Dictionary<(Color color, Size size), List<Shirt>> _shirtsCombinations;

        public SearchEngineWithIndex(List<Shirt> shirts)
        {
            _shirtsCombinations = new Dictionary<(Color color, Size size), List<Shirt>>();

            _shirts = shirts;

            foreach (var shirt in this._shirts)
            {
                if (!_shirtsCombinations.TryGetValue((shirt.Color, shirt.Size), out var index))
                {
                    index = new List<Shirt>();
                    _shirtsCombinations[(shirt.Color, shirt.Size)] = index;
                }

                index.Add(shirt);
            }

        }


        public SearchResults Search(SearchOptions options)
        {
            var colorCounts = Color.All.Select(c => new ColorCount
            {
                Color = c,
                Count = _shirtsCombinations
                    .Where(k => c.Id == k.Key.color.Id && (!options.Sizes.Any() || options.Sizes.Any(a => a.Id == k.Key.size.Id)))
                    .Sum(s => s.Value.Count)
            }).ToList();

            var sizeCounts = Size.All.Select(c => new SizeCount()
            {
                Size = c,
                Count = _shirtsCombinations
                    .Where(k => c.Id == k.Key.size.Id && (!options.Colors.Any() || options.Colors.Any(a => a.Id == k.Key.color.Id)))

                    .Sum(s => s.Value.Count)
            }).ToList();

            var shirts = _shirtsCombinations
                .Where(w => options.Sizes.Any(a => a.Id == w.Key.size.Id) && options.Colors.Any(a => a.Id == w.Key.color.Id))
                .SelectMany(s => s.Value).ToList();

            return new SearchResults
            {
                ColorCounts = colorCounts,
                SizeCounts = sizeCounts,
                Shirts = shirts,
            };
        }
    }
}