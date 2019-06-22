namespace Translations
{
  public class Result
  {
    public Result(bool isSuccess, IError error)
    {
      IsSuccess = isSuccess;
      Error = error;
    }

    public bool IsSuccess { get; private set; }
    public IError Error { get; set; }
    public TError GetError<TError>() where TError: IError
    {
      return (TError)Error;
    }

    static public Result Ok()
    {
      return new Result(true, null);
    }

    static public Result Fail(string errorMessage)
    {
      return new Result(false, new ResultError(string.Empty, errorMessage));
    }

    static public Result Fail(IError error)
    {
      return new Result(false, error);
    }

    static public Result<TValue> Ok<TValue>(TValue value)
    {
      return new Result<TValue>(value, true, null);
    }

    static public Result<TValue> Fail<TValue>(IError error)
    {
      return new Result<TValue>(default(TValue), false, error);
    }

    static public Result<TValue> Fail<TValue>(string errorMessage)
    {
      return new Result<TValue>(default(TValue), false, new ResultError(string.Empty, errorMessage));
    }
  }

  public class Result<TValue>: Result
  {
    public Result(TValue value, bool isSuccess, IError error): base(isSuccess, error)
    {
      Value = value;
    }

    public TValue Value { get; set; }
  }
}
