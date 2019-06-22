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
      var result = new Dictionary<object, object>();
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

    public Result Validate(TranslationEntity translation)
    {
      var placeholder = translation?.Placeholder?.Name;

      var regex = new Regex($"[^A-Za-z0-9{Delimiter}]+");
      var anyForbiddenCharacters = regex.Match(placeholder).Success;
      return anyForbiddenCharacters ? Result.Fail($"Placeholder '{placeholder}' contains forbidden characters") : Result.Ok();
    }

    private dynamic CreateElement(dynamic target, string path, string value)
    {
      var root = path.Split(Delimiter).FirstOrDefault();

      if (string.IsNullOrWhiteSpace(root))
      {
        return value;
      }

      var newInnerPath = path.Replace(root, string.Empty).TrimStart('.');
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
