using LYRA.Server.Enums;
using LYRA.Server.Services.Interfaces;

namespace LYRA.Server.Services.SecurityVerification
{
    public class SignatureStringBuilderFactory
    {
        private readonly IEnumerable<ISignatureStringBuilder> _builders;

        public SignatureStringBuilderFactory(IEnumerable<ISignatureStringBuilder> builders)
        {
            _builders = builders;
        }

        public ISignatureStringBuilder GetBuilder(AccessContext context)
        {
            var builder = _builders.FirstOrDefault(b => b.Context == context);
            if (builder == null)
                throw new NotSupportedException($"Unsupported AccessContext: {context}");
            return builder;
        }
    }
}
