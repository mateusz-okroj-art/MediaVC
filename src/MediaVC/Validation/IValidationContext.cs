namespace MediaVC.Validation
{
    public interface IValidationContext
    {
        void AddError(string field, string message);
    }
}
