using System;
using System.Collections.Generic;
using System.Text;

namespace RCSHTools
{
    /// <summary>
    /// An argument that must be inputed into the call
    /// </summary>
    public struct IndexedArgument : IArgument
    {
        bool IArgument.IsIndexed => true;
        bool IArgument.IsFlag => false;

        /// <summary>
        /// <inheritdoc cref="IArgument.Name"/>
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// <inheritdoc cref="IArgument.Help"/>
        /// </summary>
        public string Help { get; }
        /// <summary>
        /// <inheritdoc cref="IArgument.Checker"/>
        /// </summary>
        public Predicate<string> Checker { get; }
        /// <summary>
        /// The type of the variable
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Creates a new indexed argument
        /// </summary>
        /// <param name="name"></param>
        /// <param name="help"></param>
        /// <param name="type"></param>
        /// <param name="checker"></param>
        public IndexedArgument(string name, string help, Type type, Predicate<string> checker)
        {
            Name = name;
            Help = help;
            Checker = checker ?? ArgumentCheckers.Any;
            Type = type;
        }

        object IArgument.TryGet(string v)
        {
            if (!Checker.Invoke(v)) return false;
            return v;
        }
    }

    /// <summary>
    /// An argument that can be toggled on and off
    /// </summary>
    public struct FlagArgument : IArgument
    {
        Type IArgument.Type => typeof(bool);
        bool IArgument.IsIndexed => false;
        bool IArgument.IsFlag => true;

        /// <summary>
        /// The name of the flag => also what triggers it
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Help about the flag
        /// </summary>
        public string Help { get; }
        /// <summary>
        /// <inheritdoc cref="IArgument.Checker"/>
        /// </summary>
        public Predicate<string> Checker { get; }

        /// <summary>
        /// Creates a new flag argument
        /// </summary>
        /// <param name="name"></param>
        /// <param name="help"></param>
        public FlagArgument(string name, string help = null)
        {
            Name = name;
            Help = help;
            Checker = ArgumentCheckers.Any;
        }

        object IArgument.TryGet(string v)
        {
            return true;
        }
    }

    /// <summary>
    /// An argument that is optional to the process
    /// </summary>
    public struct OptionalArgument : IArgument
    {
        private string defaultValue;

        /// <summary>
        /// The type of the argument
        /// </summary>
        public Type Type { get; }
        /// <summary>
        /// The name of the argument
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Help data about the argument
        /// </summary>
        public string Help { get; }
        bool IArgument.IsIndexed => false;
        bool IArgument.IsFlag => false;
        /// <summary>
        /// <inheritdoc cref="IArgument.Checker"/>
        /// </summary>
        public Predicate<string> Checker { get; }


        /// <summary>
        /// Creates a new optional argument
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dValue"></param>
        /// <param name="type"></param>
        /// <param name="checker"></param>
        /// <param name="help"></param>
        public OptionalArgument(string name, string dValue, Type type, Predicate<string> checker, string help)
        {
            this.Help = help;
            this.Name = name;
            this.Type = type;
            this.Checker = checker ?? ArgumentCheckers.Any;
            defaultValue = dValue;
        }

        object IArgument.TryGet(string v)
        {
            if (Checker(v)) return v;
            return defaultValue;
        }
    }
}
