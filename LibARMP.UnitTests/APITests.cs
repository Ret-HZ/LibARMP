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
            Assert.AreEqual((uint)1, id);
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
        public void ArmpEntry_SetIndex()
        {
            ARMP armp_v1 = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            ARMP armp_v2Column = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            ARMP armp_v2Structured = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);

            armp_v1.GetMainTable().GetEntry(1).SetIndex(2);
            armp_v2Column.GetMainTable().GetEntry(1).SetIndex(2);
            armp_v2Structured.GetMainTable().GetEntry(1).SetIndex(2);

            armp_v1 = ArmpFileReader.ReadARMP(ArmpFileWriter.WriteARMPToArray(armp_v1));
            armp_v2Column = ArmpFileReader.ReadARMP(ArmpFileWriter.WriteARMPToArray(armp_v2Column));
            armp_v2Structured = ArmpFileReader.ReadARMP(ArmpFileWriter.WriteARMPToArray(armp_v2Structured));

            Assert.AreEqual((uint)2, armp_v1.GetMainTable().GetEntry(1).Index);
            Assert.AreEqual((uint)2, armp_v2Column.GetMainTable().GetEntry(1).Index);
            Assert.AreEqual((uint)2, armp_v2Structured.GetMainTable().GetEntry(1).Index);
        }


        [TestMethod]
        public void ArmpEntry_TrySetIndex()
        {
            ARMP armp_v1 = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            ARMP armp_v2Column = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            ARMP armp_v2Structured = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);

            armp_v1.GetMainTable().GetEntry(1).TrySetIndex(2);
            armp_v2Column.GetMainTable().GetEntry(1).TrySetIndex(2);
            armp_v2Structured.GetMainTable().GetEntry(1).TrySetIndex(2);
            bool result = armp_v1.GetMainTable().GetEntry(0).TrySetIndex(9999);

            armp_v1 = ArmpFileReader.ReadARMP(ArmpFileWriter.WriteARMPToArray(armp_v1));
            armp_v2Column = ArmpFileReader.ReadARMP(ArmpFileWriter.WriteARMPToArray(armp_v2Column));
            armp_v2Structured = ArmpFileReader.ReadARMP(ArmpFileWriter.WriteARMPToArray(armp_v2Structured));

            Assert.AreEqual((uint)2, armp_v1.GetMainTable().GetEntry(1).Index);
            Assert.AreEqual((uint)2, armp_v2Column.GetMainTable().GetEntry(1).Index);
            Assert.AreEqual((uint)2, armp_v2Structured.GetMainTable().GetEntry(1).Index);
            Assert.IsFalse(result);
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
        public void ArmpEntry_TryGetValueFromColumn()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);
            ArmpEntry entry = armp.GetMainTable().GetEntry("value");
            ArmpTableColumn column = armp.GetMainTable().GetColumn("u16_");
            ushort value1, value2, value3;
            entry.TryGetValueFromColumn("u16_", out value1);
            entry.TryGetValueFromColumn(2, out value2);
            entry.TryGetValueFromColumn(column, out value3);

            Assert.AreEqual((UInt16)800, value1);
            Assert.AreEqual((UInt16)800, value2);
            Assert.AreEqual((UInt16)800, value3);

            ArmpTable table;
            byte value_u8;
            entry.TryGetValueFromColumn("table", out table);
            Assert.AreEqual("value", table.GetEntry(2).Name);
            table.GetEntry(2).TryGetValueFromColumn("u8", out value_u8);
            Assert.AreEqual((byte)64, value_u8);

            int value_s32;
            bool result_nonexistentColumn = entry.TryGetValueFromColumn("does_not_exist", out value_s32);
            Assert.IsFalse(result_nonexistentColumn);
            Assert.AreEqual((int)0, value_s32);

            long value_s64;
            bool result_invalidValueType = entry.TryGetValueFromColumn("string", out value_s64);
            Assert.IsFalse(result_invalidValueType);
            Assert.AreEqual((long)0, value_s64);
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
            Assert.AreEqual((uint)3, id);
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
            uint index = armp.GetMainTable().GetColumn("u32_").Index;
            Assert.AreEqual((uint)2, index);
        }


        [TestMethod]
        public void ArmpTableColumn_IsValid()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            bool valid = (bool)armp.GetMainTable().GetColumn("u32_").IsValid;
            Assert.AreEqual(true, valid);
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


        [TestMethod]
        public void ArmpTableColumn_SetIndex()
        {
            ARMP armp_v1 = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            ARMP armp_v2Column = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            ARMP armp_v2Structured = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);

            armp_v1.GetMainTable().GetColumn("s16").SetIndex(2);
            armp_v2Column.GetMainTable().GetColumn("s16_").SetIndex(2);
            armp_v2Structured.GetMainTable().GetColumn("s16_").SetIndex(2);

            armp_v1 = ArmpFileReader.ReadARMP(ArmpFileWriter.WriteARMPToArray(armp_v1));
            armp_v2Column = ArmpFileReader.ReadARMP(ArmpFileWriter.WriteARMPToArray(armp_v2Column));
            armp_v2Structured = ArmpFileReader.ReadARMP(ArmpFileWriter.WriteARMPToArray(armp_v2Structured));

            Assert.AreEqual((uint)2, armp_v1.GetMainTable().GetColumn("s16").Index);
            Assert.AreEqual((uint)2, armp_v2Column.GetMainTable().GetColumn("s16_").Index);
            Assert.AreEqual((uint)2, armp_v2Structured.GetMainTable().GetColumn("s16_").Index);
        }


        [TestMethod]
        public void ArmpTableColumn_TrySetIndex()
        {
            ARMP armp_v1 = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            ARMP armp_v2Column = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            ARMP armp_v2Structured = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);

            armp_v1.GetMainTable().GetColumn("s16").TrySetIndex(2);
            armp_v2Column.GetMainTable().GetColumn("s16_").TrySetIndex(2);
            armp_v2Structured.GetMainTable().GetColumn("s16_").TrySetIndex(2);
            bool result = armp_v1.GetMainTable().GetColumn(0).TrySetIndex(9999);

            armp_v1 = ArmpFileReader.ReadARMP(ArmpFileWriter.WriteARMPToArray(armp_v1));
            armp_v2Column = ArmpFileReader.ReadARMP(ArmpFileWriter.WriteARMPToArray(armp_v2Column));
            armp_v2Structured = ArmpFileReader.ReadARMP(ArmpFileWriter.WriteARMPToArray(armp_v2Structured));

            Assert.AreEqual((uint)2, armp_v1.GetMainTable().GetColumn("s16").Index);
            Assert.AreEqual((uint)2, armp_v2Column.GetMainTable().GetColumn("s16_").Index);
            Assert.AreEqual((uint)2, armp_v2Structured.GetMainTable().GetColumn("s16_").Index);
            Assert.IsFalse(result);
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
            armp.GetMainTable().AddEntry("new");
            armp.GetMainTable().GetEntry("new").SetValueFromColumn("table", copy);

            Assert.AreEqual(table.GetEntry(1).Name, copy.GetEntry(1).Name);
            Assert.AreEqual(table.GetEntry(2).Name, copy.GetEntry(2).Name);
            Assert.AreEqual(table.GetEntry(1).GetValueFromColumn<byte>("u8"), copy.GetEntry(1).GetValueFromColumn<byte>("u8"));
            Assert.AreEqual(table.GetEntry(2).GetValueFromColumn<byte>("u8"), copy.GetEntry(2).GetValueFromColumn<byte>("u8"));
            Assert.AreEqual(table.GetEntry(1).IsValid, copy.GetEntry(1).IsValid);
            Assert.AreEqual(table.GetEntry(2).IsValid, copy.GetEntry(2).IsValid);

            ARMP armp_saved = ArmpFileReader.ReadARMP(ArmpFileWriter.WriteARMPToArray(armp));
            table = armp_saved.GetMainTable().GetEntry("value").GetValueFromColumn<ArmpTable>("table");
            copy = armp_saved.GetMainTable().GetEntry("new").GetValueFromColumn<ArmpTable>("table");
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
            Assert.AreEqual(4, entries.Count);
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
            Assert.AreEqual(null, resultEntry);
            resultEntry = null;
            resultBool = armp.GetMainTable().TryGetEntry(12345, out resultEntry);
            Assert.IsFalse(resultBool);
            Assert.AreEqual(null, resultEntry);
        }


        [TestMethod]
        public void ArmpTable_GetEntryNames()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            List<string> names = armp.GetMainTable().GetEntryNames();
            Assert.AreEqual(4, names.Count);
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
            Assert.AreEqual(70, columns.Count);
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
            Assert.AreEqual((uint)9, column.ID);
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
            Assert.AreEqual(null, resultColumn);
            resultColumn = null;
            resultBool = armp.GetMainTable().TryGetColumn(12345, out resultColumn);
            Assert.IsFalse(resultBool);
            Assert.AreEqual(null, resultColumn);
        }


        [TestMethod]
        public void ArmpTable_GetColumnNames()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            List<string> names = armp.GetMainTable().GetColumnNames();
            Assert.AreEqual(70, names.Count);
            names = armp.GetMainTable().GetColumnNames(false);
            Assert.AreEqual(55, names.Count);
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
            Assert.AreEqual(3, columns.Count);
            columns = armp.GetMainTable().GetColumnsByType<Int64>();
            Assert.AreEqual(3, columns.Count);
        }


        [TestMethod]
        public void ArmpTable_GetOrderedColumnsByType()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            List<ArmpTableColumn> columns = armp.GetMainTable().GetOrderedColumnsByType(typeof(Int64));
            Assert.AreEqual(3, columns.Count);
            Assert.AreEqual((uint)7, columns[0].Index);
            columns = armp.GetMainTable().GetColumnsByType<Int64>();
            Assert.AreEqual(3, columns.Count);
            Assert.AreEqual((uint)51, columns[1].Index);
        }


        [TestMethod]
        public void ArmpTable_GetColumnNamesByType()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            List<string> names = armp.GetMainTable().GetColumnNamesByType(typeof(Int64));
            Assert.AreEqual(3, names.Count);
            names = armp.GetMainTable().GetColumnNamesByType<Int64>();
            Assert.AreEqual(3, names.Count);
        }


        [TestMethod]
        public void ArmpTable_GetColumnIDsByType()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            List<uint> indices = armp.GetMainTable().GetColumnIDsByType(typeof(Int64));
            Assert.AreEqual(3, indices.Count);
            Assert.AreEqual((uint)8, indices[0]);
            indices = armp.GetMainTable().GetColumnIDsByType<Int64>();
            Assert.AreEqual(3, indices.Count);
            Assert.AreEqual((uint)8, indices[0]);
        }


        [TestMethod]
        public void ArmpTable_GetColumnID()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            int index = (int)armp.GetMainTable().GetColumnID("s32_");
            Assert.AreEqual(7, index);
        }


        [TestMethod]
        public void ArmpTable_GetColumnIndex()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            uint index = armp.GetMainTable().GetColumnIndex("s32_");
            Assert.AreEqual((uint)6, index);
            index = armp.GetMainTable().GetColumnIndex(7);
            Assert.AreEqual((uint)6, index);
        }


        [TestMethod]
        public void ArmpTable_SetColumnIndex()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().SetColumnIndex("s32_", 30);
            uint index = armp.GetMainTable().GetColumnIndex("s32_");
            Assert.AreEqual((uint)30, index);
            armp.GetMainTable().SetColumnIndex(7, 45);
            index = armp.GetMainTable().GetColumnIndex(7);
            Assert.AreEqual((uint)45, index);
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
        public void ArmpTable_IsColumnArray()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            bool special = armp.GetMainTable().IsColumnArray("vf128_");
            Assert.IsTrue(special);
            special = armp.GetMainTable().IsColumnArray("f32_");
            Assert.IsFalse(special);
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
            Assert.Throws<ColumnNotFoundException>(() => armp.GetMainTable().GetColumn(column));
            //After saving
            byte[] temp = ArmpFileWriter.WriteARMPToArray(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(temp);
            Assert.Throws<ColumnNotFoundException>(() => armp_new.GetMainTable().GetColumn(column));
        }


        [TestMethod]
        public void ArmpTable_SearchByName()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            List<ArmpEntry> entries = armp.GetMainTable().SearchByName("value");
            Assert.AreEqual(3, entries.Count);
            entries = armp.GetMainTable().SearchByName("min_");
            Assert.AreEqual(1, entries.Count);
        }


        [TestMethod]
        public void ArmpTable_SearchByValue()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            List<ArmpEntry> entries = armp.GetMainTable().SearchByValue("s16_", (Int16)32767);
            Assert.AreEqual(1, entries.Count);
            entries = armp.GetMainTable().SearchByValue("u64_array[0]", (UInt64)0);
            Assert.AreEqual(3, entries.Count);
        }


        [TestMethod]
        public void ArmpTable_AddEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().AddEntry("test_entry");
            byte[] stream = ArmpFileWriter.WriteARMPToArray(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            ArmpEntry entry = armp_new.GetMainTable().GetEntry("test_entry");
            Assert.AreEqual((uint)4, entry.ID);
            Assert.AreEqual((byte)0, entry.GetValueFromColumn<byte>("u8_"));
            Assert.AreEqual((UInt16)0, entry.GetValueFromColumn<UInt16>("u16_"));
            Assert.AreEqual((UInt32)0, entry.GetValueFromColumn<UInt32>("u32_"));
            Assert.AreEqual((UInt64)0, entry.GetValueFromColumn<UInt64>("u64_"));
            Assert.AreEqual((float)0, entry.GetValueFromColumn<float>("f32_"));
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
            Assert.AreEqual((uint)2, armp_new.GetMainTable().GetEntry("test_entry").ID);
            Assert.AreEqual((uint)3, armp_new.GetMainTable().GetEntry("min_value").ID);
            Assert.AreEqual((uint)4, armp_new.GetMainTable().GetEntry("test_entry2").ID);
            Assert.AreEqual((uint)5, armp_new.GetMainTable().GetEntry("max_value").ID);
            Assert.AreEqual((uint)6, armp_new.GetMainTable().GetEntry("test_entry3").ID);
        }


        [TestMethod]
        public void ArmpTable_DeleteEntry()
        {
            ARMP armp = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            armp.GetMainTable().DeleteEntry(0);
            armp.GetMainTable().DeleteEntry("min_value");
            Assert.AreEqual((uint)0, armp.GetMainTable().GetEntry("value").ID);
            Assert.AreEqual("max_value", armp.GetMainTable().GetEntry(1).Name);
            byte[] stream = ArmpFileWriter.WriteARMPToArray(armp);
            ARMP armp_new = ArmpFileReader.ReadARMP(stream);
            Assert.AreEqual((uint)0, armp_new.GetMainTable().GetEntry("value").ID);
            Assert.AreEqual("max_value", armp_new.GetMainTable().GetEntry(1).Name);
        }


        [TestMethod]
        public void ArmpTable_SetValue()
        {
            ARMP armp_v1 = ArmpFileReader.ReadARMP(TestFiles.v1AllTypes);
            ARMP armp_v2Column = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            ARMP armp_v2Structured = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);

            armp_v1.GetMainTable().SetValue(1, "s16", 1234);
            armp_v2Column.GetMainTable().SetValue(1, "s16_", 4321);
            armp_v2Structured.GetMainTable().SetValue(1, "s16_", 2143);

            armp_v1 = ArmpFileReader.ReadARMP(ArmpFileWriter.WriteARMPToArray(armp_v1));
            armp_v2Column = ArmpFileReader.ReadARMP(ArmpFileWriter.WriteARMPToArray(armp_v2Column));
            armp_v2Structured = ArmpFileReader.ReadARMP(ArmpFileWriter.WriteARMPToArray(armp_v2Structured));

            Assert.AreEqual((short)1234, armp_v1.GetMainTable().GetEntry(1).GetValueFromColumn("s16"));
            Assert.AreEqual((short)4321, armp_v2Column.GetMainTable().GetEntry(1).GetValueFromColumn("s16_"));
            Assert.AreEqual((short)2143, armp_v2Structured.GetMainTable().GetEntry(1).GetValueFromColumn("s16_"));
        }


        [TestMethod]
        public void ArmpTable_SetStorageMode()
        {
            ARMP armp_v2Column = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeColumn);
            ARMP armp_v2Structured = ArmpFileReader.ReadARMP(TestFiles.v2AllTypesModeStructured);

            armp_v2Column.GetMainTable().SetStorageMode(StorageMode.Structured);
            armp_v2Structured.GetMainTable().SetStorageMode(StorageMode.Column);

            armp_v2Column = ArmpFileReader.ReadARMP(ArmpFileWriter.WriteARMPToArray(armp_v2Structured));
            armp_v2Structured = ArmpFileReader.ReadARMP(ArmpFileWriter.WriteARMPToArray(armp_v2Column));

            Assert.AreEqual(StorageMode.Column, armp_v2Column.GetMainTable().TableInfo.StorageMode);
            Assert.AreEqual(StorageMode.Structured, armp_v2Structured.GetMainTable().TableInfo.StorageMode);
        }
        #endregion
    }
}
