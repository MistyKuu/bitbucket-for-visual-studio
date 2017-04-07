using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces;
using ReactiveUI;

namespace GitClientVS.Contracts
{
    public class PagedCollection<TData> : ReactiveList<TData>, ISupportIncrementalLoading
    {
        private readonly Func<int, int, Task<IEnumerable<TData>>> _loadTask;
        private readonly int _pageSize;

        public PagedCollection(Func<int, int, Task<IEnumerable<TData>>> loadTask, int pageSize)
        {
            _loadTask = loadTask;
            _pageSize = pageSize;
        }

        public async Task LoadNextPageAsync()
        {
            var pageNumber = (Count / _pageSize) + 1;

            var data = (await _loadTask(_pageSize, pageNumber)).ToList();
            int count = Count;
            foreach (var item in data.Skip(count % _pageSize))
                Add(item);
        }
    }
}