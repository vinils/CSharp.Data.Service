namespace Data.Controllers
{
    using Data.Models;
    using Microsoft.AspNet.OData;
    using System;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class LimitDecimalDenormalizedsController : ODataController
    {
        private DataContext _context;

        public LimitDecimalDenormalizedsController()
            => _context = new DataContext();

        public LimitDecimalDenormalizedsController(DataContext context)
            => _context = context;

        // GET: odata/Groups2
        [EnableQuery]
        public IQueryable<LimitDecimalDenormalized> GetLimitDecimalDenormalizeds()
            => _context.LimitDecimalDenormalized;

        // GET: odata/Groups2(5)
        [EnableQuery]
        public SingleResult<LimitDecimalDenormalized> GetLimitDecimalDenormalizeds([FromODataUri] Guid key)
            => SingleResult.Create(_context.LimitDecimalDenormalized
                .Where(LimitDecimalDenormalized => LimitDecimalDenormalized.GroupId == key));

        //// PUT: odata/Groups2(5)
        //public async Task<IActionResult> Put([FromODataUri] Guid key, LimitDecimalDenormalized update)
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
        //        if (!LimitDecimalDenormalizedExists(key))
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
        public async Task<IHttpActionResult> Post(LimitDecimalDenormalized LimitDecimalDenormalized)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.LimitDecimalDenormalized.Add(LimitDecimalDenormalized);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LimitDenormalizedExists(LimitDecimalDenormalized.GroupId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(LimitDecimalDenormalized);
        }

        //// PATCH: odata/Groups2(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public async Task<IActionResult> Patch([FromODataUri] Guid key, Delta<LimitDecimalDenormalized> patch)
        //{
        //    //Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    LimitDecimalDenormalized limitDecimalDenormalized = await _context.LimitDecimalDenormalizeds.FindAsync(key);
        //    if (limitDecimalDenormalized == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Patch(limitDecimalDenormalized);

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!LimitDecimalDenormalizedExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(limitDecimalDenormalized);
        //}

        //// DELETE: odata/Groups2(5)
        //public async Task<IActionResult> Delete([FromODataUri] Guid key)
        //{
        //    LimitDecimalDenormalized limitDecimalDenormalized = await _context.LimitDecimalDenormalizeds.FindAsync(key);
        //    if (limitDecimalDenormalized == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.LimitDecimalDenormalizeds.Remove(limitDecimalDenormalized);
        //    await _context.SaveChangesAsync();

        //    return StatusCode((int)HttpStatusCode.NoContent);
        //}

        // GET: odata/LimitDecimalDenormalizedsV4(5)/DataDecimal
        [EnableQuery]
        public SingleResult<DataDecimal> GetDataDecimal([FromODataUri] Guid key)
            => SingleResult.Create(_context.LimitDecimalDenormalized
                .Where(m => m.GroupId == key).Select(e => e.Data));

        private bool LimitDenormalizedExists(Guid key)
            => _context.LimitDecimalDenormalized.Any(e => e.GroupId == key);
    }
}
