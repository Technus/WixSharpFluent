using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WixSharp.Fluent.Extensions
{
    /// <summary>
    /// Helpers for features
    /// </summary>
    public static class FeatureExtensions
    {
        /// <summary>
        /// Computes the Feature ID from the Feature Name. Used when invoking <see cref="SetSmart{FeatureT}(FeatureT, string, string, int?)"/>
        /// </summary>
        /// <typeparam name="FeatureT"></typeparam>
        /// <param name="feature"></param>
        /// <returns></returns>
        internal static string ComputeId<FeatureT>(this FeatureT feature) where FeatureT : Feature
        {
            if (string.IsNullOrWhiteSpace(feature.Name))
                throw new ArgumentException("Feature Name must be defined");
            return $"Feature_{feature.Name.Replace(' ', '_').Replace('.', '_')}";
        }

        /// <summary>
        /// Computes the Property name from the Feature Name. Used when invoking <see cref="SetSmart{FeatureT}(FeatureT, string, string, int?)"/>
        /// </summary>
        /// <typeparam name="FeatureT"></typeparam>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static string GetPropertyName<FeatureT>(this FeatureT feature) where FeatureT : Feature
        {
            return feature.ComputeId().ToUpperInvariant();
        }

        /// <summary>
        /// Sets the Feature ID and Condition to check against the <see cref="GetPropertyName{FeatureT}(FeatureT)"/> and set the level to 2 when its equal 0
        /// </summary>
        /// <typeparam name="FeatureT"></typeparam>
        /// <param name="feature"></param>
        /// <param name="id"></param>
        /// <param name="condition">What the property should be equal to</param>
        /// <param name="targetLevel">To what Level set the feature on condition</param>
        /// <returns></returns>
        public static FeatureT SetSmart<FeatureT>(this FeatureT feature, string id = null, string condition = null, int? targetLevel = null) where FeatureT : Feature
        {
            condition = condition ?? $"{feature.GetPropertyName()} = 0";

            if (id!=null)
                feature.Id = id;
            else if(feature.Id==null)
            {
                if (string.IsNullOrWhiteSpace(feature.Name))
                    throw new ArgumentException("Feature Name must be defined");
                feature.Id = feature.ComputeId();
            }

            feature.Condition = new FeatureCondition(Condition.Create(condition), targetLevel ?? 2);

            return feature;
        }
    }
}
