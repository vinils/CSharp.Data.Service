namespace Data.Controllers
{
    using Data.Models;
    using Microsoft.AspNet.OData;
    using System;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class DatasController : ODataController
    {
        private DataContext _context;

        public DatasController()
            => _context = new DataContext();

        public DatasController(DataContext context)
            => _context = context;

        // GET: odata/Datas
        [EnableQuery]
        public IQueryable<Data> Get()
            => _context.Data;

        // GET: odata/Datas(5)
        [EnableQuery]
        public SingleResult<Data> GetData([FromODataUri] Guid groupId, [FromODataUri] DateTime collectionDate)
            => SingleResult.Create(_context.Data
                .Where(m => m.GroupId == groupId && m.CollectionDate == collectionDate));

        //// PUT: odata/Datas(5)
        //public async Task<IActionResult> Put([FromODataUri] Guid groupId, [FromODataUri] DateTime collectionDate, Delta<Data> patch)
        //{
        //    //Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    Data data = await _context.Datas.FindAsync(groupId, collectionDate);
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

        [HttpPost]
        public IHttpActionResult BulkInsert(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var datas = parameters["Datas"] as System.Collections.Generic.IEnumerable<Data>;

            if (!datas.Any())
                return Ok(new Data[] { });

            var datasDistinct = datas
                .GroupBy(e => new { e.CollectionDate, e.GroupId })
                .Select(eG => eG.First())
                .ToList();

            var dates = datasDistinct.GroupBy(e => e.CollectionDate).Select(e=> e.Key);
            var minData = dates.Min();
            var maxData = dates.Max();

            var dbData = _context.DataDecimal
                .Where(e=> e.CollectionDate >= minData && e.CollectionDate <= maxData)
                .ToList();

            var missingRecords = datasDistinct
                .Where(d => !dbData.Any(e => 
                        e.CollectionDate == d.CollectionDate 
                        && e.GroupId == d.GroupId))
                .ToList();

            _context.Data.AddRange(missingRecords);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return Ok(missingRecords);
        }

        public async Task<IHttpActionResult> Post(Data data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Data.Add(data);

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

        //// PATCH: odata/Datas(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public async Task<IActionResult> Patch([FromODataUri] Guid groupId, [FromODataUri] DateTime collectionDate, Delta<Data> patch)
        //{
        //    //Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var data = await _context.Datas.FindAsync(groupId, collectionDate);
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

        //// DELETE: odata/Datas(5)
        //public async Task<IActionResult> Delete([FromODataUri] Guid groupId, [FromODataUri] DateTime collectionDate)
        //{
        //    var data = await _context.Datas.FindAsync(groupId, collectionDate);
        //    if (data == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Datas.Remove(data);
        //    await _context.SaveChangesAsync();

        //    return StatusCode((int)HttpStatusCode.NoContent);
        //}

        // GET: odata/Datas(5)/Group
        [EnableQuery]
        public SingleResult<Group> GetGroup([FromODataUri] Guid groupId, [FromODataUri] DateTime collectionDate)
            => SingleResult.Create(_context.Data
                .Where(m => m.GroupId == groupId && m.CollectionDate == collectionDate).Select(e => e.Group));

        private bool DataExists(Guid groupId, DateTime collectionDate)
            => _context.Data
            .Any(e => e.GroupId == groupId && e.CollectionDate == collectionDate);
    }
}
