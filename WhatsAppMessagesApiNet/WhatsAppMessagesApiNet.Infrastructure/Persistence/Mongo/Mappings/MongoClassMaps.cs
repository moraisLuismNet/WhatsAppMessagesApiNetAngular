using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using WhatsAppMessagesApiNet.Domain.Entities;
using WhatsAppMessagesApiNet.Domain.Enums;

namespace WhatsAppMessagesApiNet.Infrastructure.Persistence.Mongo.Mappings;

public static class MongoClassMaps
{
    private static bool _initialized;

    public static void Register()
    {
        if (_initialized) return;
        _initialized = true;

        BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(typeof(DateTime), new DateTimeSerializer(DateTimeKind.Utc, BsonType.DateTime));

        if (!BsonClassMap.IsClassMapRegistered(typeof(User)))
        {
            BsonClassMap.RegisterClassMap<User>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdMember(x => x.Email);
                map.MapMember(x => x.Role).SetSerializer(new EnumSerializer<UserRole>(BsonType.String));
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(Message)))
        {
            BsonClassMap.RegisterClassMap<Message>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapMember(x => x.Status).SetSerializer(new EnumSerializer<MessageStatus>(BsonType.String));
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(AuditLog)))
        {
            BsonClassMap.RegisterClassMap<AuditLog>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapMember(x => x.Action).SetSerializer(new EnumSerializer<AuditAction>(BsonType.String));
            });
        }
    }
}
