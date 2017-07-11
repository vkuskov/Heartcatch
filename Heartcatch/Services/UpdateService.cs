using System.Collections.Generic;

namespace Heartcatch.Services
{
    public sealed class UpdateService : IUpdateService
    {
        private readonly List<Data> toSubscribe = new List<Data>();

        private readonly DataSorter sorter = new DataSorter();
        private readonly List<IUpdateable> toRemove = new List<IUpdateable>();

        private readonly List<Data> updateables = new List<Data>();

        public void Subscribe(IUpdateable updateable, int priority)
        {
            toSubscribe.Add(new Data
            {
                Priorirty = priority,
                Updateable = updateable
            });
        }

        public void Unsubscribe(IUpdateable updateable)
        {
            toRemove.Add(updateable);
        }

        public void Update()
        {
            if (toSubscribe.Count > 0)
            {
                foreach (var it in toSubscribe)
                    updateables.Add(it);
                toSubscribe.Clear();
                updateables.Sort(sorter);
            }
            foreach (var it in updateables)
                it.Updateable.OnUpdate();
            foreach (var it in toRemove)
                updateables.RemoveAll(updateable => updateable.Updateable == it);
        }

        private struct Data
        {
            public int Priorirty;
            public IUpdateable Updateable;
        }

        private class DataSorter : IComparer<Data>
        {
            public int Compare(Data x, Data y)
            {
                return x.Priorirty - y.Priorirty;
            }
        }
    }
}