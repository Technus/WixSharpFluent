using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WixSharp.Fluent.Extensions
{
    public static class PathExtensions
    {
        public static string[] BreakPathToPieces(this string path)
        {
            return path.Replace('/', '\\').Split('\\').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        }

        public static string[][] BreakPathsToPieces(this string paths)
        {
            return paths.Split(';').Where(s => !string.IsNullOrWhiteSpace(s)).Select(path=>path.BreakPathToPieces()).ToArray();
        }
    }
}
