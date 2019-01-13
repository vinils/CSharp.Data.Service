namespace Data.Controllers
{
    using Microsoft.AspNet.OData;
    using Data.Models;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Data.Entity.Infrastructure;

    public class ExamDecimalsController : ODataController
    {
        private DataContext _context;

        public ExamDecimalsController()
            => _context = new DataContext();

        public ExamDecimalsController(DataContext context)
            => _context = context;

        // GET: odata/ExamDecimals
        [EnableQuery]
        public IQueryable<ExamDecimal> Get()
            => _context.ExamDecimal;

        // GET: odata/ExamDecimals(5)
        [EnableQuery]
        public SingleResult<ExamDecimal> GetExam([FromODataUri] Guid groupId, [FromODataUri] DateTime collectionDate)
            => SingleResult.Create(_context.ExamDecimal
                .Where(m => m.GroupId == groupId && m.CollectionDate == collectionDate));

        //// PUT: odata/ExamDecimals(5)
        //public async Task<IActionResult> Put([FromODataUri] Guid groupId, [FromODataUri] DateTime collectionDate, Delta<ExamDecimal> patch)
        //{
        //    //Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    ExamDecimal exam = await _context.ExamDecimals.FindAsync(groupId, collectionDate);
        //    if (exam == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Put(exam);

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ExamExists(groupId, collectionDate))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(exam);
        //}

        public async Task<IHttpActionResult> Post(ExamDecimal exam)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ExamDecimal.Add(exam);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ExamExists(exam.GroupId, exam.CollectionDate))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(exam);
        }

        //// PATCH: odata/ExamDecimals(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public async Task<IActionResult> Patch([FromODataUri] Guid groupId, [FromODataUri] DateTime collectionDate, Delta<ExamDecimal> patch)
        //{
        //    //Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    ExamDecimal exam = await _context.ExamDecimals.FindAsync(groupId, collectionDate);
        //    if (exam == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Patch(exam);

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ExamExists(groupId, collectionDate))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(exam);
        //}

        //// DELETE: odata/ExamDecimals(5)
        //public async Task<IActionResult> Delete([FromODataUri] Guid groupId, [FromODataUri] DateTime collectionDate)
        //{
        //    ExamDecimal exam = await _context.ExamDecimals.FindAsync(groupId, collectionDate);
        //    if (exam == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.ExamDecimals.Remove(exam);
        //    await _context.SaveChangesAsync();

        //    return StatusCode((int) HttpStatusCode.NoContent);
        //}

        // GET: odata/ExamDecimals(5)/Group
        [EnableQuery]
        public SingleResult<Group> GetGroup([FromODataUri] Guid groupId, [FromODataUri] DateTime collectionDate)
            => SingleResult.Create(_context.ExamDecimal
                .Where(m => m.GroupId == groupId && m.CollectionDate == collectionDate).Select(e=> e.Group));

        private bool ExamExists(Guid groupId, DateTime collectionDate)
            => _context.ExamDecimal
            .Any(e => e.GroupId == groupId && e.CollectionDate == collectionDate);
    }
}
