using BudgetFlow.Application.Common.Models;
using System.Xml.Serialization;

namespace BudgetFlow.Application.Common.Utils
{
    public class CurrencyListSerializer
    {
        public static async Task<CurrencyList> DeserializeAsync(string xmlDocument)
        {
            var serializer = new XmlSerializer(typeof(CurrencyList));
            using var stringReader = new StringReader(xmlDocument);
            return await Task.FromResult(( CurrencyList )serializer.Deserialize(stringReader)!);
        }
    }
}
