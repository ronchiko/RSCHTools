namespace RCSHTools.Parsing
{
    /// <summary>
    /// An automata in which the final state index doesn't matter
    /// </summary>
    public interface IAutomata<T>
    {
        /// <summary>
        /// The states of the automata
        /// </summary>
        IAutomataState<T>[] States { get; }
        /// <summary>
        /// Passes a value to the automata return true if it passed it
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        bool Pass(StringParser sp);
    }
    /// <summary>
    /// An automata in which the final state index matters
    /// </summary>
    public interface IMultiAutomata<T> : IAutomata<T>
    {
        /// <summary>
        /// <inheritdoc cref="IAutomata{T}.Pass(StringParser)"/>, and outputs the final state
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        bool Pass(StringParser sp, out int state);
    }

    /// <summary>
    /// An automata state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAutomataState<T>
    {
        /// <summary>
        /// Returns the index of the next state according to the given input
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        int Read(T c);
    }
}
