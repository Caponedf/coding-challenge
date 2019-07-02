using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    /// <summary>
    /// Search engine implementation with index built in CTOR. 
    /// </summary>
    public class SearchEngineWithIndex : ISearchEngine
    {
        private readonly Dictionary<(Guid colorId, Guid sizeId), List<Shirt>> _shirtsIndex;

        public SearchEngineWithIndex(List<Shirt> shirts)
        {
            if (shirts == null)
            {
                throw new ArgumentException($"Parameter shirts is mandatory.", nameof(shirts));
            }

            _shirtsIndex = new Dictionary<(Guid colorId, Guid sizeId), List<Shirt>>();

            foreach (var shirt in shirts)
            {
                if (!_shirtsIndex.TryGetValue((shirt.Color.Id, shirt.Size.Id), out var index))
                {
                    index = new List<Shirt>();
                    _shirtsIndex[(shirt.Color.Id, shirt.Size.Id)] = index;
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
                    .Where(k => c.Id == k.Key.colorId)
                    .Where(w => !options.Sizes.Any() || options.Sizes.Any(a => a.Id == w.Key.sizeId))
                    .Where(w => !options.Colors.Any() || options.Colors.Any(a => a.Id == w.Key.colorId))
                    .Sum(s => s.Value.Count)
            }).ToList();

            var sizeCounts = Size.All.Select(c => new SizeCount
            {
                Size = c,
                Count = _shirtsIndex
                    .Where(k => c.Id == k.Key.sizeId)
                    .Where(w => !options.Sizes.Any() || options.Sizes.Any(a => a.Id == w.Key.sizeId))
                    .Where(w => !options.Colors.Any() || options.Colors.Any(a => a.Id == w.Key.colorId))
                    .Sum(s => s.Value.Count)
            }).ToList();

            var shirts = _shirtsIndex
                .Where(w => !options.Sizes.Any() || options.Sizes.Any(a => a.Id == w.Key.sizeId))
                .Where(w => !options.Colors.Any() || options.Colors.Any(a => a.Id == w.Key.colorId))
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