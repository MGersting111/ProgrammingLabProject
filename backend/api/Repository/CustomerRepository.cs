using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Models;
using api.Data;


namespace api.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDBContext _context;

        public CustomerRepository(ApplicationDBContext context )
        {
            _context = context;
        }
      
        public IEnumerable<Customer> GetCustomers()
        {
            return _context.Customers.Take(100).ToList();
        }

    public Customer GetCustomerById(string CustomerId)
    {
        return _context.Customers.Find(CustomerId);
    }

    public Customer GetCustomerByLatitude(double Latitude)
    {
        return _context.Customers.FirstOrDefault(c => c.Latitude == Latitude);
    }
    public Customer GetCustomerByLongitude(double Longitude)
    {
        return _context.Customers.FirstOrDefault(c => c.Longitude == Longitude);
    }
        
    }
}