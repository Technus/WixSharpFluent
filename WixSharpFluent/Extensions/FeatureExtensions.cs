using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        /// Computes the Property name from the Feature Name. Used when invoking <see cref="SetSmart{FeatureT}(FeatureT, string, string, int?)"/>
        /// </summary>
        /// <typeparam name="FeatureT"></typeparam>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static string GetPropertyName<FeatureT>(this FeatureT feature) where FeatureT : Feature
        {
            return $"FEATURE_{feature.Id.Replace(' ', '_').Replace('.', '_').ToUpperInvariant()}";
        }

        /// <summary>
        /// Sets the Feature ID and Condition to check against the <see cref="GetPropertyName{FeatureT}(FeatureT)"/> and set the level to 2 when its equal 0
        /// </summary>
        /// <typeparam name="FeatureT"></typeparam>
        /// <param name="feature">The feature needs unique name</param>
        /// <param name="id">The id to set on the feature</param>
        /// <param name="condition">What the property should be equal to</param>
        /// <param name="targetLevel">To what Level set the feature on condition</param>
        /// <returns></returns>
        public static FeatureT SetSmart<FeatureT>(this FeatureT feature, string id = null, string condition = null, int? targetLevel = null) where FeatureT : Feature
        {
            condition = condition ?? $"{feature.GetPropertyName()} = 0";
            feature.Condition = new FeatureCondition(Condition.Create(condition), targetLevel ?? 2);
            return feature;
        }

        /// <summary>
        /// Gets the default condition to enable this feature
        /// </summary>
        /// <typeparam name="FeatureT"></typeparam>
        /// <param name="feature"></param>
        /// <param name="state"></param>
        /// <param name="propValue"></param>
        /// <returns></returns>
        public static Condition GetCondition<FeatureT>(this FeatureT feature, InstallState? state = null, string propValue = null) where FeatureT : Feature
        {
            state = state ?? InstallState.Local;
            propValue = propValue ?? "1";

            return Condition.Create($"!{feature.Id} = {((int)state)}") |
                Condition.Create($"{feature.GetPropertyName()} = {propValue}");
        }

        /// <summary>
        /// Due to certain limitations when setting condition on features you need to specify all features this components depends on (not needed if depends on one)
        /// (this includes parent features if 'and-ed' check needs to be done on them too )
        /// </summary>
        /// <typeparam name="FeaturesT"></typeparam>
        /// <param name="features"></param>
        /// <param name="state"></param>
        /// <param name="propValue"></param>
        /// <returns></returns>
        public static Condition GetConditions<FeaturesT>(this FeaturesT features, InstallState? state = null, string propValue = null) where FeaturesT : IEnumerable<Feature>
        {
            Condition condition=null;
            foreach (var feature in features)
                if (condition == null)
                    condition = feature.GetCondition(state,propValue);
                else
                    condition &= feature.GetCondition(state, propValue);

            condition = condition ?? Condition.Always;

            return condition;
        }

        /// <summary>
        /// Due to certain limitations when setting condition on features you need to specify all features this components depends on (not needed if depends on one)
        /// (this includes parent features if 'and-ed' check needs to be done on them too )
        /// </summary>
        /// <param name="features"></param>
        /// <returns></returns>
        public static Condition GetConditions(params Feature[] features)
        {
            return features.GetConditions();
        }
    }
}
