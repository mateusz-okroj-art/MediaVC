namespace MediaVC
{
    public sealed class Ref<T>
    {
        public Ref(T initValue = default!) => value = initValue;

        public T value;

        public static implicit operator T(Ref<T> source) => source.value;
    }
}
