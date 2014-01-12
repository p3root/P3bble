using System;
using System.Runtime.Serialization;
using P3bble.Core.Helper;

namespace P3bble.Core.Types
{
    [DataContract]
    public abstract class P3bbleVersion : IComparable<P3bbleVersion>
    {
        public DateTime Timestamp
        {
            get
            {
                return this.TimestampInternal.AsDateTime();
            }
        }

        public Version Version
        {
            get
            {
                return this.VersionInternal.AsVersion();
            }
        }

        [DataMember(Name = "timestamp", IsRequired = true)]
        internal int TimestampInternal { get; set; }

        [DataMember(Name = "friendlyVersion", IsRequired = true)]
        internal string VersionInternal { get; set; }

        public static bool operator >(P3bbleVersion v1, P3bbleVersion v2)
        {
            return v1 != null && v2 != null && v1.Version > v2.Version;
        }

        public static bool operator <(P3bbleVersion v1, P3bbleVersion v2)
        {
            return v1 != null && v2 != null && v1.Version < v2.Version;
        }

        public static bool operator ==(P3bbleVersion v1, P3bbleVersion v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(P3bbleVersion v1, P3bbleVersion v2)
        {
            return !v1.Equals(v2);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        public int CompareTo(P3bbleVersion other)
        {
            return this.Version.CompareTo(other.Version);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var ver = obj as P3bbleVersion;
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
