﻿// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Context
{
    using System;
    using System.IO;


    public class MoveMessageSendContext :
        OldMessageContext,
        ISendContext
    {
        readonly Action<Stream> _bodyWriter;
        readonly Action<EndpointAddress> _notifySend;

        public MoveMessageSendContext(IReceiveContext context)
        {
            SetUsing(context);
            CopyOrInitializeOriginalMessageId(context);

            Id = context.Id;

            _notifySend = address => context.NotifySend(this, address);

            _bodyWriter = stream => context.CopyBodyTo(stream);
        }

        public Guid Id { get; set; }

        public Type DeclaringMessageType
        {
            get { return typeof(object); }
        }

        public void SetDeliveryMode(DeliveryMode deliveryMode)
        {
            DeliveryMode = deliveryMode;
        }

        public DeliveryMode DeliveryMode { get; private set; }

        public void SerializeTo(Stream stream)
        {
            _bodyWriter(stream);
        }

        public void NotifySend(EndpointAddress address)
        {
            _notifySend(address);
        }

        void CopyOrInitializeOriginalMessageId(IReceiveContext context)
        {
            SetOriginalMessageId(context.OriginalMessageId);

            if (string.IsNullOrEmpty(OriginalMessageId))
                SetOriginalMessageId(context.MessageId);
        }
    }
}