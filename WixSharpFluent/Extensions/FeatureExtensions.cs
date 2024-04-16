using Microsoft.Deployment.WindowsInstaller;
using System.Collections.Generic;

namespace WixSharp.Fluent.Extensions
{
  /// <summary>
  /// Helpers for features
  /// </summary>
  public static class FeatureExtensions
    {
        private static readonly Dictionary<Feature, string> _propertyMapping = new Dictionary<Feature, string>();

        /// <summary>
        /// Computes the default property name for a given <see cref="Feature"/> <see cref="WixEntity.Id"/>
        /// </summary>
        /// <typeparam name="FeatureT"></typeparam>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static string GetDefaultPropertyName<FeatureT>(this FeatureT feature) where FeatureT : Feature
        {
            return $"FEATURE_{feature.Id.Replace(' ', '_').Replace('.', '_').ToUpperInvariant()}";
        }

        /// <summary>
        /// Computes the Property name from the <see cref="Feature"/> <see cref="WixEntity.Id"/>
        /// </summary>
        /// <typeparam name="FeatureT"></typeparam>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static string GetPropertyName<FeatureT>(this FeatureT feature) where FeatureT : Feature
        {
            _propertyMapping.TryGetValue(feature, out var property);
            return property ?? feature.GetDefaultPropertyName();
        }

        /// <summary>
        /// Sets the Property name from the <see cref="Feature"/> <see cref="WixEntity.Id"/>
        /// </summary>
        /// <typeparam name="FeatureT"></typeparam>
        /// <param name="feature"></param>
        /// <param name="propertyName">The property id to set on the feature</param>
        /// <returns></returns>
        public static FeatureT SetPropertyName<FeatureT>(this FeatureT feature, string propertyName = null) where FeatureT : Feature
        {
            _propertyMapping[feature] = propertyName ?? feature.GetDefaultPropertyName();
            return feature;
        }

        /// <summary>
        /// Sets the <see cref="Feature.Condition"/> to check against the <paramref name="propertyName"/> when its equal 0 set the level to 2 
        /// </summary>
        /// <typeparam name="FeatureT"></typeparam>
        /// <param name="feature">The feature needs unique name</param>
        /// <param name="propertyName">The property id to set on the feature</param>
        /// <returns></returns>
        public static FeatureT SetSmart<FeatureT>(this FeatureT feature, string propertyName = null) where FeatureT : Feature
        {
            if(propertyName == null)
              propertyName = feature.GetPropertyName();
            else
              feature.SetPropertyName(propertyName);
            feature.Condition = new FeatureCondition(Condition.Create($"{propertyName} = 0"), 2);
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
