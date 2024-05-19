using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto
{
    public class OrderEntry
    {
       public async Task<List<OrderEntry>> GetByFilter(FilterOrderEntryDto filterDto, int page, int pagesize, string sortColumn, string sortOrder)
    {
        IQueryable<OrderEntry> query = createFilterQuery(filterDto);
        query = addOrderLogicToQuery(query, page, pagesize, sortColumn, sortOrder);
        List<OrderEntry> orderEntries = await query.ToListAsync();
        return orderEntries;
    }
    }
}