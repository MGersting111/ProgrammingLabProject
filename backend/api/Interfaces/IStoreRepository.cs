using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Data;


namespace api.Interfaces
{
    public interface IStoreRepository
{
    List<Store> GetAllStores();
    Store GetStoreByStoreId(string StoreId);
    Store GetStoreByZipcode(int Zipcode);
    Store GetStoreByState_abbr(string StateAbbr);
    Store GetStoreByLatitude(double Latitude);

    Store GetStoreByCity(string City);

    Store GetStoreByState(string State);

    Store GetStoreByLongitude(double Longitude);

    Store GetStoreByDistance(double Distance);
    
}


}