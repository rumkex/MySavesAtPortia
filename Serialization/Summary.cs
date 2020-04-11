using System;
using System.Collections.Generic;
using System.IO;

namespace MySavesAtPortia.Serialization
{
    public enum Gender
    {
        Male,
        Female,
        Max,
    }

    public class Summary: IArchiveItem<Summary>
    {
        private const int CurrentVersion = 201901040;

        public IList<int> SubModelIds { get; } = new[] { -1, -1, -1, -1, -1, -1 };

        public string PlayerName { get; set; }
        public Gender PlayerGender { get; set; }
        public int PlayerLevel { get; set; }

        public long GameTime { get; set; }
        public DateTime SystemTime { get; set; }
        public TimeSpan PlayedTime { get; set; }
        public SummaryPlayerIdentity PlayerIdentity { get; set; }
        public int PlayerMoney { get; set; }

        public bool WorkshopNameSet { get; set; }
        public string WorkshopName { get; set; }
        public string WorkshopLevel { get; set; }

        public FigurePreviewData figurePreviewData;
        public string[] appearUnitsEquipPaths;
        public string[] appearUnitsNudePaths;
        public string Tattoo { get; set; }
        public AppearData appearData;
        public bool IsMainStoryDone { get; set; }
        public IList<int> DLCRequire { get; set; }

        public void Deserialize(SafeReader reader)
        {
            var recordVersion = reader.ReadInt32();

            if (recordVersion > CurrentVersion)
                throw new Exception($"Version {recordVersion} is not supported, last is {CurrentVersion}");

            var summaryData = reader.ReadBytes();
            using (var memoryStream = new MemoryStream(summaryData, false))
            {
                using (var r = new SafeReader(memoryStream, reader.CompatibleBinaryReader))
                {
                    Import(r, recordVersion);
                }
            }
        }

        public void Serialize(SafeWriter writer)
        {
            var data = Export();
            writer.WriteInt32(CurrentVersion);
            writer.WriteBytes(data);
        }

        private void Import(SafeReader r, int recordVersion)
        {
            if (recordVersion >= 201812210)
            {
                DLCRequire = r.ReadIntList();
            }

            PlayerIdentity = new SummaryPlayerIdentity(
                new Guid(r.ReadBytes()),
                new DateTime(r.ReadInt64()));

            PlayerName = r.ReadNullableString();
            PlayerGender = recordVersion >= 201711280 ? (Gender) r.ReadInt32() : Gender.Male;
            PlayerLevel = r.ReadInt32();
            PlayerMoney = r.ReadInt32();
            if (recordVersion < 201711281)
            {
                WorkshopNameSet = false;
                WorkshopName = r.ReadString();
            }
            else
            {
                WorkshopNameSet = r.ReadBoolean();
                WorkshopName = r.ReadNullableString();
            }

            if (recordVersion >= 201801150)
            {
                int length1 = r.ReadInt32();
                appearUnitsEquipPaths = new string[length1];
                for (int index = 0; index < length1; ++index)
                    appearUnitsEquipPaths[index] =
                        recordVersion < 201810100 ? r.ReadString() : r.ReadNullableString();
                if (recordVersion >= 201810100)
                {
                    int length2 = r.ReadInt32();
                    appearUnitsNudePaths = new string[length2];
                    for (int index = 0; index < length2; ++index)
                        appearUnitsNudePaths[index] = r.ReadNullableString();
                    if (recordVersion >= 201812250)
                    {
                        for (int index = 0; index < 6; ++index)
                            SubModelIds[index] = r.ReadInt32();
                    }
                    else
                    {
                        for (int index = 0; index < 3; ++index)
                        {
                            SubModelIds[index] = -1;
                            SubModelIds[index + 3] = r.ReadInt32();
                        }
                    }
                }
                else
                {
                    for (int index = 0; index < 3; ++index)
                        SubModelIds[index] = -1;
                }

                appearData = new AppearData();
                appearData.Deserialize(r);
                if (recordVersion >= 201812100)
                    Tattoo = r.ReadNullableString();
            }
            else
            {
                appearUnitsEquipPaths = null;
                appearData = null;
            }

            WorkshopLevel = r.ReadString();
            GameTime = r.ReadInt64();
            SystemTime = new DateTime(r.ReadInt64());
            PlayedTime = new TimeSpan(r.ReadInt64());
            if (recordVersion >= 201901040)
                IsMainStoryDone = r.ReadBool();
            byte[] data = r.ReadBytes();
            figurePreviewData =
                data == null || data.Length == 0 ? (FigurePreviewData) null : new FigurePreviewData(data);
            r.ReadBytes();
        }

        private byte[] Export()
        {
            using (MemoryStream memoryStream = new MemoryStream(500))
            {
                using (SafeWriter w = new SafeWriter(memoryStream))
                {
                    w.WriteIntList(DLCRequire);
                    var byteArray = PlayerIdentity.Guid.ToByteArray();
                    w.WriteBytes(byteArray);
                    w.WriteInt64(PlayerIdentity.CreationTime.Ticks);
                    w.WriteNullableString(PlayerName);
                    w.WriteInt32((int) PlayerGender);
                    w.WriteInt32(PlayerLevel);
                    w.WriteInt32(PlayerMoney);
                    w.WriteBool(WorkshopNameSet);
                    w.WriteNullableString(WorkshopName);
                    w.WriteInt32(appearUnitsEquipPaths.Length);
                    foreach (var path in appearUnitsEquipPaths)
                        w.WriteNullableString(path);
                    w.WriteInt32(appearUnitsNudePaths.Length);
                    foreach (var path in appearUnitsNudePaths)
                        w.WriteNullableString(path);
                    foreach (var id in SubModelIds)
                        w.WriteInt32(id);

                    appearData.Serialize(w);
                    w.WriteNullableString(Tattoo);
                    w.WriteString(WorkshopLevel);
                    w.WriteInt64(GameTime);
                    w.WriteInt64(SystemTime.Ticks);
                    w.WriteInt64(PlayedTime.Ticks);
                    w.WriteBool(IsMainStoryDone);
                    w.WriteBytes(figurePreviewData != null ? figurePreviewData.ToBytes() : new byte[0]);
                    w.WriteBytes(new byte[0]);
                }

                return memoryStream.ToArray();
            }
        }
    }
}