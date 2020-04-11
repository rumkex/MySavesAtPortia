using System;
using System.Collections.Generic;
using System.IO;

namespace MySavesAtPortia.Serialization
{
	public interface IArchiveItem<out T> where T: new()
	{
		void Deserialize(SafeReader reader);
		void Serialize(SafeWriter writer);
	}

	public class Archive
	{
		private readonly Header header = new Header();

		private readonly OrderedDictionary<string, byte[]> items = new OrderedDictionary<string, byte[]>();
		public IDictionary<string, byte[]> Items => items;

		public int Version => header.Version;

		public T Fetch<T>(string key) where T : IArchiveItem<T>, new()
		{
			var result = new T();
			using (var stream = new MemoryStream(Items[key], writable: false))
			using (var reader = new SafeReader(stream))
			{
				result.Deserialize(reader);
			}
			return result;
		}

		public void Store<T>(string key, T item) where T : IArchiveItem<T>, new()
		{
			using (var stream = new MemoryStream())
			using (var writer = new SafeWriter(stream))
			{
				item.Serialize(writer);
				Items[key] = stream.GetBuffer();
			}
		}

		public void Save(string filePath)
		{
			string directoryName = Path.GetDirectoryName(filePath);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			Write(filePath);
		}

		private void Write(string filePath)
		{
			using (var output = new FileStream(filePath, FileMode.Create, FileAccess.Write))
			{
				using (var binaryWriter = new BinaryWriter(output))
				{
					header.Write(binaryWriter);
					WriteArchive(binaryWriter);
				}
			}
		}

		private void WriteArchive(BinaryWriter writer)
		{
			writer.Write(items.Count);
			foreach (var (key, data) in items)
			{
				writer.Write(key);
				writer.Write(data.Length);
				writer.Write(data);
				var crc = CRC64.Compute(data);
				writer.Write(crc);
			}
		}

		public void Load(string filePath)
		{
			using (FileStream input = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader reader = new BinaryReader(input))
				{
					header.Load(reader);
					if (!header.IsCompatibleVersion())
					{
						throw new Exception("Incompatible archive version");
					}

					var blockCount = reader.ReadInt32();
					for (var block = 0; block < blockCount; block++)
					{
						var text = reader.ReadString();
						var size = reader.ReadInt32();
						var data = reader.ReadBytes(size);
						var expected = reader.ReadUInt64();
						var crc = CRC64.Compute(data);
						if (crc != expected)
						{
							throw new Exception($"Item checksum error: '{text}'");
						}
						items.Add(text, data);
					}
				}
			}
		}
	}

	public class Header
	{
		private const int LatestVersion = 71;
		private const int CompatibleVersion = 5;
		private int version;
		private const int SafeWriterReaderMagic = 1;
		private int magic;

		public Header()
		{
			version = LatestVersion;
		}

		public int Version
		{
			get { return version; }
		}

		private bool IsDemo()
		{
			return Version == 42;
		}

		public bool IsCompatibleVersion()
		{
			return Version >= CompatibleVersion && Version <= LatestVersion && !IsDemo();
		}

		public bool IsSafeWriter()
		{
			return magic == SafeWriterReaderMagic;
		}

		public DateTime Load(BinaryReader r)
		{
			version = r.ReadInt32();
			long ticks = r.ReadInt64();
			magic = r.ReadInt32();
			return new DateTime(ticks);
		}

		public void Write(BinaryWriter w)
		{
			w.Write(LatestVersion);
			w.Write(DateTime.Now.Ticks);
			w.Write(SafeWriterReaderMagic);
		}
	}
}
