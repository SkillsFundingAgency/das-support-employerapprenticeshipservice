using System.Text;

namespace Sfa.Das.Console.Core.Helpers
{
    public class PayeSchemeObfuscator : IPayeSchemeObfuscator
    {
        public string ObscurePayeScheme(string payeSchemeId)
        {
            var length = payeSchemeId.Length;

            var response = new StringBuilder(payeSchemeId);

            for (var i = 1; i < length - 1; i++)
            {
                if (response[i].ToString() != "/")
                {
                    response.Remove(i, 1);
                    response.Insert(i, "*");
                }
            }

            return response.ToString();
        }
    }
}
