namespace DotNetOpen.Data.EntityFramework.Mappings.NameStrategy
{
    public class ToLowerNameStrategy : INameStrategy
    {
        public string ToName(string from)
        {
            return string.IsNullOrEmpty(from)
                       ? from
                       : from.ToLowerInvariant();
        }
    }
}