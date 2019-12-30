namespace Data.Controllers
{
    using Data.Models;
    using Microsoft.AspNet.OData;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class LimitStringDenormalizedsController : ODataController
    {
        private DataContext _context;

        //public LimitStringDenormalizedsController()
        //    => _context = new DataContext();

        public LimitStringDenormalizedsController(DataContext context)
            => _context = context ?? throw new ArgumentNullException(nameof(context));

        // GET: odata/Groups2
        [EnableQuery]
        public IQueryable<LimitStringDenormalized> GetLimitStringDenormalizeds()
            => _context.LimitStringDenormalized;

        // GET: odata/Groups2(5)
        [EnableQuery]
        public SingleResult<LimitStringDenormalized> GetLimitStringDenormalizedsV4([FromODataUri] Guid key)
            => SingleResult.Create(_context.LimitStringDenormalized
                .Where(LimitStringDenormalized => LimitStringDenormalized.GroupId == key));

        //// PUT: odata/Groups2(5)
        //public async Task<IActionResult> Put([FromODataUri] Guid key, LimitStringDenormalized update)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    if (key != update.GroupId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(update).State = EntityState.Modified;
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!LimitStringDenormalizedExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //    return Updated(update);
        //}

        // POST: odata/Groups2
        public async Task<IActionResult> Post([FromBody] LimitStringDenormalized limitStringDenormalized)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.LimitStringDenormalized.Add(limitStringDenormalized);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LimitDenormalizedExists(limitStringDenormalized.GroupId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(limitStringDenormalized);
        }

        //// PATCH: odata/Groups2(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public async Task<IActionResult> Patch([FromODataUri] Guid key, Delta<LimitStringDenormalized> patch)
        //{
        //    //Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var limitStringDenormalized = await _context.LimitStringDenormalizeds.FindAsync(key);
        //    if (limitStringDenormalized == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Patch(limitStringDenormalized);

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!LimitStringDenormalizedExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(limitStringDenormalized);
        //}

        //// DELETE: odata/Groups2(5)
        //public async Task<IActionResult> Delete([FromODataUri] Guid key)
        //{
        //    var limitStringDenormalized = await _context.LimitStringDenormalizeds.FindAsync(key);
        //    if (limitStringDenormalized == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.LimitStringDenormalizeds.Remove(limitStringDenormalized);
        //    await _context.SaveChangesAsync();

        //    return StatusCode((int)HttpStatusCode.NoContent);
        //}

        // GET: odata/LimitStringDenormalizedsV4(5)/DataDecimal
        [EnableQuery]
        public SingleResult<DataString> GetDataString([FromODataUri] Guid key)
            => SingleResult.Create(_context.LimitStringDenormalized
                .Where(m => m.GroupId == key).Select(e => e.Data));

        private bool LimitDenormalizedExists(Guid key)
            => _context.LimitStringDenormalized.Any(e => e.GroupId == key);
    }
}
