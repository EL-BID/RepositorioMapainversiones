using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Reflection;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace IMRepo.Services.Utilities
{
    public class JaosLibUtils
    {
        public void sendMessagesOnTempData(ITempDataDictionary TempData, dynamic ViewBag)
        {
            if (!string.IsNullOrEmpty(ViewBag.errorMessage?.ToString()))
                TempData["errorMessage"] = ViewBag.errorMessage;
            if (!string.IsNullOrEmpty(ViewBag.okMessage?.ToString()))
                TempData["okMessage"] = ViewBag.okMessage;
            if (!string.IsNullOrEmpty(ViewBag.debugErrorMessage?.ToString()))
                TempData["debugErrorMessage"] = ViewBag.debugErrorMessage;
            if (!string.IsNullOrEmpty(ViewBag.warningMessage?.ToString()))
                TempData["warningMessage"] = ViewBag.warningMessage;
            if (!string.IsNullOrEmpty(ViewBag.Message?.ToString()))
                TempData["Message"] = ViewBag.Message;
            if (!string.IsNullOrEmpty(ViewBag.returnUrl?.ToString()))
                TempData["returnUrl"] = ViewBag.returnUrl;
            if (!string.IsNullOrEmpty(ViewBag.bufferedUrl?.ToString()))
                TempData["bufferedUrl"] = ViewBag.bufferedUrl;
        }

        public void receiveMessagesFromTempData(ITempDataDictionary TempData, dynamic ViewBag)
        {
            if (!string.IsNullOrEmpty(TempData["errorMessage"]?.ToString()))
                ViewBag.errorMessage = TempData["errorMessage"]?.ToString();
            if (!string.IsNullOrEmpty(TempData["okMessage"]?.ToString()))
                ViewBag.okMessage = TempData["okMessage"]?.ToString();
            if (!string.IsNullOrEmpty(TempData["debugErrorMessage"]?.ToString()))
                ViewBag.debugErrorMessage = TempData["debugErrorMessage"]?.ToString();
            if (!string.IsNullOrEmpty(TempData["warningMessage"]?.ToString()))
                ViewBag.warningMessage = TempData["warningMessage"]?.ToString();
            if (!string.IsNullOrEmpty(TempData["Message"]?.ToString()))
                ViewBag.Message = TempData["Message"]?.ToString();
            if (!string.IsNullOrEmpty(TempData["returnUrl"]?.ToString()))
                ViewBag.returnUrl = TempData["returnUrl"]?.ToString();
            if (!string.IsNullOrEmpty(TempData["bufferedUrl"]?.ToString()))
                ViewBag.bufferedUrl = TempData["bufferedUrl"]?.ToString();
        }





        /// <summary>
        /// Mandatory number errors are hidden to avoid the invalid number error when 
        /// decimals or thousand separators are included
        /// </summary>
        /// <returns></returns>
        public string? getMandatoryNumbersErrorMessages(ModelStateDictionary ModelState, params string[] numericFields)
        {
            if (numericFields != null && numericFields.Length > 0)
            {
                return
                string.Join("; ", ModelState.Keys.Where(k => numericFields.Contains(k)).SelectMany(k => ModelState[k]!.Errors).Select(e => e.ErrorMessage));
                //                string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }
            return null;
        }



        //========================================================
        // 
        //         Titles for Model and Properties
        //
        //--------------------------------------------------------

        public string titleOf<TModel>(string fieldName)
        {
            MemberInfo? property = typeof(TModel).GetProperty(fieldName);
            var attribute = property?.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                                    .Cast<DisplayNameAttribute>().Single();
            return attribute?.DisplayName ?? "";
        }

        public string titleOf<TModel>()
        {
            var displayNameAttribute = typeof(TModel).GetCustomAttribute<DisplayNameAttribute>();
            return displayNameAttribute?.DisplayName ?? "";
        }


        public string titleForXX<TModel>(string propertyName)
        {
            MemberInfo? property = typeof(TModel).GetProperty(propertyName);

            if (property != null)
            {
                DisplayAttribute? displayAttribute = property
                    .GetCustomAttribute<DisplayAttribute>();
                if (displayAttribute != null)
                {
                    return displayAttribute.Name ?? "";
                }
                // If the display name is not specified, return the property name
                return property.Name;
            }
            return "";
        }


        public string? titleForXX<TModel>()
        {

            Type modelType = typeof(TModel);

            // Get the DisplayName attribute of the model class
            DisplayNameAttribute? displayNameAttribute = modelType.GetCustomAttribute<DisplayNameAttribute>();

            // Check if the DisplayName attribute is applied
            if (displayNameAttribute != null)
                return displayNameAttribute.DisplayName;
            return nameof(TModel);
        }


    }
}
