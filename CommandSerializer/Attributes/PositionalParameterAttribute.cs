using System;

namespace CommandSerializer.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PositionalParameterAttribute : Attribute
    {
        public string Name { get; set; }
        public bool Required { get; set; } = true;
        public Type Converter { get; set; }

        public ParameterAttribute ToParameterAttribute()
        {
            return new ParameterAttribute
            {
                Required = Required,
                Action = Name,
                Converter = Converter
            };
        }
    }
}
