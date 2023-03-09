namespace MediaVC
{
    /// <summary>
    /// Groups all types of results
    /// </summary>
    public interface IActionResult
    {
    }

    /// <summary>
    /// Groups results when value is expected
    /// </summary>
    /// <typeparam name="Tresult"></typeparam>
    public interface IActionResult<Tresult> : IActionResult
    {
    }
}
