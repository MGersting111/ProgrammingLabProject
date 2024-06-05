using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Data;

namespace api.Interfaces
{
    public interface ICustomerRepository
    {
            IEnumerable<Customer> GetCustomers();
            Customer GetCustomerById(string customerId);
            Customer GetCustomerByLatitude(double latitude); 
            Customer GetCustomerByLongitude(double longitude); 

    }
}  