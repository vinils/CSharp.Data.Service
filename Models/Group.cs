namespace Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

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
        public virtual ICollection<Exam> Exams { get; set; }
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
    }
}