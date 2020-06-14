using System;
using System.Collections.Generic;
using System.Text;

namespace RCSHTools
{
    /// <summary>
    /// Adds customizbilty for parsing command line arguments
    /// </summary>
    public class ArgumentHandler
    {
        private readonly List<IArgument> arguments;
        private readonly string description;
        private readonly string command;


        /// <summary>
        /// Creates a new empty argument handler
        /// </summary>
        /// <param name="command">Name of the command</param>
        /// <param name="description"></param>
        public ArgumentHandler(string command, string description = null)
        {
            this.command = command;
            arguments = new List<IArgument>();
            this.description = description;
        }

        private void AddArgument(IArgument argument)
        {
            arguments.Add(argument);
        }
        private void Reorganize()
        {
            for (int i = 0; i < arguments.Count; i++)
            {
                if(arguments[i].IsIndexed)
                {
                    int v = i;
                    while(v > 0 && !arguments[v - 1].IsIndexed)
                    {
                        IArgument te = arguments[v - 1];
                        arguments[v - 1] = arguments[v];
                        arguments[v] = te;
                        
                        v--;
                    }
                }
            }
        }

        private IArgument SearchByName(string name)
        {
            foreach (var argument in arguments)
                if (!argument.IsIndexed && argument.Name == name) return argument;
            return null;
        }
        private IArgument GetIndexed(ref int index)
        {
            if (arguments[index].IsIndexed)
            {
                return arguments[index++];
            }
            return null;
        }
        private Dictionary<string, object> CreateDefaultDict()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (IArgument argument in arguments)
                dict.Add(argument.Name, argument.TryGet(""));
            return dict;
        }

        /// <summary>
        /// Adds an indexed argument to the handler
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="checker"></param>
        /// <param name="help"></param>
        public void AddIndexedArguement(string name, Type type, Predicate<string> checker = null, string help = null)
        {
            AddArgument(new IndexedArgument(name, help, type, checker));
        }
        /// <summary>
        /// Adds a flag to the handler
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="help"></param>
        public void AddFlag(string flag, string help = null)
        {
            AddArgument(new FlagArgument(flag, help));
        }
        /// <summary>
        /// Adds an optional argument
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="checker"></param>
        /// <param name="defaultValue"></param>
        /// <param name="help"></param>
        public void AddArgument(string name, Type type, Predicate<string> checker = null, string defaultValue = null, string help = null)
        {
            AddArgument(new OptionalArgument(name, defaultValue, type, checker, help));
        }

        private void PrintTitle(string msg)
        {
            ConsoleColor fg = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg);
            Console.ForegroundColor = fg;
        }

        /// <summary>
        /// Prints the argument detail of this handler
        /// </summary>
        public void PrintArguments(bool reorganize = true)
        {
            if (reorganize) Reorganize();

            ConsoleColor fg = Console.ForegroundColor;
            ConsoleColor bg = Console.BackgroundColor;

            bool eoa = false; 
            PrintTitle("Arguments:");
            foreach (IArgument argument in arguments)
            {
                Console.Write("\t");
                if (argument.IsIndexed)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("[Hidden: ");
                    Console.Write(argument.Name + "]");
                    Console.ForegroundColor = fg;
                }
                else
                {
                    if (!eoa)
                    {
                        Console.WriteLine();
                        PrintTitle("Optional Arguments:");
                        Console.Write("\t");
                        eoa = true;
                    }

                    if (argument.IsFlag)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("[Flag] ");
                        Console.ForegroundColor = fg;
                    }
                    Console.Write(argument.Name);
                }
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(" " + argument.Type.Name);
                Console.ForegroundColor = fg;

                if (argument.Help != null) Console.WriteLine(": " + argument.Help);
                else Console.WriteLine();
            }

            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;
        }

        /// <summary>
        /// Prints the command header
        /// </summary>
        public void PrintHeader()
        {
            Reorganize();
            ConsoleColor fg = Console.ForegroundColor;
            ConsoleColor bg = Console.BackgroundColor;

            Console.Write(command + " ");
            int i = 0;
            IArgument arg;
            while ((arg = GetIndexed(ref i)) != null)
            {
                Console.Write("[" + arg.Name + ": " + arg.Type.Name + "] ");
            }
            Console.WriteLine();

            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;
        }

        /// <summary>
        /// Prints the help to the console
        /// </summary>
        public void PrintHelp()
        {
            Reorganize();
            ConsoleColor fg = Console.ForegroundColor;
            ConsoleColor bg = Console.BackgroundColor;

            PrintHeader();
            if (description != null)
            {
                Console.WriteLine();
                PrintTitle("Description: ");
                StringBuilder sb = new StringBuilder("\t");
                int line = 0;
                foreach (char d in description)
                {
                    sb.Append(d);
                    line++;
                    if(line > 50 && char.IsWhiteSpace(d))
                    {
                        line = 0;
                        sb.Append("\n\t");
                    } 
                }

                Console.WriteLine(sb.ToString());
            }
            Console.WriteLine();

            PrintArguments();

            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;
        }

        /// <summary>
        /// Parses the arguments
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Dictionary<string, object> Parse(string[] args)
        {
            Reorganize();
            Dictionary<string, object> dict = CreateDefaultDict();

            int indexed = 0;

            for (int i = 0; i < args.Length; i++)
            {
                IArgument argument = SearchByName(args[i]);
                argument = argument ?? GetIndexed(ref indexed);
                if (argument != null && !dict.ContainsKey(argument.Name))
                {
                    if (!argument.IsFlag && !argument.IsIndexed)
                        i++;
                    dict.Add(argument.Name, argument.TryGet(args[i]));
                }
            }

            return dict;
        }
    }

    /// <summary>
    /// Represents an argument
    /// </summary>
    public interface IArgument
    {
        /// <summary>
        /// The type of the argument
        /// </summary>
        Type Type { get; }
        /// <summary>
        /// The name of the argument
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Provides help on this argument
        /// </summary>
        string Help { get; }
        /// <summary>
        /// Is this argument an indexed argument
        /// </summary>
        bool IsIndexed { get; }
        /// <summary>
        /// Is this argument a flag
        /// </summary>
        bool IsFlag { get; }
        /// <summary>
        /// Checks if an argument is valid
        /// </summary>
        Predicate<string> Checker { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        object TryGet(string v);
    }
}
