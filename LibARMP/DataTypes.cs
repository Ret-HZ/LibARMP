using LibARMP.Exceptions;
using System;
using System.Collections.Generic;

namespace LibARMP
{
    internal class DataTypes
    {

        internal static List<ArmpType> Types = new List<ArmpType>(32);


        static DataTypes()
        {
            Init();
        }


        /// <summary>
        /// Initializes all armp type information.
        /// </summary>
        private static void Init()
        {
            Types.Add(new ArmpType() { CSType = null, IDv1 = -1, MemberTypeIDv1 = -1, IDv2 = -1, MemberTypeIDv2 = 0, IDOE = -1, Size = 0, DefaultValue = null }); //Invalid
            Types.Add(new ArmpType() { CSType = typeof(byte), IDv1 = 2, MemberTypeIDv1 = 0, IDv2 = 2, MemberTypeIDv2 = 4, IDOE = 5, Size = 1, DefaultValue = (byte) 0 }); //uint8
            Types.Add(new ArmpType() { CSType = typeof(UInt16), IDv1 = 1, MemberTypeIDv1 = 1, IDv2 = 1, MemberTypeIDv2 = 3, IDOE = 4, Size = 2, DefaultValue = (UInt16) 0 }); //uint16
            Types.Add(new ArmpType() { CSType = typeof(UInt32), IDv1 = 0, MemberTypeIDv1 = 2, IDv2 = 0, MemberTypeIDv2 = 2, IDOE = 3, Size = 4, DefaultValue = 0u }); //uint32
            Types.Add(new ArmpType() { CSType = typeof(UInt64), IDv1 = 8, MemberTypeIDv1 = 3, IDv2 = 8, MemberTypeIDv2 = 1, IDOE = -1, Size = 8, DefaultValue = (UInt64) 0 }); //uint64
            Types.Add(new ArmpType() { CSType = typeof(sbyte), IDv1 = 5, MemberTypeIDv1 = 4, IDv2 = 5, MemberTypeIDv2 = 8, IDOE = 2, Size = 1, DefaultValue = (sbyte) 0 }); //int8
            Types.Add(new ArmpType() { CSType = typeof(Int16), IDv1 = 4, MemberTypeIDv1 = 5, IDv2 = 4, MemberTypeIDv2 = 7, IDOE = 1, Size = 2, DefaultValue = (Int16) 0 }); //int16
            Types.Add(new ArmpType() { CSType = typeof(Int32), IDv1 = 3, MemberTypeIDv1 = 6, IDv2 = 3, MemberTypeIDv2 = 6, IDOE = 0, Size = 4, DefaultValue = 0 }); //int32
            Types.Add(new ArmpType() { CSType = typeof(Int64), IDv1 = 10, MemberTypeIDv1 = 7, IDv2 = 10, MemberTypeIDv2 = 5, IDOE = -1, Size = 8, DefaultValue = (Int64) 0 }); //int64
            // Missing: float16 and float16 array types
            Types.Add(new ArmpType() { CSType = typeof(float), IDv1 = 7, MemberTypeIDv1 = 9, IDv2 = 7, MemberTypeIDv2 = 10, IDOE = -1, Size = 4, DefaultValue = 0f }); //float32
            Types.Add(new ArmpType() { CSType = typeof(double), IDv1 = -1, MemberTypeIDv1 = -1, IDv2 = 11, MemberTypeIDv2 = 9, IDOE = -1, Size = 8, DefaultValue = 0.0 }); //float64
            Types.Add(new ArmpType() { CSType = typeof(bool), IDv1 = 6, MemberTypeIDv1 = 11, IDv2 = 6, MemberTypeIDv2 = 4, IDOE = 6, Size = 1, DefaultValue = false }); //boolean
            Types.Add(new ArmpType() { CSType = typeof(string), IDv1 = 0, MemberTypeIDv1 = 12, IDv2 = 13, MemberTypeIDv2 = 12, IDOE = 1, Size = 8, DefaultValue = string.Empty }); //string
            Types.Add(new ArmpType() { CSType = typeof(ArmpTable), IDv1 = 9, MemberTypeIDv1 = 13, IDv2 = 9, MemberTypeIDv2 = 13, IDOE = -1, Size = 8, DefaultValue = null }); //table

            Types.Add(new ArmpType() { CSType = typeof(List<byte>), IDv1 = -1, MemberTypeIDv1 = -1, IDv2 = 14, MemberTypeIDv2 = 14, IDOE = -1, Size = 1, Align = 8, DefaultValue = null, IsArray = true }); //uint8 array
            Types.Add(new ArmpType() { CSType = typeof(List<UInt16>), IDv1 = -1, MemberTypeIDv1 = -1, IDv2 = 15, MemberTypeIDv2 = 15, IDOE = -1, Size = 2, Align = 8, DefaultValue = null, IsArray = true }); //uint16 array
            Types.Add(new ArmpType() { CSType = typeof(List<UInt32>), IDv1 = -1, MemberTypeIDv1 = -1, IDv2 = 16, MemberTypeIDv2 = 16, IDOE = -1, Size = 4, Align = 8, DefaultValue = null, IsArray = true }); //uint32 array
            Types.Add(new ArmpType() { CSType = typeof(List<UInt64>), IDv1 = -1, MemberTypeIDv1 = -1, IDv2 = 17, MemberTypeIDv2 = 17, IDOE = -1, Size = 8, Align = 8, DefaultValue = null, IsArray = true }); //uint64 array
            Types.Add(new ArmpType() { CSType = typeof(List<sbyte>), IDv1 = -1, MemberTypeIDv1 = -1, IDv2 = 18, MemberTypeIDv2 = 18, IDOE = -1, Size = 1, Align = 8, DefaultValue = null, IsArray = true }); //int8 array
            Types.Add(new ArmpType() { CSType = typeof(List<Int16>), IDv1 = -1, MemberTypeIDv1 = -1, IDv2 = 19, MemberTypeIDv2 = 19, IDOE = -1, Size = 2, Align = 8, DefaultValue = null, IsArray = true }); //int16 array
            Types.Add(new ArmpType() { CSType = typeof(List<Int32>), IDv1 = -1, MemberTypeIDv1 = -1, IDv2 = 20, MemberTypeIDv2 = 20, IDOE = -1, Size = 4, Align = 8, DefaultValue = null, IsArray = true }); //int32 array
            Types.Add(new ArmpType() { CSType = typeof(List<Int64>), IDv1 = -1, MemberTypeIDv1 = -1, IDv2 = 21, MemberTypeIDv2 = 21, IDOE = -1, Size = 8, Align = 8, DefaultValue = null, IsArray = true }); //int64 array
            Types.Add(new ArmpType() { CSType = typeof(List<string>), IDv1 = -1, MemberTypeIDv1 = -1, IDv2 = 22, MemberTypeIDv2 = 25, IDOE = -1, Size = 8, Align = 8, DefaultValue = null, IsArray = true }); //string array
            Types.Add(new ArmpType() { CSType = typeof(List<ArmpTable>), IDv1 = -1, MemberTypeIDv1 = -1, IDv2 = 23, MemberTypeIDv2 = 26, IDOE = -1, Size = 8, Align = 8, DefaultValue = null, IsArray = true }); //table array
            Types.Add(new ArmpType() { CSType = typeof(List<float>), IDv1 = -1, MemberTypeIDv1 = -1, IDv2 = 25, MemberTypeIDv2 = 23, IDOE = -1, Size = 4, Align = 8, DefaultValue = null, IsArray = true }); //float32 array
            Types.Add(new ArmpType() { CSType = typeof(List<double>), IDv1 = -1, MemberTypeIDv1 = -1, IDv2 = 26, MemberTypeIDv2 = 24, IDOE = -1, Size = 8, Align = 8, DefaultValue = null, IsArray = true }); //float64 array
            Types.Add(new ArmpType() { CSType = typeof(float[]), IDv1 = -1, MemberTypeIDv1 = -1, IDv2 = 27, MemberTypeIDv2 = 27, IDOE = -1, Size = 4, Align = 0x10, DefaultValue = null, IsArray = true }); //VF128
            Types.Add(new ArmpType() { CSType = typeof(double[]), IDv1 = -1, MemberTypeIDv1 = -1, IDv2 = 28, MemberTypeIDv2 = 28, IDOE = -1, Size = 8, Align = 0x10, DefaultValue = null, IsArray = true }); //VD256
            Types.Add(new ArmpType() { CSType = typeof(List<bool>), IDv1 = -1, MemberTypeIDv1 = -1, IDv2 = 29, MemberTypeIDv2 = 29, IDOE = -1, Size = 1, Align = 8, DefaultValue = null, IsArray = true }); //boolean array
        }


        /// <summary>
        /// Gets the <see cref="ArmpType"/> matching the provided C# Type.
        /// </summary>
        /// <param name="cstype">The C# type.</param>
        /// <returns>An <see cref="ArmpType"/>.</returns>
        /// <exception cref="TypeNotSupportedException">The provided C# type is not supported by the armp format.</exception>
        internal static ArmpType GetArmpTypeByCSType(Type cstype)
        {
            foreach (ArmpType type in Types)
            {
                if (type.CSType == cstype) return type;
            }
            throw new TypeNotSupportedException(cstype);
        }
    }
}
