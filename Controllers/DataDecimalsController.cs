namespace Data.Controllers
{
    using Microsoft.AspNet.OData;
    using Data.Models;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class DataDecimalsController : ODataController
    {
        private readonly DataContext _context;

        //public DataDecimalsController()
        //    => _context = new DataContext();

        public DataDecimalsController(DataContext context)
            => _context = context ?? throw new ArgumentNullException(nameof(context));

        // GET: odata/DataDecimals
        [EnableQuery]
        public IQueryable<DataDecimal> Get()
            => _context.DataDecimal;

        // GET: odata/DataDecimals(5)
        [EnableQuery]
        public SingleResult<DataDecimal> GetData([FromODataUri] Guid groupId, [FromODataUri] DateTime collectionDate)
            => SingleResult.Create(_context.DataDecimal
                .Where(m => m.GroupId == groupId && m.CollectionDate == collectionDate));

        //// PUT: odata/DataDecimals(5)
        //public async Task<IActionResult> Put([FromODataUri] Guid groupId, [FromODataUri] DateTime collectionDate, Delta<DataDecimal> patch)
        //{
        //    //Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    DataDecimal data = await _context.DataDecimals.FindAsync(groupId, collectionDate);
        //    if (data == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Put(data);

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!DataExists(groupId, collectionDate))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(data);
        //}

        public async Task<IActionResult> Post([FromBody] DataDecimal data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.DataDecimal.Add(data);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DataExists(data.GroupId, data.CollectionDate))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(data);
        }

        //// PATCH: odata/DataDecimals(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public async Task<IActionResult> Patch([FromODataUri] Guid groupId, [FromODataUri] DateTime collectionDate, Delta<DataDecimal> patch)
        //{
        //    //Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    DataDecimal data = await _context.DataDecimals.FindAsync(groupId, collectionDate);
        //    if (data == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Patch(data);

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!DataExists(groupId, collectionDate))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(data);
        //}

        //// DELETE: odata/DataDecimals(5)
        //public async Task<IActionResult> Delete([FromODataUri] Guid groupId, [FromODataUri] DateTime collectionDate)
        //{
        //    DataDecimal data = await _context.DataDecimals.FindAsync(groupId, collectionDate);
        //    if (data == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.DataDecimals.Remove(data);
        //    await _context.SaveChangesAsync();

        //    return StatusCode((int) HttpStatusCode.NoContent);
        //}

        // GET: odata/DataDecimals(5)/Group
        [EnableQuery]
        public SingleResult<Group> GetGroup([FromODataUri] Guid groupId, [FromODataUri] DateTime collectionDate)
            => SingleResult.Create(_context.DataDecimal
                .Where(m => m.GroupId == groupId && m.CollectionDate == collectionDate).Select(e=> e.Group));

        private bool DataExists(Guid groupId, DateTime collectionDate)
            => _context.DataDecimal
            .Any(e => e.GroupId == groupId && e.CollectionDate == collectionDate);

        public IActionResult DecimalTotal(Guid? groupId, DateTime? startDate, DateTime? endDate, decimal? startValue, decimal? endValue)
        {
            if (!groupId.HasValue)
                return BadRequest();

            var qry = _context.DataDecimal
                .Where(dec => dec.GroupId == groupId
                      && (!startDate.HasValue || dec.CollectionDate >= startDate.Value) && (!endDate.HasValue || dec.CollectionDate < endDate.Value)
                      && (!startValue.HasValue || dec.DecimalValue >= startValue.Value) && (!endValue.HasValue || dec.DecimalValue < endValue.Value))
                .GroupBy(dec => dec.GroupId)
                .Select(dec => new { GroupId = dec.Key, Sum = dec.Sum(e => e.DecimalValue), Count = dec.Count() });

            var list = qry.ToList();
            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(qry);
            return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(qry));
        }
    }
}
