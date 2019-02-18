namespace Data.Controllers
{
    using Data.Models;
    using Microsoft.AspNet.OData;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    public static class MyExtensions
    {
        private static void CastListToDictionaryTree<Y>(this List<Group> groups, DictionaryTree<Group, Y> rootDictionary, List<Group> rootGroups) where Y : class
        {
            foreach (var rootGroup in rootGroups)
            {
                var newRootDictionary = rootDictionary.New(rootGroup);
                var newRootGroups = groups.Where(g => g.ParentId == rootGroup.Id).ToList();
                CastListToDictionaryTree(groups, newRootDictionary, newRootGroups);
            }
        }

        public static DictionaryTree<Group, Y> ToDictionaryTree<Y>(this System.Data.Entity.DbSet<Group> groupsDb, Func<Group, Y> getKey, Group rootGroup = null) where Y : class
        {
            var groups = groupsDb.ToList();
            var rootGroups = groups.Where(g => g.ParentId == rootGroup?.Id).ToList();
            var root = new DictionaryTree<Group, Y>(getKey, rootGroup);
            groups.CastListToDictionaryTree(root, rootGroups);
            return root;
        }

        private static string treatIndex(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(System.Text.NormalizationForm.FormC)
                .ToUpper();
        }
    }

    public class GroupNameTree
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string Name { get; set; }
        public IEnumerable<GroupNameTree> Childs { get; set; }

        private static string treatIndex(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(System.Text.NormalizationForm.FormC)
                .ToUpper();
        }

        public List<Group> CastToGroupList(Group parent, Func<Group, GroupNameTree, Group> cast)
        {
            var ret = new List<Group>();
            var root = cast(parent, this);
            ret.Add(root);

            foreach(var child in Childs)
            {
                ret.AddRange(child.CastToGroupList(root, cast));
            }

            return ret;
        }

        private List<Group> NotIn(DictionaryTree<Group, string> dictionaryTree, Func<GroupNameTree, string> getKey)
        {
            var key = getKey(this);
            var ret = new List<Group>();

            if (dictionaryTree.ContainsKey(key))
            {
                foreach(var child in Childs)
                {
                    ret.AddRange(child.NotIn(dictionaryTree[key], getKey));
                }
            }
            else
            {
                Group cast(Group parent, GroupNameTree group) => new Group() { Id = Guid.NewGuid(), Name = group.Name, ParentId = parent?.Id };
                return CastToGroupList(dictionaryTree.Data, cast);
            }

            return ret;
        }

        public List<Group> NotIn(System.Data.Entity.DbSet<Group> groupsDb)
        {
            string getKey(Group group) => treatIndex(group.Name);
            string getGroupNameKey(GroupNameTree group) => treatIndex(group.Name);

            var groups = groupsDb.ToDictionaryTree(getKey);
            return NotIn(groups, getGroupNameKey);
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

        public async Task<IHttpActionResult> BulkInsertByName(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var groups = parameters["Groups"] as GroupNameTree;
            var newGroups = groups.NotIn(_context.Group);

            _context.Group.AddRange(newGroups);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return Ok(newGroups);
        }
    }

    public class DictionaryTree<T, Y> : Dictionary<Y, DictionaryTree<T, Y>> where Y : class
    {
        public DictionaryTree<T, Y> Parent { get; private set; }
        public T Data { get; private set; }
        protected readonly Func<T, Y> getKey;
        protected Y key => getKey(Data);
        protected List<Y> GetKeyList()
        {
            var ret = new List<Y>();

            if (Parent.key != null)
                ret.AddRange(Parent.GetKeyList());

            ret.Add(key);
            return ret;
        }

        public Y[] Key => GetKeyList().ToArray();

        public DictionaryTree(Func<T, Y> getKey, T root = default(T), DictionaryTree<T, Y> parent = null)
            : base()
        {
            this.getKey = getKey;
            this.Data = root;
            this.Parent = parent;
        }

        public DictionaryTree<T, Y> this[T data]
        {
            get
            {
                var key = getKey(data);
                return this[key];
            }
        }

        public DictionaryTree<T, Y> this[Y[] keys]
        {
            get
            {
                DictionaryTree<T, Y> lastDictionary = this;
                foreach(var key in keys)
                {
                    if (lastDictionary.ContainsKey(key))
                    {
                        lastDictionary = lastDictionary[key];
                    }
                    else
                    {
                        return null;
                    }
                }
                return lastDictionary;
            }
        }

        public bool ContainsKey(T data)
        {
            var key = getKey(data);
            return this.ContainsKey(key);
        }

        public virtual DictionaryTree<T, Y> New(T data)
        {
            Y key = getKey(data);
            var newDictionary = new DictionaryTree<T, Y>(this.getKey, data, this);
            this.Add(key, newDictionary);

            return newDictionary;
        }

        public virtual DictionaryTree<T, Y> AddIfNew(params T[] datas)
        {
            var lastDictionary = this;
            foreach(var data in datas)
            {
                var key = getKey(data);
                if (lastDictionary.ContainsKey(key))
                    lastDictionary = lastDictionary[key];
                else
                    lastDictionary = lastDictionary.New(data);
            }

            return lastDictionary;
        }
    }
}
