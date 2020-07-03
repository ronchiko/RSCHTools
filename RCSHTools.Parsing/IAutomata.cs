namespace RCSHTools.Parsing
{
    /// <summary>
    /// An automata in which the final state index doesn't matter
    /// </summary>
    public interface IAutomata<T>
    {
        IAutomataState<T>[] States { get; }
        bool Pass(StringParser sp);
    }
    /// <summary>
    /// An automata in which the final state index matters
    /// </summary>
    public interface IMultiAutomata<T> : IAutomata<T>
    {
        bool Pass(StringParser sp, out int state);
    }

    /// <summary>
    /// An automata state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAutomataState<T>
    {
        int Read(T c);
    }
}
