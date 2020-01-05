namespace Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class LimitDecimal
    {
        [ForeignKey("Group")]
        public Guid? GroupId { get; set; }
        public int Priority { get; set; }
        public decimal Max { get; set; }

        public decimal? Min { get; set; }
        public string Name { get; set; }

        [ForeignKey("GroupId, Priority, Min")]
        public virtual LimitDecimal Last { get; set; }

        public virtual Group Group { get; set; }
    }
}