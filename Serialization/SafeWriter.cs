using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Newtonsoft.Json;

namespace MySavesAtPortia.Serialization
{
  public struct SafeWriter : IDisposable
  {
    private readonly BinaryWriter w;
    private readonly int beginPos;

    public SafeWriter(Stream input)
      : this(new BinaryWriter(input))
    {
    }

    private SafeWriter(BinaryWriter w)
    {
      beginPos = 0;
      this.w = w;
      WriteInt32(int.MinValue);
      beginPos = Position;
    }

    private int Position
    {
      get => w.BaseStream.Position < int.MaxValue ? (int) w.BaseStream.Position:
          throw new Exception("too large stream");
      set => w.BaseStream.Position = value;
    }

    void IDisposable.Dispose()
    {
      w.Flush();
      int position = Position;
      int num = position - beginPos;
      Position = beginPos - 4;
      WriteInt32(num);
      Position = position;
    }

    public SafeWriter CreateSubWriter()
    {
      return new SafeWriter(w);
    }

    public Stream GetStream()
    {
      return w.BaseStream;
    }

    public void WriteDic<Tkey, TValue>(Dictionary<Tkey, TValue> dic) where Tkey : struct
    {
      if (dic == null || dic.Count == 0)
      {
        w.Write(0);
      }
      else
      {
        w.Write(dic.Count);
        foreach (KeyValuePair<Tkey, TValue> keyValuePair in dic)
        {
          w.Write(keyValuePair.Key.ToString());
          w.Write(keyValuePair.Value.ToString());
        }
      }
    }

    public void WriteList<T>(List<T> list)
    {
      if (list == null || list.Count == 0)
      {
        w.Write(0);
      }
      else
      {
        w.Write(list.Count);
        foreach (T obj in list)
          w.Write(JsonConvert.SerializeObject(obj));
      }
    }

    public void WriteBool(bool value)
    {
      w.Write(value);
    }

    public void WriteInt16(short value)
    {
      w.Write(value);
    }

    public void WriteInt32(int value)
    {
      w.Write(value);
    }

    public void WriteInt64(long value)
    {
      w.Write(value);
    }

    public void WriteDouble(double value)
    {
      w.Write(value);
    }

    public void WriteFloat(float value)
    {
      w.Write(value);
    }

    public void WriteUint64(ulong value)
    {
      w.Write(value);
    }

    public void WriteUint32(uint value)
    {
      w.Write(value);
    }

    public void WriteUint16(ushort value)
    {
      w.Write(value);
    }

    public void WriteString(string value)
    {
      w.Write(value);
    }

    public void WriteNullableString(string value)
    {
      if (string.IsNullOrEmpty(value))
      {
        WriteBool(true);
      }
      else
      {
        WriteBool(false);
        w.Write(value);
      }
    }

    public void WriteBytes(byte[] buffer)
    {
      if (buffer == null)
      {
        WriteInt32(0);
      }
      else
      {
        WriteInt32(buffer.Length);
        w.Write(buffer);
      }
    }

    public void WriteBytes(byte[] buffer, int index, int count)
    {
      w.Write(buffer, index, count);
    }

    public void WriteByte(byte value)
    {
      w.Write(value);
    }

    public void WriteColor3(Color c)
    {
      w.Write(c.r);
      w.Write(c.g);
      w.Write(c.b);
    }

    public void WriteColor(Color c)
    {
      w.Write(c.r);
      w.Write(c.g);
      w.Write(c.b);
      w.Write(c.a);
    }

    public void WriteVector2(Vector2 v)
    {
      w.Write(v.X);
      w.Write(v.Y);
    }

    public void WriteVector3(Vector3 v)
    {
      w.Write(v.X);
      w.Write(v.Y);
      w.Write(v.Z);
    }

    public void WriteQuaternion(Quaternion v)
    {
      w.Write(v.X);
      w.Write(v.Y);
      w.Write(v.Z);
      w.Write(v.W);
    }

    public void WriteIntList(IList<int> v)
    {
      w.Write(v.Count);
      foreach (int num in v)
        w.Write(num);
    }
  }
}