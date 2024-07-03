using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class CacheEntry
    {
        public string CacheKey { get; set; }
        public string JsonValue { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}