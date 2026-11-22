using System.Collections.Generic;

namespace Expressions.Task3.E3SQueryProvider.Constants
{
    public static class StringMethods
    {
        public new const string Equals = nameof(string.Equals);
        public const string StartsWith = nameof(string.StartsWith);
        public const string EndsWith = nameof(string.EndsWith);
        public const string Contains = nameof(string.Contains);

        public static readonly Dictionary<string, (bool useLeft, bool useRight)> Patterns = new Dictionary<string, (bool, bool)>
        {
            { Equals, (false, false) },
            { StartsWith, (false, true) },
            { EndsWith, (true, false) },
            { Contains, (true, true) }
        };
    }
}
