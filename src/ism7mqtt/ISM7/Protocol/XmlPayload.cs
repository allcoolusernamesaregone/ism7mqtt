﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ism7mqtt.ISM7.Protocol
{
    public abstract class XmlPayload : IPayload
    {
        private static readonly ConcurrentDictionary<Type, XmlSerializer> _serializers = new ConcurrentDictionary<Type, XmlSerializer>();

        public byte[] Serialize()
        {
            using var stringWriter = new Utf8StringWriter();
            var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings {Indent = false});

            var serializer = _serializers.GetOrAdd(GetType(), x => new XmlSerializer(x));
            serializer.Serialize(xmlWriter, this);
            stringWriter.Flush();
            var xml = stringWriter.ToString();
            return Encoding.UTF8.GetBytes(xml);
        }

        public abstract PayloadType Type { get; }
    }
}