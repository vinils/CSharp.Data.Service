namespace Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class DataString : Data
    {
        [Required]
        public string StringValue { get; set; }
        public override dynamic Value => StringValue;

        public virtual LimitStringDenormalized LimitDenormalized { get; set; }
    }
}