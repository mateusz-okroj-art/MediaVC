namespace MediaVC
{
    public class Failure : IActionResult
    {

    }

    public class Failure<T> : Failure, IActionResult<T>
    {

    }
}
