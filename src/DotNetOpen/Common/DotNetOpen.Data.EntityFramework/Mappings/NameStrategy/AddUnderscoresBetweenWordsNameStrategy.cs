using System.Text;

namespace DotNetOpen.Data.EntityFramework.Mappings.NameStrategy
{
    public class AddUnderscoresBetweenWordsNameStrategy : INameStrategy
    {
        public string ToName(string from)
        {
            if (string.IsNullOrEmpty(from))
                return from;
            var chars = from.ToCharArray();
            var sb = new StringBuilder(chars.Length);

            var prev = 'A';
            foreach (var c in chars)
            {
                if (c != '_' && char.IsUpper(c) && !char.IsUpper(prev))
                {
                    sb.Append('_');
                }
                sb.Append(c);
                prev = c;
            }

            return sb.ToString();
        }
    }
}