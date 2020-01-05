namespace Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class LimitStringDenormalized
    {
        public Guid GroupId { get; set; }
        public DateTime CollectionDate { get; set; }

        public string Expected { get; set; }
        public int? Color { get; set; }

        public virtual DataString Data { get; set; }
    }
}