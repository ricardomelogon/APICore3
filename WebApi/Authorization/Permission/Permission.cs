using System.ComponentModel.DataAnnotations;

namespace WebApi.Authorization
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1028:Enum Storage should be Int32", Justification = "ushort is defined instead of int since permissions are transformed into 16 bit characters and therefore an int32 would break the character limit")]
    public enum Permission : ushort
    {
        //SYSTEM SECTION

        /// <summary>
        /// <para><b>Group:</b> NotSet</para>
        /// <para><b>Description:</b> //SYSTEM SECTION</para>
        /// </summary>
        Locked = 0, //error condition

        /// <summary>
        /// <para><b>Group:</b> System</para>
        /// <para><b>Description:</b> Basic User Role</para>
        /// </summary>
        User = 1,

        /// <summary>
        /// <para><b>Group:</b> System</para>
        /// <para><b>Description:</b> This allows the user to access every feature</para>
        /// </summary>
        AccessAll = ushort.MaxValue,
    }
}