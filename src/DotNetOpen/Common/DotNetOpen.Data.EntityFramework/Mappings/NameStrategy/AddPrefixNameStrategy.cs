namespace DotNetOpen.Data.EntityFramework.Mappings.NameStrategy
{
    public class AddPrefixNameStrategy : INameStrategy
    {
        private readonly string prefix;
        public AddPrefixNameStrategy(string prefix)
        {
            this.prefix = prefix;
        }

        public string ToName(string from)
        {
            return prefix == null ? from : prefix + from;
        }
    }

}