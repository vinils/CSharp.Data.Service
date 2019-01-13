

//namespace Saude1.Models
//{
//    using AdaptiveLINQ;
//    using System;
//    using System.Collections;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Linq.Expressions;
//    using System.Web;

//    public class MySalesCube : ICubeDefinition<ExamDecimal, ExamDecimalDetailCube>
//    {
//        static public readonly Expression<Func<ExamDecimal, Guid>> GroupId =
//        orderDetail => orderDetail.GroupId;

//        static public readonly Expression<Func<ExamDecimal, Group>> Group =
//            orderDetail => orderDetail.Group;

//        static public readonly Expression<Func<ExamDecimal, LimitDecimalDenormalized>> LimitDecimalDenormalized =
//            orderDetail => orderDetail.LimitDecimalDenormalized;

//        static public readonly Expression<Func<ExamDecimal, String>> Date =
//        //bug on orderDetail => orderDetail.CollectionDate.ToString("yyyy-MM-dd"); //"LINQ to Entities does not recognize the method 'System.String ToString(System.String)' method, and this method cannot be translated into a store expression
//        orderDetail => orderDetail.CollectionDate.Year 
//        + "-" + ("0" + orderDetail.CollectionDate.Month).Substring((orderDetail.CollectionDate.Month < 10 ? 0 : 1), 2)
//        + "-" + ("0" + orderDetail.CollectionDate.Day).Substring((orderDetail.CollectionDate.Day < 10 ? 0 : 1), 2);

//        static public readonly Expression<Func<ExamDecimal, int>> Year =
//            orderDetail => orderDetail.CollectionDate.Year;

//        static public readonly Expression<Func<ExamDecimal, int>> Month =
//            orderDetail => orderDetail.CollectionDate.Month;

//        static public readonly Expression<Func<ExamDecimal, int>> Day =
//            orderDetail => orderDetail.CollectionDate.Day;

//        static public readonly Expression<Func<ExamDecimal, int>> Hour =
//            orderDetail => orderDetail.CollectionDate.Hour;

//        static public readonly Expression<Func<ExamDecimal, int>> Minute =
//            orderDetail => orderDetail.CollectionDate.Minute;

//        static public readonly Expression<Func<IEnumerable<ExamDecimal>, decimal>> SumValues =
//            orderDetails => orderDetails.Sum(item => item.DecimalValue);

//        static public readonly Expression<Func<IEnumerable<ExamDecimal>, decimal>> CountValues =
//            orderDetails => orderDetails.Count();

//        static public readonly Expression<Func<IEnumerable<ExamDecimal>, decimal>> AvgValues =
//            orderDetails => orderDetails.Average(item => item.DecimalValue);
//    }

//    public partial class ExamDecimalDetailCube
//    {
//        [System.ComponentModel.DataAnnotations.Key]
//        public Guid GroupId { get; set; }

//        public int Year { get; set; }
//        public int Month { get; set; }
//        public int Day { get; set; }
//        public int Hour { get; set; }
//        public int Minute { get; set; }
//        public decimal SumValues { get; set; }
//        public decimal CountValues { get; set; }
//    }

//    public partial class ExamDecimalCube
//    {
//        [System.ComponentModel.DataAnnotations.Key]
//        public Guid GroupId { get; set; }
//        public Group Group { get; set; }
//        public LimitDecimalDenormalized LimitDecimalDenormalized { get; set; }

//        public String Date { get; set; }
//        public int Hour { get; set; }
//        public int Minute { get; set; }
//        public decimal SumValues { get; set; }
//        public decimal CountValues { get; set; }
//    }
//}

