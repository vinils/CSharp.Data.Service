namespace Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class Group
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Initials { get; set; }
        public Guid? ParentId { get; set; }
        public string MeasureUnit { get; set; }
        [ForeignKey("ParentId")]
        public virtual Group Parent { get; set; }
        public virtual ICollection<Data> Datas { get; set; }
        public virtual ICollection<Group> Childs { get; set; }
        public virtual ICollection<LimitDecimal> LimitDecimals { get; set; }

        public Group()
        {
            Childs = new HashSet<Group>();
        }

        public Guid? GetLastParentId()
        {
            if (Parent == null)
                return ParentId;
            else
            {
                var lastNode = GetLastNode();
                return lastNode.ParentId != null ? lastNode.ParentId : lastNode.Id;
            }
        }

        public Group GetLastNode()
        {
            Group lastNode = this;

            while(lastNode.Parent != null)
            {
                lastNode = lastNode.Parent;
            }

            return lastNode;
        }

        public IEnumerator<Group> GetEnumerator()
        {
            var current = this;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }

        private Group FindChildBy<Y>(Func<Group, Y> getProperty, Y key) where Y : class
            => Childs.Where(c => getProperty(c) == key).FirstOrDefault();

        private Group FindChildBy<Y>(Func<Group, Y> getProperty, params Y[] keys) where Y : class
        {
            var lastRoot = this;
            foreach(var key in keys)
            {
                lastRoot = lastRoot.Childs?
                    .Where(c => getProperty(c).Equals(key))
                    .FirstOrDefault();

                if (lastRoot == null)
                    return lastRoot;
            }

            return lastRoot;
        }

        public Group FindChildBy<Y>(Func<Group, Y> getProperty, Y rootKey, params Y[] childsKeys) where Y : class
        {
            if (childsKeys == null || childsKeys.Length == 0)
                throw new ArgumentNullException("names argument null or empty");

            if (!Name.Equals(rootKey))
                throw new ArgumentException($"Root name {rootKey} not equals this root name {Name}!");

            return FindChildBy(getProperty, childsKeys);
        }

        public Group Find<Y>(Func<Group, Y> getProperty, params Y[] keys) where Y : class
        {
            if (keys == null || keys.Length == 0)
                throw new ArgumentNullException("names argument null or empty");

            var rootKey = keys[0];

            if (!Name.Equals(rootKey))
                return null;

            if (keys.Length == 1)
                return this;

            var childsKeys = keys.Skip(1).ToArray();
            return FindChildBy(getProperty, rootKey, childsKeys);
        }

        public Controllers.DictionaryTree<Group, Y> ToDictionaryTree<Y>(Func<Group, Y> getKey, Controllers.DictionaryTree<Group, Y> parent = null) where Y : class
        {
            var ret = new Controllers.DictionaryTree<Group, Y>(getKey, this, parent);

            foreach(var child in Childs)
            {
                try
                {
                    var newChild = child.ToDictionaryTree(getKey, ret);
                    ret.Add(newChild);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            return ret;
        }
    }
}