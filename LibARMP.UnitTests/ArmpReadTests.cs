using LibARMP.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LibARMP.UnitTests
{
    [TestClass]
    public class ArmpReadTests
    {
        ///// V1 /////
        #region v1

        [TestMethod]
        public void ReadARMP_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.AreEqual(1, armp.Version);
            Assert.AreEqual(12, armp.Revision);
            Assert.AreEqual(Version.DragonEngineV1, armp.FormatVersion);
        }


        [TestMethod]
        public void u8_v1()
        {
            ArmpTable armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes).GetMainTable();
            Assert.AreEqual((byte)32, armp.GetEntry(1).GetValueFromColumn("u8")); //value
            Assert.AreEqual((byte)0, armp.GetEntry(2).GetValueFromColumn("u8")); //min
            Assert.AreEqual((byte)255, armp.GetEntry(3).GetValueFromColumn("u8")); //max
        }


        [TestMethod]
        public void u16_v1()
        {
            ArmpTable armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes).GetMainTable();
            Assert.AreEqual((UInt16)800, armp.GetEntry(1).GetValueFromColumn("u16")); //value
            Assert.AreEqual((UInt16)0, armp.GetEntry(2).GetValueFromColumn("u16")); //min
            Assert.AreEqual((UInt16)65535, armp.GetEntry(3).GetValueFromColumn("u16")); //max
        }


        [TestMethod]
        public void u32_v1()
        {
            ArmpTable armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes).GetMainTable();
            Assert.AreEqual(12345678U, armp.GetEntry(1).GetValueFromColumn("u32")); //value
            Assert.AreEqual(0U, armp.GetEntry(2).GetValueFromColumn("u32")); //min
            Assert.AreEqual(4294967295U, armp.GetEntry(3).GetValueFromColumn("u32")); //max
        }


        [TestMethod]
        public void u64_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.AreEqual(1000000000000UL, armp.GetMainTable().GetEntry(1).GetValueFromColumn("u64")); //value
            Assert.AreEqual(0UL, armp.GetMainTable().GetEntry(2).GetValueFromColumn("u64")); //min
            Assert.AreEqual(18446744073709551615UL, armp.GetMainTable().GetEntry(3).GetValueFromColumn("u64")); //max
        }


        [TestMethod]
        public void s8_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.AreEqual((sbyte)(-32), armp.GetMainTable().GetEntry(1).GetValueFromColumn("s8")); //value
            Assert.AreEqual((sbyte)(-128), armp.GetMainTable().GetEntry(2).GetValueFromColumn("s8")); //min
            Assert.AreEqual((sbyte)127, armp.GetMainTable().GetEntry(3).GetValueFromColumn("s8")); //max
        }


        [TestMethod]
        public void s16_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.AreEqual((Int16)(-800), armp.GetMainTable().GetEntry(1).GetValueFromColumn("s16")); //value
            Assert.AreEqual((Int16)(-32768), armp.GetMainTable().GetEntry(2).GetValueFromColumn("s16")); //min
            Assert.AreEqual((Int16)32767, armp.GetMainTable().GetEntry(3).GetValueFromColumn("s16")); //max
        }


        [TestMethod]
        public void s32_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.AreEqual(-12345678, armp.GetMainTable().GetEntry(1).GetValueFromColumn("s32")); //value
            Assert.AreEqual(-2147483648, armp.GetMainTable().GetEntry(2).GetValueFromColumn("s32")); //min
            Assert.AreEqual(2147483647, armp.GetMainTable().GetEntry(3).GetValueFromColumn("s32")); //max
        }


        [TestMethod]
        public void s64_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.AreEqual(-1000000000000L, armp.GetMainTable().GetEntry(1).GetValueFromColumn("s64")); //value
            Assert.AreEqual(-9223372036854770000L, armp.GetMainTable().GetEntry(2).GetValueFromColumn("s64")); //min
            Assert.AreEqual(9223372036854770000L, armp.GetMainTable().GetEntry(3).GetValueFromColumn("s64")); //max
        }


        [TestMethod]
        public void f32_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.AreEqual(112.6F, armp.GetMainTable().GetEntry(1).GetValueFromColumn("f32")); //value
        }


        [TestMethod]
        public void bool_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.IsTrue((bool)armp.GetMainTable().GetEntry(1).GetValueFromColumn("bool")); //value
        }


        [TestMethod]
        public void string_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.AreEqual("test_string", armp.GetMainTable().GetEntry(1).GetValueFromColumn("string")); //value
        }


        [TestMethod]
        public void table_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            ArmpTable table = (ArmpTable)armp.GetMainTable().GetEntry(1).GetValueFromColumn("table");
            Assert.AreEqual(1234567891011121314UL, table.GetEntry(1).GetValueFromColumn("u64"));
        }


        [TestMethod]
        public void entryValidity_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.IsFalse(armp.GetMainTable().GetEntry(0).IsValid);
            Assert.IsTrue(armp.GetMainTable().GetEntry(1).IsValid);
            Assert.IsTrue(armp.GetMainTable().GetEntry(2).IsValid);
            Assert.IsTrue(armp.GetMainTable().GetEntry(2).IsValid);
        }


        [TestMethod]
        public void entryIndex_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.AreEqual(0U, armp.GetMainTable().GetEntry(0).Index);
            Assert.AreEqual(1U, armp.GetMainTable().GetEntry(1).Index);
            Assert.AreEqual(2U, armp.GetMainTable().GetEntry(2).Index);
            Assert.AreEqual(3U, armp.GetMainTable().GetEntry(3).Index);
        }


        [TestMethod]
        public void entryName_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.AreEqual("", armp.GetMainTable().GetEntry(0).Name);
            Assert.AreEqual("value", armp.GetMainTable().GetEntry(1).Name);
            Assert.AreEqual("value_min", armp.GetMainTable().GetEntry(2).Name);
            Assert.AreEqual("value_max", armp.GetMainTable().GetEntry(3).Name);
        }


        [TestMethod]
        public void entryFlags_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.IsFalse(armp.GetMainTable().GetEntry(1).Flags[0]);
            Assert.IsTrue(armp.GetMainTable().GetEntry(1).Flags[1]);
            Assert.IsTrue(armp.GetMainTable().GetEntry(1).Flags[2]);
            Assert.IsFalse(armp.GetMainTable().GetEntry(1).Flags[3]);
            Assert.IsFalse(armp.GetMainTable().GetEntry(1).Flags[4]);
            Assert.IsFalse(armp.GetMainTable().GetEntry(1).Flags[5]);
            Assert.IsFalse(armp.GetMainTable().GetEntry(1).Flags[6]);
            Assert.IsFalse(armp.GetMainTable().GetEntry(1).Flags[7]);

            Assert.IsFalse(armp.GetMainTable().GetEntry(2).Flags[0]);
            Assert.IsFalse(armp.GetMainTable().GetEntry(2).Flags[1]);
            Assert.IsFalse(armp.GetMainTable().GetEntry(2).Flags[2]);
            Assert.IsFalse(armp.GetMainTable().GetEntry(2).Flags[3]);
            Assert.IsFalse(armp.GetMainTable().GetEntry(2).Flags[4]);
            Assert.IsFalse(armp.GetMainTable().GetEntry(2).Flags[5]);
            Assert.IsFalse(armp.GetMainTable().GetEntry(2).Flags[6]);
            Assert.IsFalse(armp.GetMainTable().GetEntry(2).Flags[7]);

            Assert.IsTrue(armp.GetMainTable().GetEntry(3).Flags[0]);
            Assert.IsTrue(armp.GetMainTable().GetEntry(3).Flags[1]);
            Assert.IsTrue(armp.GetMainTable().GetEntry(3).Flags[2]);
            Assert.IsTrue(armp.GetMainTable().GetEntry(3).Flags[3]);
            Assert.IsTrue(armp.GetMainTable().GetEntry(3).Flags[4]);
            Assert.IsTrue(armp.GetMainTable().GetEntry(3).Flags[5]);
            Assert.IsTrue(armp.GetMainTable().GetEntry(3).Flags[6]);
            Assert.IsTrue(armp.GetMainTable().GetEntry(3).Flags[7]);
        }
        #endregion



        ///// V2 MODE COLUMN /////
        #region v2ModeColumn

        [TestMethod]
        public void ReadARMP_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual(2, armp.Version);
            Assert.AreEqual(0, armp.Revision);
            Assert.AreEqual(StorageMode.Column, armp.GetMainTable().TableInfo.StorageMode);
        }


        [TestMethod]
        public void u8_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual((byte)32, armp.GetMainTable().GetEntry(1).GetValueFromColumn("u8_")); //value
            Assert.AreEqual((byte)0, armp.GetMainTable().GetEntry(2).GetValueFromColumn("u8_")); //min
            Assert.AreEqual((byte)255, armp.GetMainTable().GetEntry(3).GetValueFromColumn("u8_")); //max
        }


        [TestMethod]
        public void u16_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual((UInt16)800, armp.GetMainTable().GetEntry(1).GetValueFromColumn("u16_")); //value
            Assert.AreEqual((UInt16)0, armp.GetMainTable().GetEntry(2).GetValueFromColumn("u16_")); //min
            Assert.AreEqual((UInt16)65535, armp.GetMainTable().GetEntry(3).GetValueFromColumn("u16_")); //max
        }


        [TestMethod]
        public void u32_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual(12345678U, armp.GetMainTable().GetEntry(1).GetValueFromColumn("u32_")); //value
            Assert.AreEqual(0U, armp.GetMainTable().GetEntry(2).GetValueFromColumn("u32_")); //min
            Assert.AreEqual(4294967295U, armp.GetMainTable().GetEntry(3).GetValueFromColumn("u32_")); //max
        }


        [TestMethod]
        public void u64_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual(1000000000000UL, armp.GetMainTable().GetEntry(1).GetValueFromColumn("u64_")); //value
            Assert.AreEqual(0UL, armp.GetMainTable().GetEntry(2).GetValueFromColumn("u64_")); //min
            Assert.AreEqual(18446744073709551615UL, armp.GetMainTable().GetEntry(3).GetValueFromColumn("u64_")); //max
        }


        [TestMethod]
        public void s8_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual((sbyte)(-32), armp.GetMainTable().GetEntry(1).GetValueFromColumn("s8_")); //value
            Assert.AreEqual((sbyte)(-128), armp.GetMainTable().GetEntry(2).GetValueFromColumn("s8_")); //min
            Assert.AreEqual((sbyte)127, armp.GetMainTable().GetEntry(3).GetValueFromColumn("s8_")); //max
        }


        [TestMethod]
        public void s16_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual((Int16)(-800), armp.GetMainTable().GetEntry(1).GetValueFromColumn("s16_")); //value
            Assert.AreEqual((Int16)(-32768), armp.GetMainTable().GetEntry(2).GetValueFromColumn("s16_")); //min
            Assert.AreEqual((Int16)32767, armp.GetMainTable().GetEntry(3).GetValueFromColumn("s16_")); //max
        }


        [TestMethod]
        public void s32_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual(-12345678, armp.GetMainTable().GetEntry(1).GetValueFromColumn("s32_")); //value
            Assert.AreEqual(-2147483648, armp.GetMainTable().GetEntry(2).GetValueFromColumn("s32_")); //min
            Assert.AreEqual(2147483647, armp.GetMainTable().GetEntry(3).GetValueFromColumn("s32_")); //max
        }


        [TestMethod]
        public void s64_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual(-1000000000000L, armp.GetMainTable().GetEntry(1).GetValueFromColumn("s64_")); //value
            Assert.AreEqual(-9223372036854770000L, armp.GetMainTable().GetEntry(2).GetValueFromColumn("s64_")); //min
            Assert.AreEqual(9223372036854770000L, armp.GetMainTable().GetEntry(3).GetValueFromColumn("s64_")); //max
        }


        [TestMethod]
        public void f32_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual(112.6F, armp.GetMainTable().GetEntry(1).GetValueFromColumn("f32_")); //value
        }


        [TestMethod]
        public void f64_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual(123.8888D, armp.GetMainTable().GetEntry(1).GetValueFromColumn("f64_")); //value
        }


        [TestMethod]
        public void bool_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.IsFalse((bool)armp.GetMainTable().GetEntry(1).GetValueFromColumn("bool_")); //value
        }


        [TestMethod]
        public void string_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual("a", armp.GetMainTable().GetEntry(1).GetValueFromColumn("string")); //value
        }


        [TestMethod]
        public void table_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            ArmpTable table = (ArmpTable)armp.GetMainTable().GetEntry(1).GetValueFromColumn("table");
            Assert.AreEqual((byte)64, table.GetEntry(2).GetValueFromColumn("u8"));
        }


        [TestMethod]
        public void entryValidity_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.IsFalse(armp.GetMainTable().GetEntry(0).IsValid);
            Assert.IsTrue(armp.GetMainTable().GetEntry(1).IsValid);
        }


        [TestMethod]
        public void entryIndex_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual(0U, armp.GetMainTable().GetEntry(0).Index);
            Assert.AreEqual(1U, armp.GetMainTable().GetEntry(1).Index);
            Assert.AreEqual(2U, armp.GetMainTable().GetEntry(2).Index);
            Assert.AreEqual(3U, armp.GetMainTable().GetEntry(3).Index);
        }


        [TestMethod]
        public void entryName_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual("", armp.GetMainTable().GetEntry(0).Name);
            Assert.AreEqual("value", armp.GetMainTable().GetEntry(1).Name);
            Assert.AreEqual("min_value", armp.GetMainTable().GetEntry(2).Name);
            Assert.AreEqual("max_value", armp.GetMainTable().GetEntry(3).Name);
        }
        #endregion



        ///// V2 MODE ENTRY /////
        #region v2ModeEntry

        [TestMethod]
        public void ReadARMP_v2ModeEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            Assert.AreEqual(2, armp.Version);
            Assert.AreEqual(0, armp.Revision);
            Assert.AreEqual(StorageMode.Structured, armp.GetMainTable().TableInfo.StorageMode);
        }


        [TestMethod]
        public void u8_v2ModeEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            Assert.AreEqual((byte)32, armp.GetMainTable().GetEntry(1).GetValueFromColumn("u8_")); //value
            Assert.AreEqual((byte)0, armp.GetMainTable().GetEntry(2).GetValueFromColumn("u8_")); //min
            Assert.AreEqual((byte)255, armp.GetMainTable().GetEntry(3).GetValueFromColumn("u8_")); //max
        }


        [TestMethod]
        public void u16_v2ModeEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            Assert.AreEqual((UInt16)800, armp.GetMainTable().GetEntry(1).GetValueFromColumn("u16_")); //value
            Assert.AreEqual((UInt16)0, armp.GetMainTable().GetEntry(2).GetValueFromColumn("u16_")); //min
            Assert.AreEqual((UInt16)65535, armp.GetMainTable().GetEntry(3).GetValueFromColumn("u16_")); //max
        }


        [TestMethod]
        public void u32_v2ModeEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            Assert.AreEqual(12345678U, armp.GetMainTable().GetEntry(1).GetValueFromColumn("u32_")); //value
            Assert.AreEqual(0U, armp.GetMainTable().GetEntry(2).GetValueFromColumn("u32_")); //min
            Assert.AreEqual(4294967295U, armp.GetMainTable().GetEntry(3).GetValueFromColumn("u32_")); //max
        }


        [TestMethod]
        public void u64_v2ModeEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            Assert.AreEqual(1000000000000UL, armp.GetMainTable().GetEntry(1).GetValueFromColumn("u64_")); //value
            Assert.AreEqual(0UL, armp.GetMainTable().GetEntry(2).GetValueFromColumn("u64_")); //min
            Assert.AreEqual(18446744073709551615UL, armp.GetMainTable().GetEntry(3).GetValueFromColumn("u64_")); //max
        }


        [TestMethod]
        public void s8_v2ModeEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            Assert.AreEqual((sbyte)(-32), armp.GetMainTable().GetEntry(1).GetValueFromColumn("s8_")); //value
            Assert.AreEqual((sbyte)(-128), armp.GetMainTable().GetEntry(2).GetValueFromColumn("s8_")); //min
            Assert.AreEqual((sbyte)127, armp.GetMainTable().GetEntry(3).GetValueFromColumn("s8_")); //max
        }


        [TestMethod]
        public void s16_v2ModeEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            Assert.AreEqual((Int16)(-800), armp.GetMainTable().GetEntry(1).GetValueFromColumn("s16_")); //value
            Assert.AreEqual((Int16)(-32768), armp.GetMainTable().GetEntry(2).GetValueFromColumn("s16_")); //min
            Assert.AreEqual((Int16)32767, armp.GetMainTable().GetEntry(3).GetValueFromColumn("s16_")); //max
        }


        [TestMethod]
        public void s32_v2ModeEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            Assert.AreEqual(-12345678, armp.GetMainTable().GetEntry(1).GetValueFromColumn("s32_")); //value
            Assert.AreEqual(-2147483648, armp.GetMainTable().GetEntry(2).GetValueFromColumn("s32_")); //min
            Assert.AreEqual(2147483647, armp.GetMainTable().GetEntry(3).GetValueFromColumn("s32_")); //max
        }


        [TestMethod]
        public void s64_v2ModeEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            Assert.AreEqual(-1000000000000L, armp.GetMainTable().GetEntry(1).GetValueFromColumn("s64_")); //value
            Assert.AreEqual(-9223372036854770000L, armp.GetMainTable().GetEntry(2).GetValueFromColumn("s64_")); //min
            Assert.AreEqual(9223372036854770000L, armp.GetMainTable().GetEntry(3).GetValueFromColumn("s64_")); //max
        }


        [TestMethod]
        public void f32_v2ModeEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            Assert.AreEqual(112.6F, armp.GetMainTable().GetEntry(1).GetValueFromColumn("f32_")); //value
        }


        [TestMethod]
        public void f64_v2ModeEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            Assert.AreEqual(123.8888D, armp.GetMainTable().GetEntry(1).GetValueFromColumn("f64_")); //value
        }


        [TestMethod]
        public void bool_v2ModeEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            Assert.IsFalse((bool)armp.GetMainTable().GetEntry(1).GetValueFromColumn("bool_")); //value
        }


        [TestMethod]
        public void string_v2ModeEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            Assert.AreEqual("a", armp.GetMainTable().GetEntry(1).GetValueFromColumn("string")); //value
        }


        [TestMethod]
        public void table_v2ModeEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            ArmpTable table = (ArmpTable)armp.GetMainTable().GetEntry(1).GetValueFromColumn("table");
            Assert.AreEqual((byte)64, table.GetEntry(2).GetValueFromColumn("u8"));
        }


        [TestMethod]
        public void entryValidity_v2ModeEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            Assert.IsFalse(armp.GetMainTable().GetEntry(0).IsValid);
            Assert.IsTrue(armp.GetMainTable().GetEntry(1).IsValid);
        }
        #endregion
    }
}
