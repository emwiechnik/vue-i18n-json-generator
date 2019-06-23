using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Translations.Entities;
using Xunit;

namespace Translations.Tests
{
  public class TranslationJsonGenerator_Should
  {
    [Theory]
    [MemberData(nameof(CorrectTestDataSets))]
    public void Generate_Output_That_Reflects_The_Input(IEnumerable<TranslationEntity> input)
    {
      // Arrange
      var subject = new TranslationJsonGenerator();

      // Act 
      var jsonResult = subject.GenerateJsonString(input.ToList());
      var objFromJson = JsonConvert.DeserializeObject<JObject>(jsonResult.Value);
      var output = GetTranslations(objFromJson);

      // Assert
      output.Should().BeEquivalentTo(input);
    }

    [Theory]
    [MemberData(nameof(CorrectAndIncorrectTestDataSets))]
    public void Fail_When_Input_Is_Invalid(IEnumerable<TranslationEntity> input, bool isValid, string _)
    {
      // Arrange
      var subject = new TranslationJsonGenerator();

      // Act 
      var intermediateResult = subject.GenerateJsonString(input.ToList());

      // Assert
      intermediateResult.IsSuccess.Should().Be(isValid);
    }

    [Theory]
    [MemberData(nameof(CorrectAndIncorrectTestDataSets))]
    public void Return_Correct_Validation_Messages_When_Input_Is_Invalid(IEnumerable<TranslationEntity> input, bool _, string validationMessage)
    {
      // Arrange
      var subject = new TranslationJsonGenerator();

      // Act 
      var intermediateResult = subject.GenerateJsonString(input.ToList());

      // Assert
      if (!intermediateResult.IsSuccess)
      {
        intermediateResult.Error.Message.Should().Be(validationMessage);
      }
    }

    private IEnumerable<TranslationEntity> GetTranslations(JObject translationsObj)
    {
      IEnumerable<JToken> jTokens = translationsObj.Descendants().Where(p => p.Count() == 0);

      foreach (var token in jTokens)
      {
        yield return new TranslationEntity { Placeholder = new PlaceholderEntity(token.Path), Text = token.ToString() };
      }
    }

    public static IEnumerable<object[]> CorrectTestDataSets =>
      new List<object[]>
      {
        new []
        {
          new [] {
            new TranslationEntity { Placeholder = new PlaceholderEntity("common.buttons.readMore"), Text = "Read more" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("common.tooltips.goToChat"), Text = "Click to chat with [personName]" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("dataTable.noDataAvailableText"), Text = "No data available" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("dataTable.noSearchResults"), Text = "Could not find any matching data" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("dataTable.search"), Text = "Search..." }
          }
        },
        new []
        {
          new [] {
            new TranslationEntity { Placeholder = new PlaceholderEntity("admin.admin.tabs.adminData.admin"), Text = "Value1" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("dataTable.data.table.data"), Text = "Value2" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("admin.chat.buttons.send.description"), Text = "Value3" }
          }
        },
        new []
        {
          new [] {
            new TranslationEntity { Placeholder = new PlaceholderEntity("A1.B1.C1"), Text = "Value1" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("A1.B1.C2"), Text = "Value2" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("A1.B1.C3"), Text = "Value3" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("A1.B2.C1"), Text = "Value4" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("A1.B3.C1.D1"), Text = "Value5" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("A1.B3.C2.D2"), Text = "Value6" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("A1.B4.C3.D1.E1"), Text = "Value7" }
          }
        }
      };

    public static IEnumerable<object[]> CorrectAndIncorrectTestDataSets =>
      new List<object[]>
      {
        new object []
        {
          new [] {
            new TranslationEntity { Placeholder = new PlaceholderEntity("common,buttons.readMore"), Text = "Read more" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("common.tooltips.goToChat"), Text = "Click to chat with [personName]" }
          },
          false,
          "Placeholder 'common,buttons.readMore' has invalid format or contains forbidden characters"
        },
        new object []
        {
          new [] {
            new TranslationEntity { Placeholder = new PlaceholderEntity("common.buttons.readMore"), Text = "Read more" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("common.tooltips;goToChat"), Text = "Click to chat with [personName]" }
          },
          false,
          "Placeholder 'common.tooltips;goToChat' has invalid format or contains forbidden characters"
        },
        new object []
        {
          new [] {
            new TranslationEntity { Placeholder = new PlaceholderEntity("common.buttons.readMore"), Text = "Read more" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("common.tooltips.goToChat"), Text = "Click to chat with [personName]" }
          },
          true,
          string.Empty
        },
        new object []
        {
          new [] {
            new TranslationEntity { Placeholder = new PlaceholderEntity("common.buttons.readMore"), Text = "Read more" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("common.tooltips/goToChat"), Text = "Click to chat with [personName]" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("dataTable.noDataAvailableText"), Text = "No data available" }
          },
          false,
          "Placeholder 'common.tooltips/goToChat' has invalid format or contains forbidden characters"
        },
        new object []
        {
          new [] {
            new TranslationEntity { Placeholder = new PlaceholderEntity("some.thing.nested.in.a.nested.some.thing"), Text = "A thing" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("thing.some.nested.a.in.nested.thing.some"), Text = "Something" }
          },
          true,
          string.Empty
        },
        new object []
        {
          new [] {
            new TranslationEntity { Placeholder = new PlaceholderEntity("some.thing..nested.in.a.nested.some.thing"), Text = "A thing" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("thing.some.nested.a.in.nested.thing.some"), Text = "Something" }
          },
          false,
          "Placeholder 'some.thing..nested.in.a.nested.some.thing' has invalid format or contains forbidden characters"
        }
      };
  }
}
