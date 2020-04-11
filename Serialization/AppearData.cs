using System.Collections.Generic;

namespace MySavesAtPortia.Serialization
{
  public class AppearData
  {
    public IList<float> PartFactorArray { get; } = new float[19];
    public IList<ColorCombine> PartColors { get; } = new ColorCombine[7];

    public void Serialize(SafeWriter w)
    {
      w.WriteInt32(PartFactorArray.Count);
      foreach (var t in PartFactorArray)
        w.WriteFloat(t);

      w.WriteInt32(PartColors.Count);
      foreach (var t in PartColors)
        t.Serialize(w);
    }

    public void Deserialize(SafeReader r)
    {
      var factorCount = r.ReadInt32();
      for (var index = 0; index < factorCount; ++index)
      {
        PartFactorArray[index] = r.ReadSingle();
      }
      var colorCount = r.ReadInt32();
      for (var index = 0; index < colorCount; ++index)
      {
        PartColors[index] = new ColorCombine();
        PartColors[index].Deserialize(r);
      }
    }

    public class ColorCombine
    {
      public Color[] Colors { get; } = new Color[2];

      public void Serialize(SafeWriter w)
      {
        foreach (var color in Colors)
        {
          w.WriteColor3(color);
        }
      }

      public void Deserialize(SafeReader r)
      {
        for (var index = 0; index < Colors.Length; ++index)
          Colors[index] = r.ReadColor3();
      }
    }
  }
}