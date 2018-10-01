using System;
using FilterCore.Attributes;
using FilterCore.Enums;
using FilterCore.Interfaces;

namespace Sample.Classes
{
    public class FilterClass : IFilter
    {
        [Filter(nameof(ObjectClass.Identification), FilterOperator.Equal)]
        public int Id { get; set; }

        [Filter(nameof(ObjectClass.FullName), FilterOperator.Like)]
        public string Name { get; set; }

        [Filter(nameof(ObjectClass.BirthDate), FilterOperator.GreaterThanOrEqual)]
        public DateTime BeginBirthDate { get; set; }

        [Filter(nameof(ObjectClass.BirthDate), FilterOperator.LessThanOrEqual)]
        public DateTime EndBirthDate { get; set; }

        [Filter("ChildObject.Identification", FilterOperator.Equal)]
        public int ChildId { get; set; }

        [Filter("ChildObject.Value", FilterOperator.GreaterThan)]
        public decimal Value { get; set; }

        [Filter("ChildObject.InnerChild.Decrease", FilterOperator.LessThan)]
        public float Decrease { get; set; }

        [Filter("ChildObject.InnerChild.HasValue", FilterOperator.Equal)]
        public bool HasValue { get; set; }

        public int CurrentPage { get; set; }
        
        public int PageSize { get; set; }
    }
}