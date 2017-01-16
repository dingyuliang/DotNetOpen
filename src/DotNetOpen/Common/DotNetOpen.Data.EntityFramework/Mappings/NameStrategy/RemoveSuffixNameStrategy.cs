using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetOpen.Data.EntityFramework.Mappings.NameStrategy
{
    public class RemoveSuffixNameStrategy : INameStrategy
    {
        private string suffixToRemove;

        public RemoveSuffixNameStrategy(string suffixToRemove)
        {
            this.suffixToRemove = suffixToRemove;
        }

        public string ToName(string from)
        {
            if (!string.IsNullOrWhiteSpace(from) 
                && from.EndsWith(suffixToRemove, StringComparison.InvariantCultureIgnoreCase))
            {
                return from.Substring(0, from.LastIndexOf(suffixToRemove, StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                return from;
            }
        }
    }
}
