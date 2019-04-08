namespace Data.Controllers
{
    using Data.Models;
    using Microsoft.AspNet.OData;
    using System;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class GroupDictionaryByName : System.Collections.Generic.Dictionary<string, GroupDictionaryByName>
    {
        public System.Collections.Generic.List<Data> Datas = new System.Collections.Generic.List<Data>();

        public void Add(Data data, params string[] keys)
        {
            var lastGroup = this;

            foreach (var key in keys)
            {
                if (!lastGroup.ContainsKey(key))
                {
                    lastGroup.Add(key, new GroupDictionaryByName());
                }

                lastGroup = lastGroup[key];
            }

            lastGroup.Datas.Add(data);
        }

        public System.Collections.Generic.List<string> ToString2(string rootKey = "")
        {
            var ret = new System.Collections.Generic.List<string>();

            if (rootKey != "")
            {
                ret.Add(rootKey);
            }

            foreach (var group in this)
            {
                ret.AddRange(group.Value.ToString2(rootKey + "\\" + group.Key));
            }

            return ret;
        }
    }

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

        // GET: odata/Groups2(5)/Datas
        [EnableQuery]
        public SingleResult<Data> GetDatas([FromODataUri] Guid key)
            => SingleResult.Create(_context.Group.Where(m => m.Id == key).SelectMany(m => m.Datas));

        // GET: odata/Groups2(5)/Parent
        [EnableQuery]
        public SingleResult<Group> GetParent([FromODataUri] Guid key)
            => SingleResult.Create(_context.Group.Where(m => m.Id == key).Select(m=> m.Parent));

        private bool GroupExists(Guid key)
            => _context.Group.Any(e => e.Id == key);

        public async Task<IHttpActionResult> BulkInsertByName(GroupDictionaryByName groups)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //_context.Data.Add(data);

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateException)
            //{
            //    if (DataExists(data.GroupId, data.CollectionDate))
            //    {
            //        return Conflict();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            return Ok();
        }
    }


    //[HttpPost]
    //public async Task<IHttpActionResult> BulkInsert(ODataActionParameters parameters)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return BadRequest(ModelState);
    //    }

    //    var datas = parameters["Datas"] as System.Collections.Generic.IEnumerable<Data>;

    //    var datasDistinct = datas
    //        .GroupBy(e => new { e.CollectionDate, e.GroupId })
    //        .Select(eG => eG.First())
    //        .ToList();

    //    var missingRecords = datasDistinct
    //        .Where(d => !_context.Data.Any(e => e.CollectionDate == d.CollectionDate && e.GroupId == d.GroupId)).ToList();

    //    _context.Data.AddRange(missingRecords);

    //    try
    //    {
    //        await _context.SaveChangesAsync();
    //    }
    //    catch (DbUpdateException)
    //    {
    //        throw;
    //    }

    //    //return Created(datas);
    //    return Ok();
    //}
}
