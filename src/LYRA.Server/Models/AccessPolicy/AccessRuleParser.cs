using LYRA.Server.Utilities;

namespace LYRA.Server.Models.AccessPolicy
{
    public static class AccessRuleParser
    {
        public static List<AccessRule> Parse(string operation)
        {
            return DelimitedStringParser.Parse(operation)
                .Select(op =>
                {
                    var parsed = OperationParser.ParseSingle(op);

                    return new AccessRule
                    {
                        Method = OperationParser.NormalizeMethod(parsed.Method),
                        PathPattern = OperationParser.NormalizePath(parsed.PathPattern)
                    };
                })
                .ToList();
        }
    }
}
