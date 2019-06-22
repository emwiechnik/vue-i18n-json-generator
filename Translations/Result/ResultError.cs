namespace Translations
{
  public class ResultError : IError
  {
    public ResultError(string errorId, string message, object details = default(object))
    {
      ErrorId = errorId;
      Details = details;
      Message = message;
    }

    public string ErrorId { get; set; }
    public object Details { get; set; }
    public string Message { get; set; }
  }
}
