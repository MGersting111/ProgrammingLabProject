using api.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using api.Models;
using MySql.Data.EntityFrameworkCore.Extensions;
using api.Interfaces;




namespace api.Controllers
{
    [Route("api/Models/Store/")]
    [ApiController]


    public class StoreController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IStoreRepository _storeRepository;
        public StoreController(ApplicationDBContext context, IStoreRepository storeRepository)
        {
            _context = context;
            _storeRepository = storeRepository;
        }

        [HttpGet]

        public IActionResult GetAll() 
        {
            var stores = _storeRepository.GetAllStores();

            return Ok(stores);
        }

        [HttpGet("StoreID")]

        public IActionResult GetStoreByStoreId(string StoreId)
        {
            var store = _storeRepository.GetStoreByStoreId(StoreId);

            if(store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        [HttpGet("Zipcode")]
        public IActionResult GetStoreByZipcode(int Zipcode)
        {
            var store = _storeRepository.GetStoreByZipcode(Zipcode);

            if(store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        [HttpGet("State_abbr")]
        public IActionResult GetStoreByState_abbr(string State_abbr)
        {
            var store = _storeRepository.GetStoreByState_abbr(State_abbr);

            if(store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        [HttpGet("Latitude")]
        public IActionResult GetStoreByLatitude(double Latitude)
        {
            var store = _storeRepository.GetStoreByLatitude(Latitude);

            if(store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        [HttpGet("Longitude")]
        public IActionResult GetStoreByLongitude (int Longitude)
        {
            var store = _storeRepository.GetStoreByLongitude(Longitude);

            if(store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        [HttpGet("City")]
        public IActionResult GetStoreByCity(string City)
        {
            var store = _storeRepository.GetStoreByCity(City);

            if(store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        [HttpGet("State")]
        public IActionResult GetStoreByState(string State)
        {
            var store = _storeRepository.GetStoreByState(State);

            if(store== null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        [HttpGet("Distance")]
        public IActionResult GetStoreByDistance(double Distance)
        {
            var store = _storeRepository.GetStoreByDistance(Distance);

            if(store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }
    }
}