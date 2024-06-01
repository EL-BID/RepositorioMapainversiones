using System.Collections.Generic;

namespace IMRepo.Utilities
{
    public static class ViewsTools
    {

        public static IDictionary<string, object>? attributes(bool isEditable, params string[] attributesPairs)
        {
            IDictionary<string, object>? attributes = null;
            if (attributesPairs.Length % 2 == 0 && attributesPairs.Length > 0)
            {
                attributes = new Dictionary<string, object>();

                for (int i = 0; i < attributesPairs.Length; i += 2)
                    attributes.Add(attributesPairs[i], attributesPairs[i + 1]);
            }

            //if (!isEditable)
            //{
            //    if (attributes == null) attributes = new Dictionary<string, object>();
            //    attributes.Add("disabled", "disabled");
            //}
            return attributes;
        }

    }
}
