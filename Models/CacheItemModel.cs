using System;

namespace Nop.Plugin.Misc.CacheAnalytics.Models
{
    public class CacheItemModel
    {
        public string Key { get; set; }

        public string Type { get; set; }

        public int? Count { get; set; }

        public string Value { get; set; }

        public int Size { get; set; }

        public DateTime ExpiryDate { get; set; }
    }
}
