using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.InMemory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer3.EntityFramework.Serialization
{
    public class UserLite
    {
        public string Subject { get; set; }
    }

    public class UserConverter: JsonConverter
    {
        //private readonly IUserService userService;

        //public UserConverter(IUserService userService)
        //{
        //    if (userService == null) throw new ArgumentNullException("userService");

        //    this.userService = userService;
        //}

        public override bool CanConvert(Type objectType)
        {
            return typeof(InMemoryUser) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var source = serializer.Deserialize<InMemoryUser>(reader);
            //var users = AsyncHelper.RunSync(async () => await userService.FindUsersAsync(new string[] { source.Subject }));
            return source;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var source = (InMemoryUser)value;

            var target = new UserLite
            {
                Subject = source.Subject
            };
            serializer.Serialize(writer, target);
        }
    }
}
