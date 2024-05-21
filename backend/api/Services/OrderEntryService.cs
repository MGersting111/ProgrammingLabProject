using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using api.Interfaces;
using api.Models;
using api.Data;
using api.Dto;
using api.Repository;

namespace api.Services
{
    public class OrderEntryService 

    {
         private readonly ApplicationDBContext _context;


    public OrderEntryService(ApplicationDBContext context )
    {
        _context = context;
    }

    public async Task<List<OrderEntryDto>> GetAllOrderEntryDtosByFilter(FilterOrderEntryDto filterDto, int page, int pageSize, string sortColumn, string sortOrder)
        {
            List<OrderEntry> orderEntriesByFilter = await _OrderRepository.GetByFilter(filterDto, page, pagesize, sortColumn, sortOrder);
            List<OrderEntryDto> orderEntryDtos = ConvertToDtoList(orderEntriesByFilter);
            return orderEntryDtos;
        }
    }
}