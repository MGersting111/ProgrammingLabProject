using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Data;
using api.Models;


namespace api.Repository
{
    public class TotalNumberRepository : ITotalNumberRepository
    {
        private readonly ApplicationDBContext _context;
       



        public TotalNumberRepository(ApplicationDBContext context)
        {
            _context = context;
            

        }

        public double GetTotalStoreRevenue(string storeId)
        {
            var orders = _context.Orders.Where(order => order.StoreId == storeId);
            return orders.Sum(order => order.total);
        }

        public Dictionary<string, double> GetFilteredStoreRevenues(FilterRevenue filter)
        {
            var storeRevenues = new Dictionary<string, double>();

            var stores = _context.Stores.ToList();
            foreach (var store in stores)
            {
                var orders = _context.Orders.Where(order => order.StoreId == store.StoreId);
                double storeRevenue = orders.Sum(order => order.total);
                storeRevenues.Add(store.StoreId, storeRevenue);
            }

            if (!string.IsNullOrEmpty(filter.StoreId))
            {
                storeRevenues = storeRevenues.Where(pair => pair.Key == filter.StoreId).ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            if (filter.StoreRevenues.HasValue)
            {
                storeRevenues = storeRevenues.Where(pair => pair.Value >= filter.StoreRevenues.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            return storeRevenues;
        }


        // private IQueryable<> createFilterQuery(FilterRevenue filter)
        // {
        //   IQueryable<> filterQuery = _totalNumberRepository.storeRevenues.Where(entry =>
        //   (filter.storeId == null || entry.StoreId == filter.storeId) &&
        //   (filter.storeRevenues == null || entry.storeRevenues == filter.storeRevenues)
        //   );
        //   return filterQuery;
        //  }
    }
}