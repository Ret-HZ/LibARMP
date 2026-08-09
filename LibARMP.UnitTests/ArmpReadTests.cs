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
            Assert.AreEqual(armp.Version, 1);
            Assert.AreEqual(armp.Revision, 12);
            Assert.AreEqual(armp.FormatVersion, Version.DragonEngineV1);
        }


        [TestMethod]
        public void u8_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("u8"), (byte)32); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("u8"), (byte)0); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("u8"), (byte)255); //max
        }


        [TestMethod]
        public void u16_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("u16"), (UInt16)800); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("u16"), (UInt16)0); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("u16"), (UInt16)65535); //max
        }


        [TestMethod]
        public void u32_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("u32"), (uint)12345678); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("u32"), (uint)0); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("u32"), (uint)4294967295); //max
        }


        [TestMethod]
        public void u64_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("u64"), (UInt64)1000000000000); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("u64"), (UInt64)0); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("u64"), (UInt64)18446744073709551615); //max
        }


        [TestMethod]
        public void s8_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("s8"), (sbyte)(-32)); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("s8"), (sbyte)(-128)); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("s8"), (sbyte)127); //max
        }


        [TestMethod]
        public void s16_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("s16"), (Int16)(-800)); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("s16"), (Int16)(-32768)); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("s16"), (Int16)32767); //max
        }


        [TestMethod]
        public void s32_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("s32"), (Int32)(-12345678)); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("s32"), (Int32)(-2147483648)); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("s32"), (Int32)2147483647); //max
        }


        [TestMethod]
        public void s64_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("s64"), (Int64)(-1000000000000)); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("s64"), (Int64)(-9223372036854770000)); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("s64"), (Int64)9223372036854770000); //max
        }


        [TestMethod]
        public void f32_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("f32"), (float)(112.6)); //value
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
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("string"), "test_string"); //value
        }


        [TestMethod]
        public void table_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            ArmpTable table = (ArmpTable)armp.GetMainTable().GetEntry(1).GetValueFromColumn("table");
            Assert.AreEqual(table.GetEntry(1).GetValueFromColumn("u64"), (UInt64)1234567891011121314);
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
            Assert.AreEqual((uint)0, armp.GetMainTable().GetEntry(0).Index);
            Assert.AreEqual((uint)1, armp.GetMainTable().GetEntry(1).Index);
            Assert.AreEqual((uint)2, armp.GetMainTable().GetEntry(2).Index);
            Assert.AreEqual((uint)3, armp.GetMainTable().GetEntry(3).Index);
        }


        [TestMethod]
        public void entryName_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Assert.AreEqual(armp.GetMainTable().GetEntry(0).Name, "");
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).Name, "value");
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).Name, "value_min");
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).Name, "value_max");
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
            Assert.AreEqual(armp.Version, 2);
            Assert.AreEqual(armp.Revision, 0);
            Assert.AreEqual(armp.GetMainTable().TableInfo.StorageMode, StorageMode.Column);
        }


        [TestMethod]
        public void u8_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("u8_"), (byte)32); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("u8_"), (byte)0); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("u8_"), (byte)255); //max
        }


        [TestMethod]
        public void u16_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("u16_"), (UInt16)800); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("u16_"), (UInt16)0); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("u16_"), (UInt16)65535); //max
        }


        [TestMethod]
        public void u32_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("u32_"), (uint)12345678); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("u32_"), (uint)0); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("u32_"), (uint)4294967295); //max
        }


        [TestMethod]
        public void u64_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("u64_"), (UInt64)1000000000000); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("u64_"), (UInt64)0); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("u64_"), (UInt64)18446744073709551615); //max
        }


        [TestMethod]
        public void s8_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("s8_"), (sbyte)(-32)); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("s8_"), (sbyte)(-128)); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("s8_"), (sbyte)127); //max
        }


        [TestMethod]
        public void s16_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("s16_"), (Int16)(-800)); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("s16_"), (Int16)(-32768)); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("s16_"), (Int16)32767); //max
        }


        [TestMethod]
        public void s32_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("s32_"), (Int32)(-12345678)); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("s32_"), (Int32)(-2147483648)); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("s32_"), (Int32)2147483647); //max
        }


        [TestMethod]
        public void s64_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("s64_"), (Int64)(-1000000000000)); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("s64_"), (Int64)(-9223372036854770000)); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("s64_"), (Int64)9223372036854770000); //max
        }


        [TestMethod]
        public void f32_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("f32_"), (float)(112.6)); //value
        }


        [TestMethod]
        public void f64_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("f64_"), (double)(123.8888)); //value
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
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("string"), "a"); //value
        }


        [TestMethod]
        public void table_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            ArmpTable table = (ArmpTable)armp.GetMainTable().GetEntry(1).GetValueFromColumn("table");
            Assert.AreEqual(table.GetEntry(2).GetValueFromColumn("u8"), (byte)64);
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
            Assert.AreEqual((uint)0, armp.GetMainTable().GetEntry(0).Index);
            Assert.AreEqual((uint)1, armp.GetMainTable().GetEntry(1).Index);
            Assert.AreEqual((uint)2, armp.GetMainTable().GetEntry(2).Index);
            Assert.AreEqual((uint)3, armp.GetMainTable().GetEntry(3).Index);
        }


        [TestMethod]
        public void entryName_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Assert.AreEqual(armp.GetMainTable().GetEntry(0).Name, "");
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).Name, "value");
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).Name, "min_value");
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).Name, "max_value");
        }
        #endregion



        ///// V2 MODE STRUCTURED /////
        #region v2ModeStructured

        [TestMethod]
        public void ReadARMP_v2ModeStructured()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);
            Assert.AreEqual(armp.Version, 2);
            Assert.AreEqual(armp.Revision, 0);
            Assert.AreEqual(armp.GetMainTable().TableInfo.StorageMode, StorageMode.Structured);
        }


        [TestMethod]
        public void u8_v2ModeStructured()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("u8_"), (byte)32); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("u8_"), (byte)0); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("u8_"), (byte)255); //max
        }


        [TestMethod]
        public void u16_v2ModeStructured()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("u16_"), (UInt16)800); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("u16_"), (UInt16)0); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("u16_"), (UInt16)65535); //max
        }


        [TestMethod]
        public void u32_v2ModeStructured()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("u32_"), (uint)12345678); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("u32_"), (uint)0); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("u32_"), (uint)4294967295); //max
        }


        [TestMethod]
        public void u64_v2ModeStructured()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("u64_"), (UInt64)1000000000000); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("u64_"), (UInt64)0); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("u64_"), (UInt64)18446744073709551615); //max
        }


        [TestMethod]
        public void s8_v2ModeStructured()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("s8_"), (sbyte)(-32)); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("s8_"), (sbyte)(-128)); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("s8_"), (sbyte)127); //max
        }


        [TestMethod]
        public void s16_v2ModeStructured()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("s16_"), (Int16)(-800)); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("s16_"), (Int16)(-32768)); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("s16_"), (Int16)32767); //max
        }


        [TestMethod]
        public void s32_v2ModeStructured()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("s32_"), (Int32)(-12345678)); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("s32_"), (Int32)(-2147483648)); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("s32_"), (Int32)2147483647); //max
        }


        [TestMethod]
        public void s64_v2ModeStructured()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("s64_"), (Int64)(-1000000000000)); //value
            Assert.AreEqual(armp.GetMainTable().GetEntry(2).GetValueFromColumn("s64_"), (Int64)(-9223372036854770000)); //min
            Assert.AreEqual(armp.GetMainTable().GetEntry(3).GetValueFromColumn("s64_"), (Int64)9223372036854770000); //max
        }


        [TestMethod]
        public void f32_v2ModeStructured()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("f32_"), (float)(112.6)); //value
        }


        [TestMethod]
        public void f64_v2ModeStructured()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("f64_"), (double)(123.8888)); //value
        }


        [TestMethod]
        public void bool_v2ModeStructured()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);
            ArmpEntry entry = armp.GetMainTable().GetEntry(1);
            Console.WriteLine("value: " +entry.GetValueFromColumn("bool_"));
            Console.WriteLine("type: " + armp.GetMainTable().GetColumn("bool_").GetDataType());
            Assert.IsFalse(armp.GetMainTable().GetEntry(1).GetValueFromColumn<bool>("bool_")); //value
        }


        [TestMethod]
        public void string_v2ModeStructured()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("string"), "a"); //value
        }


        [TestMethod]
        public void table_v2ModeStructured()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);
            ArmpTable table = (ArmpTable)armp.GetMainTable().GetEntry(1).GetValueFromColumn("table");
            Assert.AreEqual(table.GetEntry(2).GetValueFromColumn("u8"), (byte)64);
        }


        [TestMethod]
        public void entryValidity_v2ModeStructured()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);
            Assert.IsFalse(armp.GetMainTable().GetEntry(0).IsValid);
            Assert.IsTrue(armp.GetMainTable().GetEntry(1).IsValid);
        }
        #endregion
    }
}
