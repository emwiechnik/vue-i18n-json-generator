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
    public void Generate_Output_That_Is_Equivalent_To_Input(IEnumerable<TranslationEntity> input)
    {
      // Arrange
      var subject = new TranslationJsonGenerator();

      // Act 
      var intermediateResult = subject.CreateTranslationsObject(input.ToList());
      var serializedObj = JsonConvert.SerializeObject(intermediateResult.Value);
      var deserializedObj = JsonConvert.DeserializeObject<JObject>(serializedObj);
      var output = GetTranslations(deserializedObj);

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
      var intermediateResult = subject.CreateTranslationsObject(input.ToList());

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
      var intermediateResult = subject.CreateTranslationsObject(input.ToList());

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
            new TranslationEntity { Placeholder = new PlaceholderEntity("mainDashboard.tabs.personalSettings"), Text = "Personal settings" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("mainDashboard.tabs.agreementsAndRules"), Text = "Agreements and rules" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("mainDashboard.tabs.businessCard"), Text = "Business card" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("mainDashboard.bannersArea.smallBanners.tipsAndTricks"), Text = "Tips" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("mainDashboard.bannersArea.smallBanners.specialOffers"), Text = "Offers" }
          }
        },
        new []
        {
          new [] {
            new TranslationEntity { Placeholder = new PlaceholderEntity("mainDashboard.tabs.personalSettings"), Text = "Personal settings" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("mainDashboard.tabs.agreementsAndRules"), Text = "Agreements and rules" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("adminDashboard.tabs.admin.adminChat"), Text = "Chat" }
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
            new TranslationEntity { Placeholder = new PlaceholderEntity("mainDashboard,tabs.personalSettings"), Text = "Personal settings" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("mainDashboard.tabs.agreementsAndRules"), Text = "Agreements and rules" }
          },
          false,
          "Placeholder 'mainDashboard,tabs.personalSettings' contains forbidden characters"
        },
        new object []
        {
          new [] {
            new TranslationEntity { Placeholder = new PlaceholderEntity("mainDashboard.tabs.personalSettings"), Text = "Personal settings" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("mainDashboard.tabs;agreementsAndRules"), Text = "Agreements and rules" }
          },
          false,
          "Placeholder 'mainDashboard.tabs;agreementsAndRules' contains forbidden characters"
        },
        new object []
        {
          new [] {
            new TranslationEntity { Placeholder = new PlaceholderEntity("mainDashboard.tabs.personalSettings"), Text = "Personal settings" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("mainDashboard.tabs.agreementsAndRules"), Text = "Agreements and rules" }
          },
          true,
          string.Empty
        },
        new object []
        {
          new [] {
            new TranslationEntity { Placeholder = new PlaceholderEntity("mainDashboard.tabs.personalSettings"), Text = "Personal settings" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("mainDashboard.tabs/agreementsAndRules"), Text = "Agreements and rules" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("mainDashboard.tabs;bus]inessCard"), Text = "Agreements and rules" }
          },
          false,
          "Placeholder 'mainDashboard.tabs/agreementsAndRules' contains forbidden characters"
        },
        new object []
        {
          new [] {
            new TranslationEntity { Placeholder = new PlaceholderEntity("mainDashboard.tabs.otherSettings"), Text = "Personal settings" },
            new TranslationEntity { Placeholder = new PlaceholderEntity("mainDashboard.tabs.yetSomeOtherTab"), Text = "Agreements and rules" }
          },
          true,
          string.Empty
        },
      };
  }
}
