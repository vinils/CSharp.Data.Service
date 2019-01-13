namespace Data.Controllers
{
    using Data.Models;
    using Microsoft.AspNet.OData;
    using System;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class ExamsController : ODataController
    {
        private DataContext _context;

        public ExamsController()
            => _context = new DataContext();

        public ExamsController(DataContext context)
            => _context = context;

        // GET: odata/Exams
        [EnableQuery]
        public IQueryable<Exam> Get()
            => _context.Exam;

        // GET: odata/Exams(5)
        [EnableQuery]
        public SingleResult<Exam> GetExam([FromODataUri] Guid groupId, [FromODataUri] DateTime collectionDate)
            => SingleResult.Create(_context.Exam
                .Where(m => m.GroupId == groupId && m.CollectionDate == collectionDate));

        //// PUT: odata/Exams(5)
        //public async Task<IActionResult> Put([FromODataUri] Guid groupId, [FromODataUri] DateTime collectionDate, Delta<Exam> patch)
        //{
        //    //Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    Exam exam = await _context.Exams.FindAsync(groupId, collectionDate);
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

        [HttpPost]
        public async Task<IHttpActionResult> BulkInsert(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exams = parameters["Exams"] as System.Collections.Generic.IEnumerable<Exam>;

            var examsDistinct = exams
                .GroupBy(e => new { e.CollectionDate, e.GroupId })
                .Select(eG => eG.First())
                .ToList();

            var missingRecords = examsDistinct
                .Where(d => !_context.Exam.Any(e => e.CollectionDate == d.CollectionDate && e.GroupId == d.GroupId)).ToList();

            _context.Exam.AddRange(missingRecords);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            //return Created(exams);
            return Ok();
        }

        public async Task<IHttpActionResult> Post(Exam exam)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Exam.Add(exam);

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

        //// PATCH: odata/Exams(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public async Task<IActionResult> Patch([FromODataUri] Guid groupId, [FromODataUri] DateTime collectionDate, Delta<Exam> patch)
        //{
        //    //Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var exam = await _context.Exams.FindAsync(groupId, collectionDate);
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

        //// DELETE: odata/Exams(5)
        //public async Task<IActionResult> Delete([FromODataUri] Guid groupId, [FromODataUri] DateTime collectionDate)
        //{
        //    var exam = await _context.Exams.FindAsync(groupId, collectionDate);
        //    if (exam == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Exams.Remove(exam);
        //    await _context.SaveChangesAsync();

        //    return StatusCode((int)HttpStatusCode.NoContent);
        //}

        // GET: odata/Exams(5)/Group
        [EnableQuery]
        public SingleResult<Group> GetGroup([FromODataUri] Guid groupId, [FromODataUri] DateTime collectionDate)
            => SingleResult.Create(_context.Exam
                .Where(m => m.GroupId == groupId && m.CollectionDate == collectionDate).Select(e => e.Group));

        private bool ExamExists(Guid groupId, DateTime collectionDate)
            => _context.Exam
            .Any(e => e.GroupId == groupId && e.CollectionDate == collectionDate);
    }
}
