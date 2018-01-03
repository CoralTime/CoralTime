using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.OData.Edm;

namespace CoralTime.Serialization
{
    public class ODataSerializerProvider : DefaultODataSerializerProvider
    {
        public override ODataEdmTypeSerializer GetEdmTypeSerializer(IEdmTypeReference edmType)
        {
            switch (edmType.TypeKind())
            {
                case EdmTypeKind.Enum:
                    ODataEdmTypeSerializer enumSerializer = base.GetEdmTypeSerializer(edmType);
                    return enumSerializer;

                case EdmTypeKind.Primitive:
                    ODataEdmTypeSerializer primitiveSerializer = base.GetEdmTypeSerializer(edmType);
                    return primitiveSerializer;

                case EdmTypeKind.Collection:
                    IEdmCollectionTypeReference collectionType = edmType.AsCollection();
                    if (collectionType.ElementType().IsEntity())
                    {
                        ODataEdmTypeSerializer feedSerializer = base.GetEdmTypeSerializer(edmType);
                        return feedSerializer;
                    }
                    else
                    {
                        ODataEdmTypeSerializer collectionSerializer = base.GetEdmTypeSerializer(edmType);
                        return collectionSerializer;
                    }

                case EdmTypeKind.Complex:
                    ODataEdmTypeSerializer complexTypeSerializer = base.GetEdmTypeSerializer(edmType);
                    return complexTypeSerializer;

                case EdmTypeKind.Entity:
                    ODataEdmTypeSerializer entityTypeSerializer = new ODataEntityTypeSerializer(this);
                    return entityTypeSerializer;

                default:
                    return null;
            }
        }
    }
}
