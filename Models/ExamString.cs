namespace Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ExamString : Exam
    {
        [Required]
        public string StringValue { get; set; }
        public override dynamic Value => StringValue;

        public virtual LimitStringDenormalized LimitDenormalized { get; set; }
    }
}