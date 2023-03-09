namespace MediaVC
{
    public readonly struct Result<Tresult> : IActionResult<Tresult>
    {
        public Result(Tresult value) { Value = value; }

        public Tresult Value { get; }
    }
}
