using System;

namespace Sample.Classes
{
    public class ObjectClass
    {
        public int Identification { get; set; }

        public string FullName { get; set; }

        public DateTime BirthDate { get; set; }

        public ChildObjectClass ChildObject { set; get; }

        public override string ToString()
        {
            return $"Id: {Identification}, Name: {FullName}, BirthDate: {BirthDate}"
            + (ChildObject != null ? $", ChildId: {ChildObject.Identification}, Value: {ChildObject.Value}" 
                + (ChildObject.InnerChild != null ? $", Decrease: {ChildObject.InnerChild.Decrease}, Value: {ChildObject.InnerChild.HasValue}" : string.Empty)
            : string.Empty);
        }
    }
}