using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HMS.Shared.SharedEnums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RoomStatus
    {
        Available,
        Reserved,
        Maintenance,
        NonExist
    }
}
