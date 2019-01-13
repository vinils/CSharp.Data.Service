namespace Data.Controllers
{
    using Data.Models;
    using Microsoft.AspNet.OData;
    using System;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class GroupsController : ODataController
    {
        private DataContext _context;

        public GroupsController()
            => _context = new DataContext();

        public GroupsController(DataContext context)
            => _context = context;

        // GET: odata/Groups2
        [EnableQuery(MaxExpansionDepth = 10)]
        public IQueryable<Group> Get()
            => _context.Group;

        // GET: odata/Groups2(5)
        [EnableQuery]
        public SingleResult<Group> GetGroup([FromODataUri] Guid key)
            => SingleResult.Create(_context.Group.Where(group => group.Id == key));

        //// PUT: odata/Groups2(5)
        //public async Task<IActionResult> Put([FromODataUri] Guid key, Group update)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    _context.Entry(update).State = EntityState.Modified;
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!GroupExists(key))
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
        public async Task<IHttpActionResult> Post(Group group)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Group.Add(group);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (GroupExists(group.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(group);
        }

        //// PATCH: odata/Groups2(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public async Task<IActionResult> Patch([FromODataUri] Guid key, Delta<Group> patch)
        //{
        //    //Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var group = await _context.Groups.FindAsync(key);
        //    if (group == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Patch(group);

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!GroupExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(group);
        //}

        //// DELETE: odata/Groups2(5)
        //public async Task<IActionResult> Delete([FromODataUri] Guid key)
        //{
        //    var group = await _context.Groups.FindAsync(key);
        //    if (group == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Groups.Remove(group);
        //    await _context.SaveChangesAsync();

        //    return StatusCode((int) HttpStatusCode.NoContent);
        //}

        //// GET: odata/Groups2(5)/All
        //[EnableQuery]
        //public IActionResult GetAll([FromODataUri] Guid key)
        //    => Ok(_context.Groups.Where(m => m.Id == key).SelectMany(m => m.Childs));

        //[EnableQuery]
        //public IQueryable<Group> GetAllRecursively([FromODataUri] Guid key)
        //{
        //    var groups = db.Groups.ToList();
        //    var filterIds = new System.Collections.Generic.List<Guid>() { key };
        //    var parentIds = filterIds;

        //    while (parentIds.Any())
        //    {
        //        parentIds = groups.Where(g => g.ParentId.HasValue && parentIds.Contains(g.ParentId.Value)).Select(g=>g.Id).ToList();
        //        filterIds.AddRange(parentIds);
        //    }

        //    return groups.Where(s => filterIds.Contains(s.Id)).AsQueryable();
        //}

        //[EnableQuery]
        //public IQueryable<Group> GetAllRecursively()
        //{
        //    var groups = db.Groups.ToList();
        //    var filterIds = groups.Where(g => !g.ParentId.HasValue).Select(g => g.Id).ToList();
        //    var parentIds = filterIds;

        //    while (parentIds.Any())
        //    {
        //        parentIds = groups.Where(g => g.ParentId.HasValue && parentIds.Contains(g.ParentId.Value)).Select(g => g.Id).ToList();
        //        filterIds.AddRange(parentIds);
        //    }

        //    return groups.Where(s => filterIds.Contains(s.Id)).AsQueryable();
        //}

        // GET: odata/Groups2(5)/Exams
        [EnableQuery]
        public SingleResult<Exam> GetExams([FromODataUri] Guid key)
            => SingleResult.Create(_context.Group.Where(m => m.Id == key).SelectMany(m => m.Exams));

        // GET: odata/Groups2(5)/Parent
        [EnableQuery]
        public SingleResult<Group> GetParent([FromODataUri] Guid key)
            => SingleResult.Create(_context.Group.Where(m => m.Id == key).Select(m=> m.Parent));

        private bool GroupExists(Guid key)
            => _context.Group.Any(e => e.Id == key);
    }
}
