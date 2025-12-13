using LibARMP.Exceptions;
using LibARMP.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace LibARMP.UnitTests
{
    [TestClass]
    public class APITests
    {
        ///// ArmpEntry /////
        #region ArmpEntry

        [TestMethod]
        public void ArmpEntry_ID()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            uint id = armp.GetMainTable().GetEntry("value").ID;
            Assert.AreEqual(1u, id);
        }


        [TestMethod]
        public void ArmpEntry_Name()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            string name = armp.GetMainTable().GetEntry(1).Name;
            Assert.AreEqual("value", name);
        }


        [TestMethod]
        public void ArmpEntry_Index()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            uint index = armp.GetMainTable().GetEntry("value").Index;
            Assert.AreEqual((uint)1, index);
        }


        [TestMethod]
        public void ArmpEntry_IsValid()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            bool valid = armp.GetMainTable().GetEntry("value").IsValid;
            Assert.IsTrue(valid);
        }


        [TestMethod]
        public void ArmpEntry_Flags()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            bool[] flags = armp.GetMainTable().GetEntry("value").Flags;
            Assert.IsFalse(flags[0]);
            Assert.IsTrue(flags[1]);
            Assert.IsTrue(flags[2]);
            Assert.IsFalse(flags[3]);
            Assert.IsFalse(flags[4]);
            Assert.IsFalse(flags[5]);
            Assert.IsFalse(flags[6]);
            Assert.IsFalse(flags[7]);
        }


        [TestMethod]
        public void ArmpEntry_Copy()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            ArmpEntry entry = armp.GetMainTable().GetEntry("value");
            ArmpEntry copy = entry.Copy(armp.GetMainTable());
            Assert.AreEqual(entry.GetValueFromColumn<Int32>("s32_"), copy.GetValueFromColumn<Int32>("s32_"));
            Assert.AreEqual(entry.GetValueFromColumn<double>("f64_"), copy.GetValueFromColumn<double>("f64_"));
            Assert.AreEqual(entry.GetValueFromColumn<string>("string"), copy.GetValueFromColumn<string>("string"));
        }


        [TestMethod]
        public void ArmpEntry_GetValueFromColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            ArmpEntry entry = armp.GetMainTable().GetEntry("value");
            ArmpTableColumn column = armp.GetMainTable().GetColumn("u16_");
            Assert.AreEqual((UInt16)800, entry.GetValueFromColumn<UInt16>("u16_"));
            Assert.AreEqual((UInt16)800, entry.GetValueFromColumn<UInt16>(2));
            Assert.AreEqual((UInt16)800, entry.GetValueFromColumn<UInt16>(column));
            Assert.AreEqual((UInt16)800, entry.GetValueFromColumn("u16_"));
            Assert.AreEqual((UInt16)800, entry.GetValueFromColumn(2));
            Assert.AreEqual((UInt16)800, entry.GetValueFromColumn(column));
        }


        [TestMethod]
        public void ArmpEntry_SetValueFromColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            ArmpEntry entry = armp.GetMainTable().GetEntry("value");
            UInt64 expected1 = (UInt64)77777777777;
            UInt32 expected2 = (UInt32)666666666;
            UInt16 expected3 = (UInt16)55555;
            entry.SetValueFromColumn("u64_", expected1);
            entry.SetValueFromColumn(3, expected2); // u32_
            entry.SetValueFromColumn(armp.GetMainTable().GetColumn("u16_"), (Int64)expected3);
            var result1 = entry.GetValueFromColumn("u64_");
            var result2 = entry.GetValueFromColumn("u32_");
            var result3 = entry.GetValueFromColumn("u16_");
            Assert.AreEqual(expected1, result1);
            Assert.AreEqual(expected2, result2);
            Assert.AreEqual(expected3, result3);
        }
        #endregion



        ///// ArmpTableColumn /////
        #region ArmpTableColumn

        [TestMethod]
        public void ArmpTableColumn_ID()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            uint id = armp.GetMainTable().GetColumn("u32_").ID;
            Assert.AreEqual(3u, id);
        }


        [TestMethod]
        public void ArmpTableColumn_Name()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            string name = armp.GetMainTable().GetColumn("u32_").Name;
            Assert.AreEqual("u32_", name);
        }


        [TestMethod]
        public void ArmpTableColumn_Index()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            int index = armp.GetMainTable().GetColumn("u32_").Index;
            Assert.AreEqual(4, index);
        }


        [TestMethod]
        public void ArmpTableColumn_IsValid()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            bool valid = (bool)armp.GetMainTable().GetColumn("u32_").IsValid;
            Assert.IsTrue(valid);
        }


        [TestMethod]
        public void ArmpTableColumn_GetDataType()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            ArmpTableColumn c1 = armp.GetMainTable().GetColumn("u32_");
            ArmpTableColumn c2 = armp.GetMainTable().GetColumn("s64_");
            ArmpTableColumn c3 = armp.GetMainTable().GetColumn("f32_");
            ArmpTableColumn c4 = armp.GetMainTable().GetColumn("bool_");

            Assert.AreEqual(typeof(uint), c1.GetDataType());
            Assert.AreEqual(typeof(Int64), c2.GetDataType());
            Assert.AreEqual(typeof(float), c3.GetDataType());
            Assert.AreEqual(typeof(bool), c4.GetDataType());
        }


        [TestMethod]
        public void ArmpTableColumn_Copy()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            ArmpTableColumn column = armp.GetMainTable().GetColumn("u32_");
            ArmpTableColumn copy = column.Copy();
            Assert.AreEqual(column.Name, copy.Name);
            Assert.AreEqual(column.Index, copy.Index);
            Assert.AreEqual(column.IsValid, copy.IsValid);
            Assert.AreEqual(column.GetDataType(), copy.GetDataType());
        }
        #endregion



        ///// ArmpTable /////
        #region ArmpTable

        [TestMethod]
        public void ArmpTable_Copy()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            ArmpTable table = armp.GetMainTable().GetEntry("value").GetValueFromColumn<ArmpTable>("table");
            ArmpTable copy = table.Copy(true);
            Assert.AreEqual(table.GetEntry(1).Name, copy.GetEntry(1).Name);
            Assert.AreEqual(table.GetEntry(2).Name, copy.GetEntry(2).Name);
            Assert.AreEqual(table.GetEntry(1).GetValueFromColumn<byte>("u8"), copy.GetEntry(1).GetValueFromColumn<byte>("u8"));
            Assert.AreEqual(table.GetEntry(2).GetValueFromColumn<byte>("u8"), copy.GetEntry(2).GetValueFromColumn<byte>("u8"));
            Assert.AreEqual(table.GetEntry(1).IsValid, copy.GetEntry(1).IsValid);
            Assert.AreEqual(table.GetEntry(2).IsValid, copy.GetEntry(2).IsValid);
        }


        [TestMethod]
        public void ArmpTable_GetAllEntries()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            IReadOnlyList<ArmpEntry> entries = armp.GetMainTable().GetAllEntries();
            Assert.HasCount(4, entries);
            Assert.AreEqual("", entries[0].Name);
            Assert.AreEqual("value", entries[1].Name);
            Assert.AreEqual("min_value", entries[2].Name);
            Assert.AreEqual("max_value", entries[3].Name);
        }


        [TestMethod]
        public void ArmpTable_GetEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            ArmpEntry entry = armp.GetMainTable().GetEntry(1);
            Assert.AreEqual("value", entry.Name);
            entry = armp.GetMainTable().GetEntry("value");
            Assert.AreEqual((uint)1, entry.ID);
        }


        [TestMethod]
        public void ArmpTable_TryGetEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            ArmpEntry resultEntry;
            bool resultBool = armp.GetMainTable().TryGetEntry(1, out resultEntry);
            Assert.IsTrue(resultBool);
            Assert.AreEqual("value", resultEntry.Name);
            resultEntry = null;
            resultBool = armp.GetMainTable().TryGetEntry("value", out resultEntry);
            Assert.IsTrue(resultBool);
            Assert.AreEqual("value", resultEntry.Name);
            resultEntry = null;
            resultBool = armp.GetMainTable().TryGetEntry("does_not_exist", out resultEntry);
            Assert.IsFalse(resultBool);
            Assert.IsNull(resultEntry);
            resultEntry = null;
            resultBool = armp.GetMainTable().TryGetEntry(12345, out resultEntry);
            Assert.IsFalse(resultBool);
            Assert.IsNull(resultEntry);
        }


        [TestMethod]
        public void ArmpTable_GetEntryNames()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            List<string> names = armp.GetMainTable().GetEntryNames();
            Assert.HasCount(4, names);
            Assert.AreEqual("", names[0]);
            Assert.AreEqual("value", names[1]);
            Assert.AreEqual("min_value", names[2]);
            Assert.AreEqual("max_value", names[3]);
        }


        [TestMethod]
        public void ArmpTable_GetEntryName()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            string name = armp.GetMainTable().GetEntryName(1);
            Assert.AreEqual("value", name);
        }


        [TestMethod]
        public void ArmpTable_GetAllColumns()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            IReadOnlyList<ArmpTableColumn> columns = armp.GetMainTable().GetAllColumns();
            Assert.HasCount(70, columns);
            Assert.AreEqual("", columns[0].Name);
            Assert.AreEqual("u8_", columns[1].Name);
            Assert.AreEqual("u16_", columns[2].Name);
            Assert.AreEqual("u32_", columns[3].Name);
            Assert.AreEqual("table", columns[30].Name);
        }


        [TestMethod]
        public void ArmpTable_GetColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            ArmpTableColumn column = armp.GetMainTable().GetColumn(4);
            Assert.AreEqual("u64_", column.Name);
            column = armp.GetMainTable().GetColumn("f32_");
            Assert.AreEqual(9u, column.ID);
        }


        [TestMethod]
        public void ArmpTable_TryGetColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            ArmpTableColumn resultColumn;
            bool resultBool = armp.GetMainTable().TryGetColumn(1, out resultColumn);
            Assert.IsTrue(resultBool);
            Assert.AreEqual("u8_", resultColumn.Name);
            resultColumn = null;
            resultBool = armp.GetMainTable().TryGetColumn("string", out resultColumn);
            Assert.IsTrue(resultBool);
            Assert.AreEqual("string", resultColumn.Name);
            resultColumn = null;
            resultBool = armp.GetMainTable().TryGetColumn("does_not_exist", out resultColumn);
            Assert.IsFalse(resultBool);
            Assert.IsNull(resultColumn);
            resultColumn = null;
            resultBool = armp.GetMainTable().TryGetColumn(12345, out resultColumn);
            Assert.IsFalse(resultBool);
            Assert.IsNull(resultColumn);
        }


        [TestMethod]
        public void ArmpTable_GetColumnNames()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            List<string> names = armp.GetMainTable().GetColumnNames();
            Assert.HasCount(70, names);
            names = armp.GetMainTable().GetColumnNames(false);
            Assert.HasCount(55, names);
        }


        [TestMethod]
        public void ArmpTable_GetColumnName()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            string name = armp.GetMainTable().GetColumnName(4);
            Assert.AreEqual("u64_", name);
        }


        [TestMethod]
        public void ArmpTable_GetColumnDataType()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            Type type = armp.GetMainTable().GetColumnDataType("f64_");
            Assert.AreEqual(typeof(double), type);
        }


        [TestMethod]
        public void ArmpTable_GetColumnsByType()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            List<ArmpTableColumn> columns = armp.GetMainTable().GetColumnsByType(typeof(Int64));
            Assert.HasCount(3, columns);
            columns = armp.GetMainTable().GetColumnsByType<Int64>();
            Assert.HasCount(3, columns);
        }


        [TestMethod]
        public void ArmpTable_GetColumnNamesByType()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            List<string> names = armp.GetMainTable().GetColumnNamesByType(typeof(Int64));
            Assert.HasCount(3, names);
            names = armp.GetMainTable().GetColumnNamesByType<Int64>();
            Assert.HasCount(3, names);
        }


        [TestMethod]
        public void ArmpTable_GetColumnIDsByType()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            List<uint> indices = armp.GetMainTable().GetColumnIDsByType(typeof(Int64));
            Assert.HasCount(3, indices);
            Assert.AreEqual(8u, indices[0]);
            indices = armp.GetMainTable().GetColumnIDsByType<Int64>();
            Assert.HasCount(3, indices);
            Assert.AreEqual(8u, indices[0]);
        }


        [TestMethod]
        public void ArmpTable_GetColumnID()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            uint index = armp.GetMainTable().GetColumnID("s32_");
            Assert.AreEqual(7u, index);
        }


        [TestMethod]
        public void ArmpTable_GetColumnIndex()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            int index = armp.GetMainTable().GetColumnIndex("s32_");
            Assert.AreEqual(8, index);
            index = armp.GetMainTable().GetColumnIndex(7);
            Assert.AreEqual(8, index);
        }


        [TestMethod]
        public void ArmpTable_SetColumnIndex()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().SetColumnIndex("s32_", 123);
            int index = armp.GetMainTable().GetColumnIndex("s32_");
            Assert.AreEqual(123, index);
            armp.GetMainTable().SetColumnIndex(7, 321);
            index = armp.GetMainTable().GetColumnIndex(7);
            Assert.AreEqual(321, index);
        }


        [TestMethod]
        public void ArmpTable_IsColumnValid()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            bool valid = armp.GetMainTable().IsColumnValid("f32_");
            Assert.IsTrue(valid);
            valid = armp.GetMainTable().IsColumnValid(0);
            Assert.IsFalse(valid);
        }


        [TestMethod]
        public void ArmpTable_SetColumnValidity()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().SetColumnValidity("s32_", false);
            ArmpTableColumn column = armp.GetMainTable().GetColumn("string");
            armp.GetMainTable().SetColumnValidity(column, false);
            armp.GetMainTable().SetColumnValidity(column, true);
            armp.GetMainTable().GetEntry(2).SetValueFromColumn("string", "test_string");
            byte[] buffer = ArmpFileWriter.WriteARMPToArray(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(buffer);
            Assert.IsFalse((bool)armp_new.GetMainTable().GetColumn("s32_").IsValid);
            string result = armp.GetMainTable().GetEntry(2).GetValueFromColumn<string>("string");
            string result2 = armp.GetMainTable().GetEntry(1).GetValueFromColumn<string>("string");
            Assert.IsTrue((bool)armp_new.GetMainTable().GetColumn("string").IsValid);
            Assert.AreEqual("test_string", result);
            Assert.AreEqual("", result2);

        }


        [TestMethod]
        public void ArmpTable_AddColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            ArmpTableColumn c1 = armp.GetMainTable().AddColumn<Int64>("test_s64");
            ArmpTableColumn c2 = armp.GetMainTable().AddColumn<bool>("test_bool");
            //Before saving
            Assert.AreEqual(typeof(Int64), armp.GetMainTable().GetColumnDataType(c1.Name));
            Assert.AreEqual(typeof(bool), armp.GetMainTable().GetColumnDataType(c2.Name));
            //After saving
            byte[] temp = ArmpFileWriter.WriteARMPToArray(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(temp);
            Assert.AreEqual(typeof(Int64), armp_new.GetMainTable().GetColumnDataType(c1.Name));
            Assert.AreEqual(typeof(bool), armp_new.GetMainTable().GetColumnDataType(c2.Name));
        }


        [TestMethod]
        public void ArmpTable_DeleteColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            string column = "s32_";
            bool result = armp.GetMainTable().DeleteColumn(column);
            //Before saving
            Assert.IsTrue(result);
            MyAssert.Throws<ColumnNotFoundException>(() => armp.GetMainTable().GetColumn(column));
            //After saving
            byte[] temp = ArmpFileWriter.WriteARMPToArray(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(temp);
            MyAssert.Throws<ColumnNotFoundException>(() => armp_new.GetMainTable().GetColumn(column));
        }


        [TestMethod]
        public void ArmpTable_SearchByName()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            List<ArmpEntry> entries = armp.GetMainTable().SearchByName("value");
            Assert.HasCount(3, entries);
            entries = armp.GetMainTable().SearchByName("min_");
            Assert.HasCount(1, entries);
        }


        [TestMethod]
        public void ArmpTable_SearchByValue()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            List<ArmpEntry> entries = armp.GetMainTable().SearchByValue("s16_", (Int16)32767);
            Assert.HasCount(1, entries);
            entries = armp.GetMainTable().SearchByValue("u64_array[0]", (UInt64)0);
            Assert.HasCount(3, entries);
        }


        [TestMethod]
        public void ArmpTable_AddEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().AddEntry("test_entry");
            byte[] stream = ArmpFileWriter.WriteARMPToArray(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            ArmpEntry entry = armp_new.GetMainTable().GetEntry("test_entry");
            Assert.AreEqual(4U, entry.ID);
            Assert.AreEqual((byte)0, entry.GetValueFromColumn<byte>("u8_"));
            Assert.AreEqual((UInt16)0, entry.GetValueFromColumn<UInt16>("u16_"));
            Assert.AreEqual(0U, entry.GetValueFromColumn<UInt32>("u32_"));
            Assert.AreEqual(0UL, entry.GetValueFromColumn<UInt64>("u64_"));
            Assert.AreEqual(0f, entry.GetValueFromColumn<float>("f32_"));
        }


        [TestMethod]
        public void ArmpTable_InsertEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().InsertEntry(2, "test_entry");
            armp.GetMainTable().InsertEntry(4, "test_entry2");
            armp.GetMainTable().InsertEntry(6, "test_entry3");
            byte[] stream = ArmpFileWriter.WriteARMPToArray(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(2u, armp_new.GetMainTable().GetEntry("test_entry").ID);
            Assert.AreEqual(3u, armp_new.GetMainTable().GetEntry("min_value").ID);
            Assert.AreEqual(4u, armp_new.GetMainTable().GetEntry("test_entry2").ID);
            Assert.AreEqual(5u, armp_new.GetMainTable().GetEntry("max_value").ID);
            Assert.AreEqual(6u, armp_new.GetMainTable().GetEntry("test_entry3").ID);
        }


        [TestMethod]
        public void ArmpTable_DeleteEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().DeleteEntry(0);
            armp.GetMainTable().DeleteEntry("min_value");
            Assert.AreEqual(0u, armp.GetMainTable().GetEntry("value").ID);
            Assert.AreEqual("max_value", armp.GetMainTable().GetEntry(1).Name);
            byte[] stream = ArmpFileWriter.WriteARMPToArray(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual(0u, armp_new.GetMainTable().GetEntry("value").ID);
            Assert.AreEqual("max_value", armp_new.GetMainTable().GetEntry(1).Name);
        }
        #endregion
    }
}
