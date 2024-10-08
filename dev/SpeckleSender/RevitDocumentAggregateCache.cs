using Autodesk.Revit.DB;
using RevitSharedResources.Interfaces;
using System;
using System.Collections.Generic;

namespace SpeckleSender
{
    public sealed class RevitDocumentAggregateCache : IRevitDocumentAggregateCache
    {
        private readonly Dictionary<Type, IRevitObjectCache> objectCaches;
        public Document Document { get; }

        public RevitDocumentAggregateCache(Document doc)
        {
            this.Document = doc;
            this.objectCaches = new Dictionary<Type, IRevitObjectCache>();
        }

        public IRevitObjectCache<T> GetOrInitializeEmptyCacheOfType<T>(out bool isExistingCache)
        {
            return GetOrInitializeCacheOfTypeNullable<T>(null, out isExistingCache);
        }

        public IRevitObjectCache<T> GetOrInitializeCacheOfType<T>(
          Action<IRevitObjectCache<T>> initializer,
          out bool isExistingCache
        )
        {
            return GetOrInitializeCacheOfTypeNullable<T>(initializer, out isExistingCache);
        }

        private IRevitObjectCache<T> GetOrInitializeCacheOfTypeNullable<T>(
          Action<IRevitObjectCache<T>> initializer,
          out bool isExistingCache
        )
        {
            if (!objectCaches.TryGetValue(typeof(T), out var singleCache))
            {
                isExistingCache = false;
                singleCache = new RevitObjectCache<T>(this);
                if (initializer != null)
                {
                    initializer((IRevitObjectCache<T>)singleCache);
                }
                objectCaches.Add(typeof(T), singleCache);
            }
            else
            {
                isExistingCache = true;
            }
            return (IRevitObjectCache<T>)singleCache;
        }

        public IRevitObjectCache<T> TryGetCacheOfType<T>()
        {
            if (!objectCaches.TryGetValue(typeof(T), out var singleCache))
            {
                return null;
            }
            return singleCache as IRevitObjectCache<T>;
        }

        public void Invalidate<T>()
        {
            objectCaches.Remove(typeof(T));
        }

        public void InvalidateAll()
        {
            objectCaches.Clear();
        }
    }
}
