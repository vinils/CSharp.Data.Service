namespace Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class LimitStringDenormalized
    {
        [Key, Column(Order = 1)]
        [ForeignKey("Data")]
        public Guid GroupId { get; set; }
        [Key, Column(Order = 2)]
        [ForeignKey("Data")]
        public DateTime CollectionDate { get; set; }

        public string Expected { get; set; }
        public int? Color { get; set; }

        public virtual DataString Data { get; set; }
    }
}