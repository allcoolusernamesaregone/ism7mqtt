﻿using System;
using System.Collections.Generic;
using System.Linq;
using ism7mqtt.ISM7.Protocol;
using Newtonsoft.Json.Linq;

namespace ism7mqtt.ISM7.Xml
{
    public class SolarStatisticConverterTemplate : MultiTelegramConverterTemplateBase
    {
        private const string KwType = "SolarertragW_kW";
        private const string KwMwType = "SolarertragW_kW_MW";
        private readonly List<(ushort, uint)> _values = new();

        public override void AddTelegram(ushort telegram, byte low, byte high)
        {
            if (Type == KwMwType || Type == KwType)
            {
                _values.Add((telegram, (uint)(high << 8) | low));
            }
            else
            {
                Console.Error.WriteLine($"SolarStatisticConverter({Type}): T:{telegram} H:{high} L:{low}");
            }
        }

        public override bool HasValue => Type switch
        {
            KwType => _values.Count == 2,
            KwMwType => _values.Count == 3,
            _ => false
        };

        public override JValue GetValue()
        {
            var values = _values.OrderBy(x => x.Item1).ToList();
            var wh = values[0].Item2;
            var kwh = values[1].Item2;
            var value = wh + (1_000UL * kwh);
            if (Type == KwMwType)
            {
                var mwh = values[2].Item2;
                value += mwh * 1_000_000UL;
            }
            _values.Clear();
            return new JValue(value);
        }

        public override IEnumerable<InfoWrite> GetWrite(JValue value)
        {
            throw new NotImplementedException($"CTID '{CTID}' is not yet implemented");
        }
    }
}