using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using System.Linq;

namespace RuukotoBot
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public readonly bool ForAdmin;
        public readonly IEnumerable<string> Names;

        public CommandAttribute(params string[] names) : this(false, names) { }
        public CommandAttribute(bool forAdmin, params string[] names)
        {
            ForAdmin = forAdmin;
            Names = names.Select(x => x.ToLower());
        }
    }
}
