using Newtonsoft.Json;
using Nop.Plugin.Misc.CacheAnalytics.Models;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace Nop.Plugin.Misc.CacheAnalytics.Controllers
{
    [AdminAuthorize]
    public class MiscCacheAnalyticsController : BaseController
    {
        public MiscCacheAnalyticsController()
        {

        }

        #region Configure

        [HttpGet]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            return View("Nop.Plugin.Misc.CacheAnalytics.Views.MiscCacheAnalytics.Configure");
        }

        #endregion

        #region Memory Cache

        private IEnumerable<CacheItemModel> GetCacheItems()
        {
            var list = new List<CacheItemModel>();

            var cache = MemoryCache.Default;

            var storesField = typeof(MemoryCache).GetField("_stores", BindingFlags.NonPublic | BindingFlags.Instance);

            if (storesField != null)
            {
                var stores = (object[])storesField.GetValue(cache);

                if (stores != null)
                {
                    // MemoryCacheStore
                    foreach (var store in stores)
                    {
                        var entriesField = store.GetType().GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance);

                        if (entriesField != null)
                        {                            
                            var entries = (Hashtable)entriesField.GetValue(store);

                            if (entries != null)
                            {
                                foreach (var entry in entries.Cast<DictionaryEntry>().ToList())
                                {
                                    var cacheItem = new CacheItemModel();

                                    // MemoryCacheKey
                                    var keyProp = entry.Key.GetType().GetProperty("Key", BindingFlags.NonPublic | BindingFlags.Instance);

                                    if (keyProp != null)
                                    {
                                        var entryKey = keyProp.GetValue(entry.Value);

                                        if (entryKey != null)
                                        {
                                            cacheItem.Key = entryKey.ToString();
                                        }
                                    }

                                    // MemoryCacheEntry
                                    var valueProp = entry.Value.GetType().GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Instance);

                                    if (valueProp != null)
                                    {
                                        var entryValue = valueProp.GetValue(entry.Value);

                                        if (entryValue != null)
                                        {
                                            // type
                                            if (entryValue is IDictionary)
                                            {
                                                cacheItem.Type = "Dictionary";
                                                cacheItem.Count = ((IDictionary)entryValue).Count;
                                            }
                                            else if (entryValue is IList)
                                            {
                                                cacheItem.Type = "List";
                                                cacheItem.Count = ((IList)entryValue).Count;
                                            }
                                            else
                                            {
                                                cacheItem.Type = entryValue.GetType().Name;
                                            }

                                            // value
                                            cacheItem.Value = JsonConvert.SerializeObject(entryValue, Formatting.Indented);

                                            // size
                                            int size = 0;

                                            if (entryValue is IEnumerable)
                                            {
                                                foreach (var item in ((IEnumerable)entryValue))
                                                {
                                                    size += GetObjectSize(item);
                                                }
                                            }
                                            else
                                            {
                                                size = GetObjectSize(entryValue);
                                            }

                                            cacheItem.Size = size;
                                        }
                                    }

                                    var utcAbsExpProp = entry.Value.GetType().GetProperty("UtcAbsExp", BindingFlags.NonPublic | BindingFlags.Instance);

                                    if (utcAbsExpProp != null)
                                    {
                                        var entryExpiry = utcAbsExpProp.GetValue(entry.Value);

                                        if (entryExpiry != null)
                                        {
                                            cacheItem.ExpiryDate = (DateTime)entryExpiry;
                                        }
                                    }

                                    if (cacheItem.Key != null && cacheItem.Value != null)
                                    {
                                        list.Add(cacheItem);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return list;
        }

        private int GetObjectSize(object obj)
        {
            return (int)typeof(Earlz.BareMetal.BareMetal)
                                .GetMethod("SizeOf")
                                .MakeGenericMethod(obj.GetType())
                                .Invoke(obj, null);
        }

        private void RemoveCacheItem(string key)
        {
            var cache = MemoryCache.Default;

            if (cache.Contains(key))
            {
                cache.Remove(key);
            }
        }

        #endregion

        #region Cache Items

        [HttpPost]
        public ActionResult CacheItems(DataSourceRequest command)
        {
            var cacheItems = GetCacheItems();

            var model = new DataSourceResult()
            {
                Data = cacheItems
                            .OrderByDescending(c => c.Size)
                            .Skip((command.Page - 1) * command.PageSize)
                            .Take(command.PageSize),
                Total = cacheItems.Count(),
                ExtraData = cacheItems.Sum(c => c.Size)
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [HttpPost]
        public ActionResult DeleteCacheItem(string Key)
        {
            RemoveCacheItem(Key);         

            return new NullJsonResult();
        }

        #endregion
    }
}
