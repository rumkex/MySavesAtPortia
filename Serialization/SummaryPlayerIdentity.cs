using System;

namespace MySavesAtPortia.Serialization
{
    public struct SummaryPlayerIdentity
    {
        public Guid Guid { get; }
        public DateTime CreationTime { get; }

        public SummaryPlayerIdentity(Guid playerGuid, DateTime createPlayerTime)
        {
            this.Guid = playerGuid;
            this.CreationTime = createPlayerTime;
        }

        public static bool operator !=(SummaryPlayerIdentity v1, SummaryPlayerIdentity v2)
        {
            return v1.Guid != v2.Guid || v1.CreationTime != v2.CreationTime;
        }

        public static bool operator ==(SummaryPlayerIdentity v1, SummaryPlayerIdentity v2)
        {
            return v1.Guid == v2.Guid && v1.CreationTime == v2.CreationTime;
        }

        public override bool Equals(object obj)
        {
            if (obj is SummaryPlayerIdentity identity)
            {
                return Guid == identity.Guid &&
                       CreationTime == identity.CreationTime;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode() + CreationTime.GetHashCode();
        }
    }
}