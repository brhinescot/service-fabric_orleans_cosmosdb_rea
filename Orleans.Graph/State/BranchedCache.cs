#region Using Directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

#endregion

namespace Orleans.Graph.State
{
    public delegate (T1 firstKey, T2 secondKey, T3 thirdKey, T4 fourthKey, T5 fifthKey) BranchFunction<in TEntity, T1, T2, T3, T4, T5>(TEntity entity);
    public delegate (T1 firstKey, T2 secondKey, T3 thirdKey, T4 fourthKey) BranchFunction<in TEntity, T1, T2, T3, T4>(TEntity entity);
    public delegate (T1 firstKey, T2 secondKey, T3 thirdKey) BranchFunction<in TEntity, T1, T2, T3>(TEntity entity);
    public delegate (T1 firstKey, T2 secondKey) BranchFunction<in TEntity, T1, T2>(TEntity entity);
    
    public class BranchingCache<T1, T2, T3, T4, T5, TEntity> : IEnumerable<TEntity>
    {
        [CanBeNull] private readonly BranchFunction<TEntity, T1, T2, T3, T4, T5> branchFunction;

        public BranchingCache() { }

        public BranchingCache(BranchFunction<TEntity, T1, T2, T3, T4, T5> branchFunction)
        {
            this.branchFunction = branchFunction;
        }

        #region Member Fields

        private readonly Dictionary<T1, BranchingCache<T2, T3, T4, T5, TEntity>> m = new Dictionary<T1, BranchingCache<T2, T3, T4, T5, TEntity>>();

        #endregion
        
        public TEntity Get(T1 firstKey, T2 secondKey, T3 thirdKey, T4 fourthKey, T5 fifthKey)
        {
            return !m.TryGetValue(firstKey, out var level1) ? default(TEntity) : level1.Get(secondKey, thirdKey, fourthKey, fifthKey);
        }

        public IEnumerable<TEntity> GetSubset(T1 firstKey, T2 secondKey, T3 thirdKey, T4 fourthKey)
        {
            return !m.TryGetValue(firstKey, out var level1) ? new List<TEntity>() : level1.GetSubset(secondKey, thirdKey, fourthKey);
        }

        public IEnumerable<TEntity> GetSubset(T1 firstKey, T2 secondKey, T3 thirdKey)
        {
            return !m.TryGetValue(firstKey, out var level1) ? new List<TEntity>() : level1.GetSubset(secondKey, thirdKey);
        }

        public IEnumerable<TEntity> GetSubset(T1 firstKey, T2 secondKey)
        {
            return !m.TryGetValue(firstKey, out var level1) ? new List<TEntity>() : level1.GetSubset(secondKey);
        }

        public IEnumerable<TEntity> GetSubset(T1 firstKey)
        {
            return !m.TryGetValue(firstKey, out var level1) ? new List<TEntity>() : level1.GetAll();
        }

        public IEnumerable<TEntity> Merge(T5 fifthKey)
        {
            return m.SelectMany(pair => pair.Value.Merge(fifthKey));
        }

        public IEnumerable<TEntity> GetAll()
        {
            return m.SelectMany(first => first.Value.GetAll());
        }

        public bool Set(T1 firstKey, T2 secondKey, T3 thirdKey, T4 fourthKey, T5 fifthKey, TEntity entity)
        {
            if (m.ContainsKey(firstKey))
                return m[firstKey].Set(secondKey, thirdKey, fourthKey, fifthKey, entity);

            m.Add(firstKey, new BranchingCache<T2, T3, T4, T5, TEntity> { { secondKey, thirdKey, fourthKey, fifthKey, entity } });
            return false;
        }

        public bool Set(TEntity entity)
        {
            if(branchFunction == null)
                throw new InvalidOperationException($"The {nameof(branchFunction)} has not been set.");

            (T1 firstKey, T2 secondKey, T3 thirdKey, T4 fourthKey, T5 fifthKey) = branchFunction(entity);
            return Set(firstKey, secondKey, thirdKey, fourthKey, fifthKey, entity);
        }

        public bool Remove(T1 firstKey, T2 secondKey, T3 thirdKey, T4 fourthKey, T5 fifthKey)
        {
            return m.TryGetValue(firstKey, out var level1)
                   && level1.Remove(secondKey, thirdKey, fourthKey, fifthKey);
        }

        public bool Prune(T1 firstKey, T2 secondKey, T3 thirdKey, T4 fourthKey)
        {
            return m.TryGetValue(firstKey, out var level1)
                   && level1.Prune(secondKey, thirdKey, fourthKey);
        }

        public bool Prune(T1 firstKey, T2 secondKey, T3 thirdKey)
        {
            return m.TryGetValue(firstKey, out var level1)
                   && level1.Prune(secondKey, thirdKey);
        }

