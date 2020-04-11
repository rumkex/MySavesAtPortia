using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Newtonsoft.Json;

namespace MySavesAtPortia.Serialization
{
  public struct SafeReader : IDisposable
  {
    private readonly int beginPos;
    private readonly int dataLen;
    private readonly BinaryReader reader;
    private readonly bool ignoreError;

    public SafeReader(Stream input, bool compatibleBinaryReader = false)
      : this(new BinaryReader(input), false, compatibleBinaryReader)
    {
    }

    private SafeReader(BinaryReader reader, bool ignoreError, bool compatibleBinaryReader)
    {
      dataLen = 0;
      beginPos = 0;
      this.reader = reader;
      this.ignoreError = ignoreError;
      this.CompatibleBinaryReader = compatibleBinaryReader;
      if (compatibleBinaryReader)
        return;
      dataLen = ReadInt32();
      beginPos = Position;
    }

    public bool CompatibleBinaryReader { get; }

    public int Length => (int) reader.BaseStream.Length;

    public int Position
    {
      get => reader.BaseStream.Position < (long) int.MaxValue ? (int) reader.BaseStream.Position :
        throw new Exception("too large stream");
      set => reader.BaseStream.Position = value;
    }

    void IDisposable.Dispose()
    {
      if (CompatibleBinaryReader)
        return;
      var offset = Position - beginPos;
      if (dataLen == offset)
        return;
      if (!ignoreError)
        throw new Exception($"error: should have read {dataLen} bytes, but read {offset} instead");
      Position = beginPos + dataLen;
    }

    public Dictionary<TKey, TValue> ReadDic<TKey, TValue>()
      where TKey : struct
      where TValue : struct
    {
      var dictionary = new Dictionary<TKey, TValue>();
      var num = reader.ReadInt32();
      for (var index = 0; index < num; ++index)
      {
        var str1 = reader.ReadString();
        var str2 = reader.ReadString();
        var key = ConvertTo<TKey>(str1);
        var obj = ConvertTo<TValue>(str2);
        dictionary.Add(key, obj);
      }
      return dictionary;
    }

    private static T ConvertTo<T>(object value)
    {
      return (T) Convert.ChangeType(value, typeof(T));
    }

    public List<T> ReadList<T>()
    {
      var objList = new List<T>();
      var num = reader.ReadInt32();
      for (var index = 0; index < num; ++index)
      {
        var obj = JsonConvert.DeserializeObject<T>(reader.ReadString());
        objList.Add(obj);
      }
      return objList;
    }

    public bool ReadBoolean()
    {
      return reader.ReadBoolean();
    }

    public bool ReadBool()
    {
      return reader.ReadBoolean();
    }

    public short ReadInt16()
    {
      return reader.ReadInt16();
    }

    public int ReadInt32()
    {
      return reader.ReadInt32();
    }

    public long ReadInt64()
    {
      return reader.ReadInt64();
    }

    public double ReadDouble()
    {
      return reader.ReadDouble();
    }

    public float ReadSingle()
    {
      return reader.ReadSingle();
    }

    public float ReadFloat()
    {
      return reader.ReadSingle();
    }

    public ulong ReadUint64()
    {
      return reader.ReadUInt64();
    }

    public uint ReadUint32()
    {
      return reader.ReadUInt32();
    }

    public ushort ReadUint16()
    {
      return reader.ReadUInt16();
    }

    public string ReadNullableString()
    {
      return ReadBool() ? null : reader.ReadString();
    }

    public string ReadString()
    {
      return reader.ReadString();
    }

    public byte[] ReadBytes()
    {
      var count = ReadInt32();
      return count <= 0 ? null : reader.ReadBytes(count);
    }

    public byte[] ReadBytes(int count)
    {
      return reader.ReadBytes(count);
    }

    public int ReadBytes(byte[] buffer, int index, int count)
    {
      return reader.Read(buffer, index, count);
    }

    public SafeReader ReadBytesStream(bool shouldIgnoreError = false)
    {
      return new SafeReader(this.reader, shouldIgnoreError, false);
    }

    public byte ReadByte()
    {
      return reader.ReadByte();
    }

    public Color ReadColor()
    {
      Color color;
      color.r = reader.ReadSingle();
      color.g = reader.ReadSingle();
      color.b = reader.ReadSingle();
      color.a = reader.ReadSingle();
      return color;
    }

    public Color ReadColor3()
    {
      Color color;
      color.r = reader.ReadSingle();
      color.g = reader.ReadSingle();
      color.b = reader.ReadSingle();
      color.a = 1f;
      return color;
    }

    public Vector2 ReadVector2()
    {
      return new Vector2(reader.ReadSingle(), reader.ReadSingle());
    }

    public Vector3 ReadVector3()
    {
      return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    public Quaternion ReadQuaternion()
    {
      return new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    public List<int> ReadIntList()
    {
      var num = reader.ReadInt32();
      var intList = new List<int>(num);
      for (var index = 0; index < num; ++index)
        intList.Add(reader.ReadInt32());
      return intList;
    }
  }
}