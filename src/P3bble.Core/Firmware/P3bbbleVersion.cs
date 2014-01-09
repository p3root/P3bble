using System;
using P3bble.Core.Helper;
using System.Runtime.Serialization;

namespace P3bble.Core.Firmware
{
    [DataContract]
    public abstract class P3bbbleVersion : IComparable<P3bbbleVersion>
    {
        [DataMember(Name = "timestamp", IsRequired = true)]
        internal int TimestampInternal { get; set; }

        public DateTime Timestamp { get { return TimestampInternal.AsDateTime(); } }

        [DataMember(Name = "friendlyVersion", IsRequired = true)]
        internal string VersionInternal { get; set; }

        public Version Version
        {
            get
            {
                return this.VersionInternal.AsVersion();
            }
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        public int CompareTo(P3bbbleVersion other)
        {
            return this.Version.CompareTo(other.Version);
        }

        public static bool operator >(P3bbbleVersion v1, P3bbbleVersion v2)
        {
            return v1.Version > v2.Version;
        }

        public static bool operator <(P3bbbleVersion v1, P3bbbleVersion v2)
        {
            return v1.Version < v2.Version;
        }

        public static bool operator ==(P3bbbleVersion v1, P3bbbleVersion v2)
        {
            return v1.Version == v2.Version;
        }

        public static bool operator !=(P3bbbleVersion v1, P3bbbleVersion v2)
        {
            return v1.Version != v2.Version;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" }, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var ver = obj as P3bbbleVersion;
            if (obj != null)
            {
                return this.Version == ver.Version;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.Version.GetHashCode();
        }
    }
}
