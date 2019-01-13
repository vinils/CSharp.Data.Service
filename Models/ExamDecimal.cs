namespace Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ExamDecimal : Exam
    {
        [Required]
        public decimal DecimalValue { get; set; }
        public override dynamic Value => DecimalValue;

        public virtual LimitDecimalDenormalized LimitDenormalized { get; set; }
    }
}