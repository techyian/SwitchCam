// This is free and unencumbered software released into the public domain.
// Happy coding!!! - GtkSharp Team

using System;

namespace SwitchCam
{
    class SectionAttribute : Attribute
    {
        public string Description { get; set; }

        public Type ContentType { get; set; }

        public Category Category { get; set; }
    }
}