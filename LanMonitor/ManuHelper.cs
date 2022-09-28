using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO.Compression;
using System.Net.NetworkInformation;

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
        public static ManuHelper Instance { get; private set; } = new ManuHelper();
        public class MacVendorInfo
        {
            public long Identifier { get; private set; }
            public byte MaskLength { get; private set; }
            public string Organization { get; private set; }

            public MacVendorInfo(long identifier, byte maskLength, string organization)
            {
                Identifier = identifier;
                MaskLength = maskLength;
                Organization = organization;
            }
        }
        public bool IsInitialized { get; private set; }

        Dictionary<byte, Dictionary<long, MacVendorInfo>> _dicts = new Dictionary<byte, Dictionary<long, MacVendorInfo>>();

        void BuildEntryDictionaries(List<MacVendorInfo> entries)
        {
            foreach (var entry in entries)
            {
                if (!_dicts.TryGetValue(entry.MaskLength, out Dictionary<long, MacVendorInfo> entryDict))
                {
                    entryDict = new Dictionary<long, MacVendorInfo>();
                    _dicts.Add(entry.MaskLength, entryDict);
                }

                entryDict[entry.Identifier] = entry;
            }
        }
        const long MAX_LONG = unchecked((long)ulong.MaxValue);
        public MacVendorInfo FindInfo(string macAddress)
        {
            if (macAddress == null)
            {
                return null;
            }
            try
            {
                return FindInfo(PhysicalAddress.Parse(macAddress.Trim().Replace(':', '-')));
            }
            catch
            {
                return null;
            }
        }
        public MacVendorInfo FindInfo(PhysicalAddress macAddress)
        {
            if (!IsInitialized)
            {
                return null;
            }
            var longBytes = new byte[8];
            var macAddrBytes = macAddress.GetAddressBytes();
            macAddrBytes.CopyTo(longBytes, 0);
            var identifier = IPAddress.HostToNetworkOrder(BitConverter.ToInt64(longBytes, 0));

            foreach (var dict in _dicts)
            {
                int mask = dict.Key;

                var maskedIdent = identifier & (MAX_LONG << (64 - mask));

                MacVendorInfo entry;
                if (dict.Value.TryGetValue(maskedIdent, out entry))
                {
                    return entry;
                }
            }

            return null;
        }
        public async Task Init(Stream manufData)
        {
            IsInitialized = false;
            var entries = await Task.Run(() => ParseManufData(manufData));
            await Task.Run(() => BuildEntryDictionaries(entries));
            IsInitialized = true;
        }

        public async Task Init(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (var decompressionStream = new GZipStream(fs, CompressionMode.Decompress))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        decompressionStream.CopyTo(ms);
                        ms.Position = 0;
                        await Init(ms);
                    }
                }
            }
        }

        private static IEnumerable<string> LineGenerator(StreamReader sr)
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                yield return line;
            }
        }

        private List<MacVendorInfo> ParseManufData(Stream manufData)
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
                var descParts = parts[1].Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
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

            return bag.ToList();
        }
    }
}
