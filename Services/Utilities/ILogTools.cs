using System.Security.Claims;

namespace JaosLib.Services.Utilities
{
    public interface ILogTools
    {

        void Log(ClaimsPrincipal user, string modelName, string actionName, int itemId, object model, IEnumerable<string>? excludedFields = null);
    }
}
