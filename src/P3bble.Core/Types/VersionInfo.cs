using System;
using System.Runtime.Serialization;
using P3bble.Helper;

namespace P3bble.Types
{
    /// <summary>
    /// Version information
    /// </summary>
    [DataContract]
    public abstract class VersionInfo : IComparable<VersionInfo>
    {
        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public DateTime Timestamp
        {
            get
            {
                return this.TimestampInternal.AsDateTime();
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public Version Version
        {
            get
            {
                return this.VersionInternal.AsVersion();
            }
        }

        [DataMember(Name = "timestamp", IsRequired = true)]
        internal int TimestampInternal { get; set; }

        [DataMember(Name = "friendlyVersion", IsRequired = false)]
        internal string VersionInternal { get; set; }

        /// <summary>
        /// Determines whether one specified P3bbleVersion is greater than another specified P3bbleVersion.
        /// </summary>
        /// <param name="v1">The first object to compare.</param>
        /// <param name="v2">The second object to compare.</param>
        /// <returns>true if v1 is greater than v2; otherwise, false.</returns>
        public static bool operator >(VersionInfo v1, VersionInfo v2)
        {
            return v1 != null && v2 != null && v1.Version > v2.Version;
        }

        /// <summary>
        /// Determines whether one specified P3bbleVersion is less than another specified P3bbleVersion.
        /// </summary>
        /// <param name="v1">The first object to compare.</param>
        /// <param name="v2">The second object to compare.</param>
        /// <returns>true if v1 is less than v2; otherwise, false.</returns>
        public static bool operator <(VersionInfo v1, VersionInfo v2)
        {
            return v1 != null && v2 != null && v1.Version < v2.Version;
        }

        /// <summary>
        /// Determines whether one specified P3bbleVersion is equal to another specified P3bbleVersion.
        /// </summary>
        /// <param name="v1">The first object to compare.</param>
        /// <param name="v2">The second object to compare.</param>
        /// <returns>true if v1 is equal to v2; otherwise, false.</returns>
        public static bool operator ==(VersionInfo v1, VersionInfo v2)
        {
            return v1.Equals(v2);
        }

        /// <summary>
        /// Determines whether one specified P3bbleVersion is not equal to another specified P3bbleVersion.
        /// </summary>
        /// <param name="v1">The first object to compare.</param>
        /// <param name="v2">The second object to compare.</param>
        /// <returns>true if v1 is not equal to v2; otherwise, false.</returns>
        public static bool operator !=(VersionInfo v1, VersionInfo v2)
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
        public int CompareTo(VersionInfo other)
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
            var ver = obj as VersionInfo;
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
