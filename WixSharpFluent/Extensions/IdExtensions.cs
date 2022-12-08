using System;

namespace WixSharp.Fluent.Extensions
{
    /// <summary>
    /// Just utility for ID handling
    /// </summary>
    public static class IdExtensions
    {
        /// <summary>
        /// Formats guid as string to valid Wix Id
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        internal static string ToValidWixId(this string guid)
        {
            return $"_{guid.Replace("-", "")}";
        }

        /// <summary>
        /// Formats guid as string to valid Wix Id
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string ToValidWixId(this Guid guid)
        {
            return guid.ToString().ToValidWixId();
        }

        /// <summary>
        /// Checks if the ID is Set
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal static bool IsIdSet(this WixEntity entity)
        {
            return !entity.Id.IsEmpty();
        }
    }
}
