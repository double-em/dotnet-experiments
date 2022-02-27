using System;

namespace SourceGeneration.DataModel
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    [System.Diagnostics.Conditional("DataModelGenerator_DEBUG")]
    public sealed class DataModelAttribute : Attribute
    {
        public DataModelAttribute() {}
        public string PropertyName { get; set; }
    }
}