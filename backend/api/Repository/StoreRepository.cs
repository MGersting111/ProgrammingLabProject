using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Models;
using api.Data;

namespace api.Repository
{
    
    public class StoreRepository : IStoreRepository
{
    private readonly ApplicationDBContext _context;

    public StoreRepository(ApplicationDBContext context )
    {
        _context = context;
    }

        public List<Store> GetAllStores()
        {
            return _context.Stores.ToList();
        }

        public Store GetStoreByStoreId(string StoreId)
        {
            return _context.Stores.Find(StoreId);
        }

        public Store GetStoreByZipcode(int Zipcode)
        {
            return _context.Stores.FirstOrDefault(s => s.Zipcode == Zipcode);
        }

        public Store GetStoreByState_abbr(string StateAbbr)
        {
            return _context.Stores.FirstOrDefault(s => s.State_abbr == StateAbbr);
        }

        public Store GetStoreByLatitude(double Latitude)
        {
            return _context.Stores.FirstOrDefault(s => s.Latitude == Latitude);
        }

         public Store GetStoreByLongitude(double Longitude)
        {
            return _context.Stores.FirstOrDefault(s => s.Longitude == Longitude);
        }

          public Store GetStoreByCity(string City)
        {
            return _context.Stores.FirstOrDefault(s => s.City == City);
        }

          public Store GetStoreByState(string State)
        {
            return _context.Stores.FirstOrDefault(s => s.State == State);
        }

          public Store GetStoreByDistance(double Distance)
        {
            return _context.Stores.FirstOrDefault(s => s.Distance == Distance);
        }
}
}