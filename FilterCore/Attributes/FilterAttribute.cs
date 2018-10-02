using System;
using FilterCore.Enums;

namespace FilterCore.Attributes
{
    [System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class FilterAttribute : Attribute
    {
        public string FieldToCompare{ get; private set; }

        public FilterOperator OperatorToApply{ get; private set; }

        public bool CompareToNull { get; private set; }

        public FilterAttribute(string fieldToCompare, FilterOperator operatorToApply, bool compareToNull = false)
        {
            FieldToCompare = fieldToCompare;
            OperatorToApply = operatorToApply;
            CompareToNull = compareToNull;
        }
    }
}