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
        var results = new List<ValidationResult>();

        if (model == null) 
            throw new ArgumentNullException(nameof(model));

        var properties = model.GetType().GetProperties();

        foreach (var property in properties)
        {
            foreach (var attribute in property.GetCustomAttributes(typeof(ValidationAttribute), true))
            {
                if (attribute is ValidationAttribute validationAttribute)
                {
                    var value = property.GetValue(model);

                    if (!validationAttribute.IsValid(value))
                    {
                        results.Add(new ValidationResult
                        {
                            Property = property.Name,
                            ErrorMessage = validationAttribute.ErrorMessage
                        });
                       
                    }
                }

            }
        }
        return results;

    }
}
