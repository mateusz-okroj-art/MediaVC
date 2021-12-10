namespace Xunit.Extensions
{
    public class RandomValueAttribute<Tvalue> : ClassDataAttribute
    {
        public RandomValueAttribute(Tvalue from, Tvalue to) : base(typeof(Tvalue))
        {

        }
    }
}