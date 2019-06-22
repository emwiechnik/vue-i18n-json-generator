namespace Translations
{
  public interface IError
  {
    string ErrorId { get; set; }
    object Details { get; set; }
    string Message { get; set; }
  }
}
