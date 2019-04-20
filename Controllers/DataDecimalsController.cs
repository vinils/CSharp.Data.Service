﻿namespace Data.Controllers
{
    using Microsoft.AspNet.OData;
    using Data.Models;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Data.Entity.Infrastructure;

    public class DataDecimalsController : ODataController
    {
        private DataContext _context;

        public DataDecimalsController()
            => _context = new DataContext();

        public DataDecimalsController(DataContext context)
            => _context = context;

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

        public async Task<IHttpActionResult> Post(DataDecimal data)
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
    }
}