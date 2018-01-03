using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.OData.Core;
using System.Threading.Tasks;

namespace CoralTime.Serialization
{
    public class ODataEntityTypeSerializer : Microsoft.AspNetCore.OData.Formatter.Serialization.ODataEntityTypeSerializer
    {
       public ODataEntityTypeSerializer(Microsoft.AspNetCore.OData.Formatter.Serialization.ODataSerializerProvider serializerProvider)
        : base(serializerProvider)
        {
        }

        public override async Task<ODataEntry> CreateEntryAsync(SelectExpandNode selectExpandNode, EntityInstanceContext entityInstanceContext)
        {
            ODataEntry entry = await base.CreateEntryAsync(selectExpandNode, entityInstanceContext);
            return ObjectEntryConverter.Convert(entry);
        }
    }
}
