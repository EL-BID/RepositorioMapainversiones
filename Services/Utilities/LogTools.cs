namespace JaosLib.Services.Utilities
{
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.Security.Claims;

    public class LogTools : ILogTools
    {
        private readonly ILogger<LogTools> logger;

        public LogTools(ILogger<LogTools> logger)
        {
            this.logger = logger;
        }


        public void Log(ClaimsPrincipal user, string modelName, string actionName, int itemId, object model, IEnumerable<string>? excludedFields = null)
        {
            string userEmail = user?.FindFirst(ClaimTypes.Email)?.Value ?? "Unknown";
            string serializedModel = excludedFields != null ?
                SerializeExcluding(model, excludedFields) :
                JsonConvert.SerializeObject(model);
            logger.LogWarning("User: {UserEmail}, Model: {ModelName}, Action: {actionName}, ID: {ModelId}, Serialized: {SerializedModel}", userEmail, modelName, actionName, itemId, serializedModel);
        }


        /// <summary>
        /// Seriliaze a Model (object) excluding unwanted fields.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="excludedFields"></param>
        /// <returns></returns>
        private string SerializeExcluding(object model, IEnumerable<string> excludedFields)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new CustomResolver(excludedFields),
                Formatting = Formatting.None
            };
            return JsonConvert.SerializeObject(model, settings);
        }


        private class CustomResolver : DefaultContractResolver
        {
            private readonly HashSet<string> excludedFields;

            public CustomResolver(IEnumerable<string> excludedFields)
            {
                this.excludedFields = new HashSet<string>(excludedFields);
            }

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
                return properties.Where(p => !excludedFields.Contains(p.PropertyName!)).ToList();
            }
        }

    }
}
