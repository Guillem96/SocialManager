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
       , LogoutReq = 0x06
       , DeleteAccountReq = 0x08
       , NewContactReq = 0x10
       , AcceptNewContact = 0x11
       , RegNewContact = 0x12
       , ListContactReq = 0x14
       , ProfileUpdateReq = 0x15
       // Server side
       , RegisterAck = 0x01
       , LoginAck = 0x03
       , AliveAck = 0x05
       , LogoutAck = 0x07
       , DeleteAccountAck = 0x09
       , ContactAck = 0x13
       , ProfileUpdateAck = 0x16
       // Both
       , Error = 0x99
       ,
    }
}
