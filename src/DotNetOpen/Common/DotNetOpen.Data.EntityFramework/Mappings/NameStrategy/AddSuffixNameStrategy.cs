namespace DotNetOpen.Data.EntityFramework.Mappings.NameStrategy
{
    public class AddSuffixNameStrategy : INameStrategy
    {
        private readonly string suffix;
        public AddSuffixNameStrategy(string suffix)
        {
            this.suffix = suffix;
        }

        public string ToName(string from)
        {
            return from == null ? suffix : from + suffix;
        }
    }
}