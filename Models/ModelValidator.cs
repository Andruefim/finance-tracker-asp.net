using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AngularWithASP.Server.Models;

public class ValidationResult
{ 
    public string Property { get; set; }
    public string ErrorMessage { get; set; }
}
public static class ModelValidator
{
    public static List<ValidationResult> Validate(object model)
    {
        if (model == null) 
            throw new ArgumentNullException(nameof(model));

        var results = from property in model.GetType().GetProperties()
                      from attribute in property.GetCustomAttributes(typeof(ValidationAttribute), true).OfType<ValidationAttribute>()
                      where !attribute.IsValid(property.GetValue(model))
                      select new ValidationResult
                      {
                          Property = property.Name,
                          ErrorMessage = attribute.ErrorMessage
                      };

        return results.ToList();
    }
}
