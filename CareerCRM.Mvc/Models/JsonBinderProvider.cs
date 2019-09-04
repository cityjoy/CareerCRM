using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Newtonsoft.Json.Linq;
using CareerCRM.Mvc.Models;

namespace CareerCRM.Mvc
{
    public class JsonBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if(context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(JObject))
            {
                return new BinderTypeModelBinder(typeof(JobjectModelBinder));
            }

            return null;
        }
    }
}