﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialManager_Server.Packets
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
       , ClientsQueryReq = 0x17
       , SendMessageReq = 0x19
       , ReadyChatReq = 0x21
       , NewAgendaEventReq = 0x23
       , DeleteAgendaEventReq = 0x24
       , LinkSocialNetworkReq = 0x27
       , DeleteLinkSocialNetReq = 0x29
       // Server side
       , RegisterAck = 0x01
       , LoginAck = 0x03
       , AliveAck = 0x05
       , LogoutAck = 0x07
       , DeleteAccountAck = 0x09
       , ContactAck = 0x13
       , ProfileUpdateAck = 0x16
       , ClientsQueryAck = 0x18
       , SendMessageAck = 0x20
       , ReadyChatAck = 0x22
       , NewAgendaEventAck = 0x25
       , DeleteAgendaEventAck = 0x26
       , LinkSocialNetworkAck = 0x28
       , DeleteLinkSocialNetAck = 0x30
       // Both
       , Error = 0x99
       ,
    }
}
