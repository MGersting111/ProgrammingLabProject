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

            if(StoreId == null)
            {
                return NotFound();
            }

            return Ok(StoreId);
        }

        [HttpGet("Zipcode")]
        public IActionResult GetStoreByZipcode(int Zipcode)
        {
            var store = _storeRepository.GetStoreByZipcode(Zipcode);

            if(Zipcode == null)
            {
                return NotFound();
            }

            return Ok(Zipcode);
        }

        [HttpGet("State_abbr")]
        public IActionResult GetStoreByState_abbr(string State_abbr)
        {
            var store = _storeRepository.GetStoreByState_abbr(State_abbr);

            if(State_abbr == null)
            {
                return NotFound();
            }

            return Ok(State_abbr);
        }

        [HttpGet("Latitude")]
        public IActionResult GetStoreByLatitude(double Latitude)
        {
            var store = _storeRepository.GetStoreByLatitude(Latitude);

            if(Latitude == null)
            {
                return NotFound();
            }

            return Ok(Latitude);
        }

        [HttpGet("Longitude")]
        public IActionResult GetStoreByLongitude (int Longitude)
        {
            var store = _storeRepository.GetStoreByLongitude(Longitude);

            if(Longitude == null)
            {
                return NotFound();
            }

            return Ok(Longitude);
        }

        [HttpGet("City")]
        public IActionResult GetStoreByCity(string City)
        {
            var store = _storeRepository.GetStoreByCity(City);

            if(City == null)
            {
                return NotFound();
            }

            return Ok(City);
        }

        [HttpGet("State")]
        public IActionResult GetStoreByState(string State)
        {
            var store = _storeRepository.GetStoreByState(State);

            if(State== null)
            {
                return NotFound();
            }

            return Ok(State);
        }

        [HttpGet("Distance")]
        public IActionResult GetStoreByDistance(double Distance)
        {
            var store = _storeRepository.GetStoreByDistance(Distance);

            if(Distance == null)
            {
                return NotFound();
            }

            return Ok(Distance);
        }
    }
}