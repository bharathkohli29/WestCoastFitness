namespace WestCoastFitness.Application.Common.Exceptions;

/// <summary>
/// Thrown when a required aggregate cannot be located. Translated by the
/// Api layer's centralized exception handler into a safe 404 response that
/// never leaks internal details.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found.")
    {
    }
}
