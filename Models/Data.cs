namespace Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public abstract class Data
    {
        public Guid GroupId { get; set; }
        public DateTime CollectionDate { get; set; }
        public abstract dynamic Value { get; }

        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }

    }
}