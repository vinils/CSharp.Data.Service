namespace Data.Controllers
{
    using Data.Models;
    using Microsoft.AspNet.OData;
    using System;
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
                try
                {
                    var newRootDictionary = rootDictionary.New(rootGroup);
                    var newRootGroups = groups.Where(g => g.ParentId == rootGroup.Id).ToList();
                    CastListToDictionaryTree(groups, newRootDictionary, newRootGroups);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
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

        public static string RemoveAccentsToUpper(this string text)
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

        private GroupNameTree FindChild(string name)
        {
            return Childs?.Where(g => g.Name.Equals(name)).FirstOrDefault();
        }

        private GroupNameTree FindChild(params string[] names)
        {
            GroupNameTree lastChild = this;
            foreach (var name in names)
            {
                lastChild = lastChild.FindChild(name);

                if (lastChild == null)
                    return null;
            }
            return lastChild;
        }

        public GroupNameTree FindChild(string rootName, params string[] childsNames)
        {
            if (childsNames == null || childsNames.Length == 0)
                throw new ArgumentNullException("names argument null or empty");

            if (!Name.Equals(rootName))
                throw new ArgumentException($"Root name {rootName} not equals this root name {Name}!");

            return FindChild(childsNames);
        }

        public GroupNameTree Find(params string[] names)
        {
            if (names == null || names.Length == 0)
                throw new ArgumentNullException("names argument null or empty");

            var rootName = names[0];

            if (!Name.Equals(rootName))
                return null;

            if (names.Length == 1)
                return this;

            var childsNames = names.Skip(1).ToArray();
            return FindChild(rootName, childsNames);
        }

        public List<Group> CastToGroupList(Group parent, Func<Group, GroupNameTree, Group> cast)
        {
            var ret = new List<Group>();
            var root = cast(parent, this);
            ret.Add(root);

            foreach (var child in Childs)
            {
                ret.AddRange(child.CastToGroupList(root, cast));
            }

            return ret;
        }

        public List<Group> NotIn(DictionaryTree<Group, string> root, Func<GroupNameTree, string> getKey)
        {
            var key = getKey(this);
            var ret = new List<Group>();

            if (root.ContainsKey(key))
            {
                foreach (var child in Childs)
                {
                    ret.AddRange(child.NotIn(root[key], getKey));
                }
            }
            else
            {
                Group cast(Group parent, GroupNameTree group) => new Group() { Id = Guid.NewGuid(), Name = group.Name, ParentId = parent?.Id };
                return CastToGroupList(root.Data, cast);
            }

            return ret;
        }

        //public List<Group> NotIn(DictionaryTree<Group, string> dictionaryTree, Func<GroupNameTree, string> getKey, string[] keys)
        //{
        //    var node = this.Find(keys);
        //    return node.NotIn(dictionaryTree, getKey);
        //}

        //public List<Group> NotIn(System.Data.Entity.DbSet<Group> groupsDb, string[] root = null)
        //{
        //    string getKey(Group group) => treatIndex(group.Name);
        //    string getGroupNameKey(GroupNameTree group) => treatIndex(group.Name);

        //    var groups = groupsDb.ToDictionaryTree(getKey);
        //    var node = root != null ? groups[root] : groups;
        //    return NotIn(node, getGroupNameKey);
        //}
    }

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

        public IHttpActionResult BulkInsertByName(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newNode = parameters["NewGroups"] as GroupNameTree;
            var rootPath = (parameters["RootPath"] as IEnumerable<string>).ToArray();
            var firstRoot = rootPath[0];

            //_context.Configuration.LazyLoadingEnabled = true;

            var firstRootGroup = _context.Group
                .Where(g => g.Name == firstRoot)
                .First();

            var lastRootGroup = firstRootGroup
                .Find(g => g.Name, rootPath)
                .ToDictionaryTree(g => MyExtensions.RemoveAccentsToUpper(g.Name));

            string getKey(GroupNameTree g) => MyExtensions.RemoveAccentsToUpper(g.Name);
            var newGroups = newNode.NotIn(lastRootGroup, getKey);

            _context.Group.AddRange(newGroups);

            try
            {
                _context.SaveChanges();
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

        public void Add(T data)
            => New(data);

        public void Add(DictionaryTree<T, Y> dictionaryTree)
        {
            if (dictionaryTree.Parent != this)
                throw new ArgumentException("Not in the same parent");

            this.Add(getKey(dictionaryTree.Data), dictionaryTree);
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
