﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WixSharp.Fluent.Attributes
{
    /// <summary>
    /// Sets the path to content
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblySourcePathAttribute : Attribute
    {
        /// <summary>
        /// The MSBuild SourcePath variable
        /// </summary>
        public string Path { get; set; }
    }
}
