using System.ComponentModel;

namespace IMRepo.Models.Domain
{
    public class AttributeUtils
    {
        /// <summary>
        /// returns the display name of a property
        /// </summary>
        /// <param name="objectType">The model that contains the property</param>
        /// <param name="propertyName">The property name who's displayName will be returned</param>
        /// <returns>The display name for the propertyName in the objectType</returns>
        public string GetDisplayName(Type objectType, string propertyName)
        {
            var propertyInfo = objectType.GetProperty(propertyName);
            if (propertyInfo != null)
            {
                DisplayNameAttribute? displayAttribute = propertyInfo
                    .GetCustomAttributes(typeof(DisplayNameAttribute), true)
                    .FirstOrDefault() as DisplayNameAttribute;

                if (displayAttribute != null && !string.IsNullOrWhiteSpace(displayAttribute.DisplayName))
                {
                    return displayAttribute.DisplayName;
                }
            }

            return propertyName; // Use property name as fallback
        }

    }
}
