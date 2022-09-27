using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Collections.Concurrent;
using System.Globalization;

namespace LanMonitor
{
    /// <summary>
    /// Manu Helper
    /// </summary>
    /// <remarks>
    /// From https://github.com/PingmanTools/MacAddressVendorLookup
    /// </remarks>
    public class ManuHelper
    {
        public class MacVendorInfo
        {
            Lazy<string> _identiferString;
            public string IdentiferString => _identiferString.Value;
            public long Identifier { get; private set; }
            public byte MaskLength { get; private set; }
            public string Organization { get; private set; }

            public MacVendorInfo(long identifier, byte maskLength, string organization)
            {
                Identifier = identifier;
                MaskLength = maskLength;
                Organization = organization;
                _identiferString = new Lazy<string>(() => GetIdentiferString(Identifier, MaskLength));
            }
            static string GetIdentiferString(long identifer, byte maskLength)
            {
                var bytes = BitConverter.GetBytes(IPAddress.NetworkToHostOrder(identifer)).Take((int)Math.Ceiling(maskLength / 8d)).ToArray();
                var str = BitConverter.ToString(bytes).Replace('-', ':');
                if (maskLength % 8 != 0)
                {
                    str += $"/{maskLength}";
                }
                return str;
            }

            public override string ToString()
            {
                return $"[MacVendorInfo: IdentiferString={IdentiferString}, Organization={Organization}]";
            }
        }
        public bool IsInitialized { get; private set; }

        List<MacVendorInfo> _entries = new List<MacVendorInfo>();

        public async Task Init(Stream manufData)
        {
            IsInitialized = false;
            _entries.Clear();
            await Task.Run(() => ParseManufData(manufData));
            IsInitialized = true;
        }

        public async Task Init(string path)
        {
            IsInitialized = false;
            _entries.Clear();
            await Task.Run(() => ParseManufData(null));
            IsInitialized = true;
        }

        static IEnumerable<string> LineGenerator(StreamReader sr)
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                yield return line;
            }
        }

        void ParseManufData(Stream manufData)
        {
            var streamReader = new StreamReader(manufData, Encoding.UTF8);
            var bag = new ConcurrentBag<MacVendorInfo>();

            Parallel.ForEach(LineGenerator(streamReader), line =>
            {
                if (line.TrimStart(new char[0]).StartsWith("#", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
                if (string.IsNullOrWhiteSpace(line))
                {
                    return;
                }

                var parts = line.Split(new char[0], 2, StringSplitOptions.RemoveEmptyEntries);
                var macStr = parts[0];
                var descParts = parts[1].Split(new[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                var shortName = descParts[0].Trim();

                string longName = null;
                if (descParts.Length > 1)
                {
                    longName = descParts[1].Trim();
                }

                byte mask = 0;
                if (macStr.Contains("/"))
                {
                    var macParts = macStr.Split(new[] { '/' }, 2, StringSplitOptions.None);
                    mask = byte.Parse(macParts[1], CultureInfo.InvariantCulture);
                    macStr = macParts[0];
                }

                var macHexParts = macStr.Split(new[] { ':', '-', '.' }, StringSplitOptions.None);
                var macBytes = new byte[8];
                if (mask == 0)
                {
                    mask = (byte)(macHexParts.Length * 8);
                }

                for (var i = 0; i < macHexParts.Length; i++)
                {
                    macBytes[i] = Convert.ToByte(macHexParts[i], 16);
                }

                var identLong = BitConverter.ToInt64(macBytes, 0);
                identLong = IPAddress.HostToNetworkOrder(identLong);

                var entry = new MacVendorInfo(identLong, mask, longName ?? shortName);
                bag.Add(entry);

            });

            _entries = bag.ToList();
        }

        public IEnumerable<MacVendorInfo> GetEntries()
        {
            if (!IsInitialized)
            {
                throw new Exception("Must be first be initialized");
            }
            return _entries;
        }
    }
}
