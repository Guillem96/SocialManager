using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Client.Packets
{
    enum PacketTypes
    {
        // Client side
        RegisterReq = 0x00
        , LoginReq = 0x02
        , AliveInf = 0x04
        // Server side
        , RegisterAck = 0x01
        , LoginAck = 0x03
        , AliveAck = 0x05
        // Both
        , Error = 0x99
        ,
    }
}
