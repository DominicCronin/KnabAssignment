#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ExchangeRates.Server.Exceptions;
#pragma warning restore IDE0130 // Namespace does not match folder structure


[Serializable]
public class ValidationException : Exception
{
	public ValidationException() { }
	public ValidationException(string message) : base(message) { }
	public ValidationException(string message, Exception inner) : base(message, inner) { }
}


[Serializable]
public class UpstreamServiceException : Exception
{
	public UpstreamServiceException() { }
	public UpstreamServiceException(string message) : base(message) { }
	public UpstreamServiceException(string message, Exception inner) : base(message, inner) { }

	public UpstreamServiceException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; init; }
}

[Serializable]
public class UpstreamBadRequestException : UpstreamServiceException
{
	public UpstreamBadRequestException() { }
	public UpstreamBadRequestException(string message) : base(message) {
		StatusCode = StatusCodes.Status400BadRequest;
	}
	public UpstreamBadRequestException(string message, Exception inner) : base(message, inner) { }

}