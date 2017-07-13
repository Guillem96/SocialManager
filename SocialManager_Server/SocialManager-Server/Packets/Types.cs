using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Server.Packets
{
    enum PacketTypes    {
                          // Client side
                          RegisterReq = 0x00
                        , LoginReq = 0x02
                          // Server side
                        , RegisterAck = 0x01
                        , LoginAck = 0x03
                          // Both
                        , Error = 0x99
                        ,
                        }
}
