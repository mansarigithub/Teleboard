using System;
using System.ComponentModel;

namespace Teleboard.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        public LocalizedDisplayNameAttribute(string Name) : base(Translator.Get(Name)) { }
    }
}