namespace Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class LimitStringDenormalized
    {
        [Key, Column(Order = 1)]
        [ForeignKey("Exam")]
        public Guid GroupId { get; set; }
        [Key, Column(Order = 2)]
        [ForeignKey("Exam")]
        public DateTime CollectionDate { get; set; }

        public string Expected { get; set; }
        public int? Color { get; set; }

        public virtual ExamString Exam { get; set; }
    }
}