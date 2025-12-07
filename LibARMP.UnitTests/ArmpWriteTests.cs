using LibARMP.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace LibARMP.UnitTests
{
    [TestClass]
    public class ArmpWriteTests
    {
        ///// V1 /////
        #region v1

        [TestMethod]
        public void WriteARMP_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(armp.Version, armp_new.Version);
            Assert.AreEqual(armp.Revision, armp_new.Revision);
            Assert.AreEqual(armp.FormatVersion, armp_new.FormatVersion);
            CollectionAssert.AreEqual(armp.GetMainTable().GetEntryNames(), armp_new.GetMainTable().GetEntryNames());
            CollectionAssert.AreEqual(armp.GetMainTable().GetColumnNames(), armp_new.GetMainTable().GetColumnNames());
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("string"), armp_new.GetMainTable().GetEntry(1).GetValueFromColumn("string"));
        }


        [TestMethod]
        public void u8_v1()
        {
            byte expectedValue = 123;
            string columnName = "u8";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void u16_v1()
        {
            UInt16 expectedValue = 12345;
            string columnName = "u16";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void u32_v1()
        {
            uint expectedValue = 123456789U;
            string columnName = "u32";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void u64_v1()
        {
            UInt64 expectedValue = 1234567891011121314UL;
            string columnName = "u64";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void s8_v1()
        {
            sbyte expectedValue = -123;
            string columnName = "s8";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void s16_v1()
        {
            short expectedValue = -12345;
            string columnName = "s16";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void s32_v1()
        {
            int expectedValue = -123456789;
            string columnName = "s32";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void s64_v1()
        {
            Int64 expectedValue = -1234567891011121314L;
            string columnName = "s64";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void f32_v1()
        {
            float expectedValue = 1.2345F;
            string columnName = "f32";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void bool_v1()
        {
            bool expectedValue = false;
            string columnName = "bool";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void string_v1()
        {
            string expectedValue = "this is a test string";
            string columnName = "string";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName), expectedValue);
        }


        [TestMethod]
        public void table_v1()
        {
            UInt64 expectedValue = 999999999UL;
            string columnName = "table";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            ArmpTable table = (ArmpTable)armp.GetMainTable().GetEntry("value").GetValueFromColumn(columnName);
            table.GetEntry(1).Name = "new_name";
            table.GetEntry(1).SetValueFromColumn("u64", expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            ArmpTable table_new = (ArmpTable)armp_new.GetMainTable().GetEntry("value").GetValueFromColumn(columnName);
            Assert.AreEqual(expectedValue, table_new.GetEntry(1).GetValueFromColumn("u64"));
            Assert.AreEqual("new_name", table_new.GetEntry(1).Name);
        }


        [TestMethod]
        public void entryValidity_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            armp.GetMainTable().GetEntry("value").IsValid = false;
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.IsFalse(armp_new.GetMainTable().GetEntry("value").IsValid);
        }


        [TestMethod]
        public void entryIndex_v1()
        {
            uint expectedValue = 123U;
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            armp.GetMainTable().GetEntry("value").Index = expectedValue;
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry("value").Index);
        }


        [TestMethod]
        public void entryName_v1()
        {
            string expectedValue = "test_name";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            armp.GetMainTable().GetEntry("value").Name = expectedValue;
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(expectedValue).Name);
        }


        [TestMethod]
        public void entryFlags_v1()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            armp.GetMainTable().GetEntry("value").Flags[0] = true;
            armp.GetMainTable().GetEntry("value").Flags[1] = false;
            armp.GetMainTable().GetEntry("value").Flags[2] = true;
            armp.GetMainTable().GetEntry("value").Flags[3] = false;
            armp.GetMainTable().GetEntry("value").Flags[4] = true;
            armp.GetMainTable().GetEntry("value").Flags[5] = false;
            armp.GetMainTable().GetEntry("value").Flags[6] = true;
            armp.GetMainTable().GetEntry("value").Flags[7] = false;
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.IsTrue(armp_new.GetMainTable().GetEntry("value").Flags[0]);
            Assert.IsFalse(armp_new.GetMainTable().GetEntry("value").Flags[1]);
            Assert.IsTrue(armp_new.GetMainTable().GetEntry("value").Flags[2]);
            Assert.IsFalse(armp_new.GetMainTable().GetEntry("value").Flags[3]);
            Assert.IsTrue(armp_new.GetMainTable().GetEntry("value").Flags[4]);
            Assert.IsFalse(armp_new.GetMainTable().GetEntry("value").Flags[5]);
            Assert.IsTrue(armp_new.GetMainTable().GetEntry("value").Flags[6]);
            Assert.IsFalse(armp_new.GetMainTable().GetEntry("value").Flags[7]);
        }
        #endregion



        ///// V2 MODE COLUMN /////
        #region v2ModeColumn

        [TestMethod]
        public void WriteARMP_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(armp.Version, armp_new.Version);
            Assert.AreEqual(armp.Revision, armp_new.Revision);
            Assert.AreEqual(armp.FormatVersion, armp_new.FormatVersion);
            Assert.AreEqual(armp.GetMainTable().TableInfo.StorageMode, armp_new.GetMainTable().TableInfo.StorageMode);
            CollectionAssert.AreEqual(armp.GetMainTable().GetEntryNames(), armp_new.GetMainTable().GetEntryNames());
            CollectionAssert.AreEqual(armp.GetMainTable().GetColumnNames(), armp_new.GetMainTable().GetColumnNames());
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("string"), armp_new.GetMainTable().GetEntry(1).GetValueFromColumn("string"));
        }


        [TestMethod]
        public void u8_v2ModeColumn()
        {
            byte expectedValue = 123;
            string columnName = "u8_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void u16_v2ModeColumn()
        {
            UInt16 expectedValue = 12345;
            string columnName = "u16_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void u32_v2ModeColumn()
        {
            uint expectedValue = 123456789U;
            string columnName = "u32_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void u64_v2ModeColumn()
        {
            UInt64 expectedValue = 1234567891011121314UL;
            string columnName = "u64_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void s8_v2ModeColumn()
        {
            sbyte expectedValue = -123;
            string columnName = "s8_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void s16_v2ModeColumn()
        {
            short expectedValue = -12345;
            string columnName = "s16_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void s32_v2ModeColumn()
        {
            int expectedValue = -123456789;
            string columnName = "s32_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void s64_v2ModeColumn()
        {
            Int64 expectedValue = -1234567891011121314L;
            string columnName = "s64_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void f32_v2ModeColumn()
        {
            float expectedValue = 1.2345F;
            string columnName = "f32_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void f64_v2ModeColumn()
        {
            double expectedValue = 1.23456789D;
            string columnName = "f64_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void bool_v2ModeColumn()
        {
            bool expectedValue = false;
            string columnName = "bool_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void string_v2ModeColumn()
        {
            string expectedValue = "this is a test string";
            string columnName = "string";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void table_v2ModeColumn()
        {
            byte expectedValue = 123;
            string columnName = "table";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            ArmpTable table = (ArmpTable)armp.GetMainTable().GetEntry("value").GetValueFromColumn(columnName);
            table.GetEntry(1).Name = "new_name";
            table.GetEntry(1).SetValueFromColumn("u8", expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            ArmpTable table_new = (ArmpTable)armp_new.GetMainTable().GetEntry("value").GetValueFromColumn(columnName);
            Assert.AreEqual(expectedValue, table_new.GetEntry(1).GetValueFromColumn("u8"));
            Assert.AreEqual("new_name", table_new.GetEntry(1).Name);
        }


        [TestMethod]
        public void entryValidity_v2ModeColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().GetEntry("value").IsValid = false;
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.IsFalse(armp_new.GetMainTable().GetEntry("value").IsValid);
        }


        [TestMethod]
        public void entryIndex_v2ModeColumn()
        {
            uint expectedValue = 123u;
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().GetEntry("value").Index = expectedValue;
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry("value").Index);
        }


        [TestMethod]
        public void entryName_v2ModeColumn()
        {
            string expectedValue = "test_name";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().GetEntry("value").Name = expectedValue;
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(expectedValue).Name);
        }
        #endregion



        ///// V2 MODE ENTRY /////
        #region v2ModeEntry

        [TestMethod]
        public void WriteARMP_v2ModeEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(armp.Version, armp_new.Version);
            Assert.AreEqual(armp.Revision, armp_new.Revision);
            Assert.AreEqual(armp.FormatVersion, armp_new.FormatVersion);
            Assert.AreEqual(armp.GetMainTable().TableInfo.StorageMode, armp_new.GetMainTable().TableInfo.StorageMode);
            CollectionAssert.AreEqual(armp.GetMainTable().GetEntryNames(), armp_new.GetMainTable().GetEntryNames());
            CollectionAssert.AreEqual(armp.GetMainTable().GetColumnNames(), armp_new.GetMainTable().GetColumnNames());
            Assert.AreEqual(armp.GetMainTable().GetEntry(1).GetValueFromColumn("string"), armp_new.GetMainTable().GetEntry(1).GetValueFromColumn("string"));
        }


        [TestMethod]
        public void u8_v2ModeEntry()
        {
            byte expectedValue = 123;
            string columnName = "u8_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void u16_v2ModeEntry()
        {
            UInt16 expectedValue = 12345;
            string columnName = "u16_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void u32_v2ModeEntry()
        {
            uint expectedValue = 123456789U;
            string columnName = "u32_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void u64_v2ModeEntry()
        {
            UInt64 expectedValue = 1234567891011121314UL;
            string columnName = "u64_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void s8_v2ModeEntry()
        {
            sbyte expectedValue = -123;
            string columnName = "s8_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void s16_v2ModeEntry()
        {
            short expectedValue = -12345;
            string columnName = "s16_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void s32_v2ModeEntry()
        {
            int expectedValue = -123456789;
            string columnName = "s32_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void s64_v2ModeEntry()
        {
            Int64 expectedValue = -1234567891011121314L;
            string columnName = "s64_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void f32_v2ModeEntry()
        {
            float expectedValue = 1.2345F;
            string columnName = "f32_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void f64_v2ModeEntry()
        {
            double expectedValue = 1.23456789D;
            string columnName = "f64_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void bool_v2ModeEntry()
        {
            bool expectedValue = false;
            string columnName = "bool_";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void string_v2ModeEntry()
        {
            string expectedValue = "this is a test string";
            string columnName = "string";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            armp.GetMainTable().GetEntry("value").SetValueFromColumn(columnName, expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(1).GetValueFromColumn(columnName));
        }


        [TestMethod]
        public void table_v2ModeEntry()
        {
            byte expectedValue = 123;
            string columnName = "table";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            ArmpTable table = (ArmpTable)armp.GetMainTable().GetEntry("value").GetValueFromColumn(columnName);
            table.GetEntry(1).Name = "new_name";
            table.GetEntry(1).SetValueFromColumn("u8", expectedValue);
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            ArmpTable table_new = (ArmpTable)armp_new.GetMainTable().GetEntry("value").GetValueFromColumn(columnName);
            Assert.AreEqual(expectedValue, table_new.GetEntry(1).GetValueFromColumn("u8"));
            Assert.AreEqual("new_name", table_new.GetEntry(1).Name);
        }


        [TestMethod]
        public void entryValidity_v2ModeEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            armp.GetMainTable().GetEntry("value").IsValid = false;
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.IsFalse(armp_new.GetMainTable().GetEntry("value").IsValid);
        }


        [TestMethod]
        public void entryIndex_v2ModeEntry()
        {
            uint expectedValue = 123U;
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            armp.GetMainTable().GetEntry("value").Index = expectedValue;
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry("value").Index);
        }


        [TestMethod]
        public void entryName_v2ModeEntry()
        {
            string expectedValue = "test_name";
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeEntry);
            armp.GetMainTable().GetEntry("value").Name = expectedValue;
            Stream stream = ArmpFileWriter.WriteARMPToStream(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(expectedValue, armp_new.GetMainTable().GetEntry(expectedValue).Name);
        }
        #endregion
    }
}
