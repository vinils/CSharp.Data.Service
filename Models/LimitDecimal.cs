namespace Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class LimitDecimal
    {
        [Key, Column(Order = 1)]
        [ForeignKey("Group")]
        public Guid? GroupId { get; set; }
        [Key, Column(Order = 2)]
        public int Priority { get; set; }
        [Key, Column(Order = 3)]
        public decimal Max { get; set; }

        public decimal? Min { get; set; }
        public string Name { get; set; }

        [ForeignKey("GroupId, Priority, Min")]
        public virtual LimitDecimal Last { get; set; }

        public virtual Group Group { get; set; }
    }
}