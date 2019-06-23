using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Translations.Entities;

namespace Translations
{
  public class TranslationJsonGenerator
  {
    private const char Delimiter = '.';

    public Result<string> GenerateJsonString(IList<TranslationEntity> translations)
    {
      object result = new Dictionary<object, object>();
      foreach (var translation in translations)
      {
        var validationResult = Validate(translation);
        if (!validationResult.IsSuccess)
        {
          return Result.Fail<string>(validationResult.Error.Message);
        }
        result = CreateElement(result, translation.Placeholder.Name, translation.Text);
      }

      var json = JsonConvert.SerializeObject(result);

      return Result.Ok(json);
    }

    public bool IsPlaceholderValid(string placeholder)
    {
      var regex = new Regex($"[^A-Za-z0-9{Delimiter}]+");
      var anyForbiddenCharacters = regex.Match(placeholder).Success;
      var invalidFormat = placeholder.Contains($"{Delimiter}{Delimiter}");
      return !anyForbiddenCharacters && !invalidFormat;
    }

    private Result Validate(TranslationEntity translation)
    {
      var placeholder = translation?.Placeholder?.Name;
      return IsPlaceholderValid(placeholder) ? Result.Ok() : Result.Fail($"Placeholder '{placeholder}' has invalid format or contains forbidden characters");
    }

    private object CreateElement(object target, string path, string value)
    {
      var pathElements = path.Split(Delimiter);
      var root = pathElements.FirstOrDefault();

      if (string.IsNullOrWhiteSpace(root))
      {
        return value;
      }

      var newInnerPath = string.Join(Delimiter, pathElements.Skip(1));
      if (target is Dictionary<object, object>)
      {
        var d = target as Dictionary<object, object>;
        if (d.ContainsKey(root))
        {
          d[root] = CreateElement(d[root], newInnerPath, value);
          return d;
        }
        else
        {
          d.Add(root, Create(newInnerPath, value));
          return d;
        }
      }

      return value;
    }

    private object Create(string path, string value)
    {
      var pathElements = path.Split(Delimiter);
      var root = pathElements.FirstOrDefault();

      if (string.IsNullOrWhiteSpace(root))
      {
        return value;
      }

      var newInnerPath = string.Join(Delimiter, pathElements.Skip(1));

      var result = new Dictionary<object, object>
      {
        { root, Create(newInnerPath, value) }
      };

      return result;
    }
  }
}
