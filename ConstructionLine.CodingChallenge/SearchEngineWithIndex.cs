using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngineWithIndex : ISearchEngine
    {
        private readonly Dictionary<(Color color, Size size), List<Shirt>> _shirtsIndex;

        public SearchEngineWithIndex(List<Shirt> shirts)
        {
            if (shirts == null)
            {
                throw new ArgumentException($"Parameter shirts is mandatory.", nameof(shirts));
            }

            _shirtsIndex = new Dictionary<(Color color, Size size), List<Shirt>>();

            foreach (var shirt in shirts)
            {
                if (!_shirtsIndex.TryGetValue((shirt.Color, shirt.Size), out var index))
                {
                    index = new List<Shirt>();
                    _shirtsIndex[(shirt.Color, shirt.Size)] = index;
                }

                index.Add(shirt);
            }
        }


        public SearchResults Search(SearchOptions options)
        {
            if (options == null)
            {
                throw new ArgumentException($"Parameter option is mandatory.", nameof(options));
            }

            var colorCounts = Color.All.Select(c => new ColorCount
            {
                Color = c,
                Count = _shirtsIndex
                    .Where(k => c.Id == k.Key.color.Id && (!options.Sizes.Any() || options.Sizes.Any(a => a.Id == k.Key.size.Id)))
                    .Sum(s => s.Value.Count)
            }).ToList();

            var sizeCounts = Size.All.Select(c => new SizeCount()
            {
                Size = c,
                Count = _shirtsIndex
                    .Where(k => c.Id == k.Key.size.Id && (!options.Colors.Any() || options.Colors.Any(a => a.Id == k.Key.color.Id)))

                    .Sum(s => s.Value.Count)
            }).ToList();

            var shirts = _shirtsIndex
                .Where(w => !options.Sizes.Any() || options.Sizes.Any(a => a.Id == w.Key.size.Id))
                .Where(w => !options.Colors.Any() || options.Colors.Any(a => a.Id == w.Key.color.Id))
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