        public bool Prune(T1 firstKey, T2 secondKey)
        {
            return m.TryGetValue(firstKey, out var target) && target.Prune(secondKey);
        }

        public bool Prune(T1 firstKey)
        {
            return m.Remove(firstKey);
        }

        public void Clear()
        {
            m.Clear();
        }

        internal void Add(T1 firstKey, T2 secondKey, T3 thirdKey, T4 fourthKey, T5 fifthKey, TEntity entity)
        {
            Set(firstKey, secondKey, thirdKey, fourthKey, fifthKey, entity);
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return GetAll().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    
    public class BranchingCache<T1, T2, T3, T4, TEntity> : IEnumerable<TEntity>
    {
        private readonly BranchFunction<TEntity, T1, T2, T3, T4> branchFunction;
        public BranchingCache() { }
        public BranchingCache(BranchFunction<TEntity, T1, T2, T3, T4> branchFunction)
        {
            this.branchFunction = branchFunction;
        }
        #region Member Fields

        private readonly Dictionary<T1, BranchingCache<T2, T3, T4, TEntity>> m = new Dictionary<T1, BranchingCache<T2, T3, T4, TEntity>>();

        #endregion

        public TEntity Get(T1 firstKey, T2 secondKey, T3 thirdKey, T4 fourthKey)
        {
            return !m.TryGetValue(firstKey, out var level1) ? default(TEntity) : level1.Get(secondKey, thirdKey, fourthKey);
        }

        public IEnumerable<TEntity> GetSubset(T1 firstKey, T2 secondKey, T3 thirdKey)
        {
            return !m.TryGetValue(firstKey, out var level1) ? new List<TEntity>() : level1.GetSubset(secondKey, thirdKey);
        }

        public IEnumerable<TEntity> GetSubset(T1 firstKey, T2 secondKey)
        {
            return !m.TryGetValue(firstKey, out var level1) ? new List<TEntity>() : level1.GetSubset(secondKey);
        }

        public IEnumerable<TEntity> GetSubset(T1 firstKey)
        {
            return !m.TryGetValue(firstKey, out var level1) ? new List<TEntity>() : level1.GetAll();
        }
        
        public IEnumerable<TEntity> Merge(T4 fourthKey)
        {
            return m.SelectMany(pair => pair.Value.Merge(fourthKey));
        }

        public IEnumerable<TEntity> GetAll()
        {
            return m.SelectMany(first => first.Value.GetAll());
        }

        public bool Set(T1 firstKey, T2 secondKey, T3 thirdKey, T4 fourthKey, TEntity entity)
        {
            if (m.ContainsKey(firstKey))
                return m[firstKey].Set(secondKey, thirdKey, fourthKey, entity);

            m.Add(firstKey, new BranchingCache<T2, T3, T4, TEntity> {{secondKey, thirdKey, fourthKey, entity}});
            return false;
        }

        public bool Set(TEntity entity)
        {
            (T1 firstKey, T2 secondKey, T3 thirdKey, T4 fourthKey) = branchFunction(entity);
            return Set(firstKey, secondKey, thirdKey, fourthKey, entity);
        }

        public bool Remove(T1 firstKey, T2 secondKey, T3 thirdKey, T4 fourthKey)
        {
            return m.TryGetValue(firstKey, out var level1)
                   && level1.Remove(secondKey, thirdKey, fourthKey);
        }

        public bool Prune(T1 firstKey, T2 secondKey, T3 thirdKey)
        {
            return m.TryGetValue(firstKey, out var level1)
                   && level1.Prune(secondKey, thirdKey);
        }

        public bool Prune(T1 firstKey, T2 secondKey)
        {
            return m.TryGetValue(firstKey, out var target) && target.Prune(secondKey);
        }

        public bool Prune(T1 firstKey)
        {
            return m.Remove(firstKey);
        }

        public void Clear()
        {
            m.Clear();
        }

        internal void Add(T1 firstKey, T2 secondKey, T3 thirdKey, T4 fourthKey, TEntity entity)
        {
            Set(firstKey, secondKey, thirdKey, fourthKey, entity);
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return GetAll().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class BranchingCache<T1, T2, T3, TEntity> : IEnumerable<TEntity>
    {
        private readonly BranchFunction<TEntity, T1, T2, T3> branchFunction;
        
        public BranchingCache() { }
        
        public BranchingCache(BranchFunction<TEntity, T1, T2, T3> branchFunction)
        {
            this.branchFunction = branchFunction;
        }
        
        private readonly Dictionary<T1, BranchedCache<T2, T3, TEntity>> m = new Dictionary<T1, BranchedCache<T2, T3, TEntity>>();

        public TEntity Get(T1 firstKey, T2 secondKey, T3 thirdKey)
        {
            return !m.TryGetValue(firstKey, out var level1) ? default(TEntity) : level1.Get(secondKey, thirdKey);
        }

        public IEnumerable<TEntity> GetSubset(T1 firstKey, T2 secondKey)
        {
            return !m.TryGetValue(firstKey, out var level1) ? new List<TEntity>() : level1.GetSubset(secondKey);
        }

        public IEnumerable<TEntity> GetSubset(T1 firstKey)
        {
            return !m.TryGetValue(firstKey, out var level1) ? new List<TEntity>() : level1.GetAll();
        }

        public IEnumerable<TEntity> Merge(T3 thirdKey)
        {
            return m.SelectMany(pair => pair.Value.Merge(thirdKey));
        }

        public IEnumerable<TEntity> GetAll()
        {
            return m.SelectMany(first => first.Value.GetAll());
        }

        public bool Set(T1 firstKey, T2 secondKey, T3 thirdKey, TEntity entity)
        {
            if (m.ContainsKey(firstKey))
                return m[firstKey].Set(secondKey, thirdKey, entity);
            
            m.Add(firstKey, new BranchedCache<T2, T3, TEntity>{{secondKey, thirdKey, entity}});
            return false;
        }
        
        public bool Set(TEntity entity)
        {
            (T1 firstKey, T2 secondKey, T3 thirdKey) = branchFunction(entity);
            return Set(firstKey, secondKey, thirdKey, entity);
        }

        public bool Remove(T1 firstKey, T2 secondKey, T3 thirdKey)
        {
            return m.TryGetValue(firstKey, out var level1)
                   && level1.Remove(secondKey, thirdKey);
        }

        public bool Prune(T1 firstKey, T2 secondKey)
        {
            return m.TryGetValue(firstKey, out var target) && target.Prune(secondKey);
        }

        public bool Prune(T1 firstKey)
        {
            return m.Remove(firstKey);
        }
        
        public void Clear()
        {
            m.Clear();
        }
        
        internal void Add(T1 firstKey, T2 secondKey, T3 thirdKey, TEntity entity)
        {
            Set(firstKey, secondKey, thirdKey, entity);
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return GetAll().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class BranchedCache<T1, T2, TEntity> : IEnumerable<TEntity>
    {
        #region Member Fields

        [CanBeNull] private readonly BranchFunction<TEntity, T1, T2> branchFunction;
        private readonly Dictionary<T1, Dictionary<T2, TEntity>> m = new Dictionary<T1, Dictionary<T2, TEntity>>();

        #endregion

        public BranchedCache() { }

        public BranchedCache(BranchFunction<TEntity, T1, T2> branchFunction)
        {
            this.branchFunction = branchFunction;
        }

        public TEntity Get(T1 firstKey, T2 secondKey)
        {
            if (!m.TryGetValue(firstKey, out var level1)) return default(TEntity);
            return level1.TryGetValue(secondKey, out TEntity entity) ? entity : default(TEntity);
        }

        public IEnumerable<TEntity> GetSubset(T1 firstKey)
        {
            return m.TryGetValue(firstKey, out var target)
                ? target.Select(pair => pair.Value)
                : new List<TEntity>();
        }

        public IEnumerable<TEntity> Merge(T2 secondKey)
        {
            return m.Select(pair => pair.Value.TryGetValue(secondKey, out TEntity entity) ? entity : default(TEntity))
                .TakeWhile(entity => entity != null);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return m
                .SelectMany(first => first.Value
                    .Select(second => second.Value));
        }

        public bool Set(T1 firstKey, T2 secondKey, TEntity entity)
        {
            if (m.ContainsKey(firstKey))
            {
                var level1 = m[firstKey];
                if (level1.ContainsKey(secondKey))
                {
                    level1[secondKey] = entity;
                    return true;
                }
                level1.Add(secondKey, entity);
                return false;
            }
            m.Add(firstKey, new Dictionary<T2, TEntity> {{secondKey, entity}});
            return false;
        }

        public bool Set(TEntity entity)
        {
            (T1 firstKey, T2 secondKey) = branchFunction(entity);
            return Set(firstKey, secondKey, entity);
        }

        public bool Remove(T1 firstKey, T2 secondKey)
        {
            return m.TryGetValue(firstKey, out var level1)
                   && level1.Remove(secondKey);
        }

        public bool Prune(T1 firstKey)
        {
            return m.Remove(firstKey);
        }

        public void Clear()
        {
            m.Clear();
        }

        internal void Add(T1 firstKey, T2 secondKey, TEntity entity)
        {
            Set(firstKey, secondKey, entity);
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return GetAll().GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}