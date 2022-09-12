namespace TeduMicroservices.IDP.Common.Exceptions;

public class EntityNotFoundException : ApplicationException
{
    public EntityNotFoundException() : base("Entity was not found.")
    {
    }
}