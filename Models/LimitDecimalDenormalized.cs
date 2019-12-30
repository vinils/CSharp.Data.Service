namespace Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class LimitDecimalDenormalized : IDataLimit
    {
        public Guid GroupId { get; set; }
        public DateTime CollectionDate { get; set; }

        public string Name { get; set; }
        public MinTypeEnum? MinType { get; set; }
        public decimal? Min { get; set; }
        public MaxTypeEnum? MaxType { get; set; }
        public decimal? Max { get; set; }
        public int? Color { get; set; }

        public virtual DataDecimal Data { get; set; }

        public string Description
        {
            get
            {
                var minDesc = "";

                if (MinType.HasValue && MinType.Value == MinTypeEnum.BiggerThan)
                    minDesc = ">";
                else if (MinType.HasValue && MinType.Value == MinTypeEnum.EqualsOrBiggerThan)
                    minDesc = ">=";

                var maxDesc = "";

                if (MaxType.HasValue && MaxType.Value == MaxTypeEnum.SmallThan)
                    maxDesc = "<";
                else if (MaxType.HasValue && MaxType.Value == MaxTypeEnum.SmallOrEqualsThan)
                    maxDesc = "<=";

                var ret = "";

                if (Min.HasValue)
                    ret += minDesc + " " + Min.Value.ToString("0.##");

                if (Min.HasValue && Max.HasValue)
                    ret += " a ";

                if (Max.HasValue)
                    ret += maxDesc + " " + Max.Value.ToString("0.##");

                ret += ";";

                var limitDescrition = !string.IsNullOrWhiteSpace(Name) ? Name + ": " : "";

                return limitDescrition + ret;
            }
        }

        public bool IsUnderLimit(decimal value)
        {
            var isUnderMin = !Min.HasValue
                || (MinType.Value == MinTypeEnum.BiggerThan && value > Min.Value)
                || (MinType.Value == MinTypeEnum.EqualsOrBiggerThan && value >= Min.Value);

            var isUnderMax = !Max.HasValue
                || (MaxType == MaxTypeEnum.SmallThan && value < Max.Value)
                || (MaxType == MaxTypeEnum.SmallOrEqualsThan && value <= Max.Value);

            return isUnderMin && isUnderMax;
        }
    }
}