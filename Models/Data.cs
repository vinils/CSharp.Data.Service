namespace Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public abstract class Data
    {
        [Key, Column(Order = 1)]
        public Guid GroupId { get; set; }
        [Key, Column(Order = 2)]
        public DateTime CollectionDate { get; set; }
        public abstract dynamic Value { get; }

        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }
    }
}