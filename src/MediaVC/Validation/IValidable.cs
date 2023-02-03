namespace MediaVC.Validation
{
    public interface IValidable
    {
        bool Validate(IValidationContext validationContext);
    }
}
