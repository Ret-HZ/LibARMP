using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using CsvHelper;
using LibARMP;
using LibARMP.IO;

namespace armp.tool
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine($"[WARNING] No file to process");
                return;
            }

            foreach (string path in args)
                ProcessFile(path);
        }

        internal static void ProcessFile(string path)
        {
            switch(Path.GetExtension(path).ToLower())
            {
                case ".bin":
                    ProcessArmpFile(path);
                    break;
                case ".csv":
                    ProcessCsvFile(path);
                    break;
                default:
                    Console.WriteLine("[ERROR] Ignoring unknown file type: {}");
                    break;
            }
        }

        internal static void ProcessArmpFile(string path)
        {
            using (TextWriter tr = new StreamWriter(Path.ChangeExtension(path, ".csv"), false))
            {
                using (CsvWriter cw = new CsvWriter(tr, CultureInfo.InvariantCulture))
                {
                    ARMP data = ArmpFileReader.ReadARMP(path);
                    ArmpTable tbl = data.GetMainTable();

                    // Filter to keep only valid columns and entries
                    IEnumerable<ArmpTableColumn> columns = tbl.GetAllColumns().Where(x => x.IsValid);
                    IEnumerable<ArmpEntry> entries = tbl.GetAllEntries().Where(x => x.IsValid);

                    // Write column headers row
                    cw.WriteField("ROW-ID");
                    cw.WriteField("ROW-NAME");
                    foreach (ArmpTableColumn col in columns)
                    {
                        cw.WriteField(col.Name);
                    }
                    cw.NextRecord();

                    // Write data
                    foreach (ArmpEntry e in entries)
                    {
                        cw.WriteField(e.ID);
                        cw.WriteField(e.Name);
                        foreach (ArmpTableColumn col in columns)
                        {
                            object val = null;
                            if (e.ColumnHasData(col.Name))
                                val = e.GetValueFromColumn(col);

                            cw.WriteField(val);
                        }
                        cw.NextRecord();
                    }
                }
            }
        }

        delegate ArmpEntry GetEntryFunc(ArmpTable tbl, CsvReader cr);
        internal static void ProcessCsvFile(string path)
        {
            string outpath = Path.ChangeExtension(path, ".bin");
            ARMP data = ArmpFileReader.ReadARMP(outpath);
            ArmpTable tbl = data.GetMainTable();
            using (TextReader tr = new StreamReader(path, false))
            {
                using (CsvReader cr = new CsvReader(tr, CultureInfo.InvariantCulture))
                {
                    // Read header row
                    cr.Read();
                    cr.ReadHeader();
                    List<string> columns = new List<string>(cr.HeaderRecord);

                    GetEntryFunc getEntry = null;
                    if (columns.Contains("ROW-NAME"))
                    {
                        columns.Remove("ROW-NAME");
                        getEntry = (ArmpTable tbl, CsvReader cr) => { return tbl.GetEntry(cr["ROW-NAME"]); };
                    }

                    if (columns.Contains("ROW-ID"))
                    {
                        columns.Remove("ROW-ID");
                        getEntry = (ArmpTable tbl, CsvReader cr) => { return tbl.GetEntry(uint.Parse(cr["ROW-ID"])); };
                    }

                    IEnumerable<ArmpTableColumn> cols = tbl.GetAllColumns().Where(x => columns.Contains(x.Name));

                    while (cr.Read())
                    {
                        ArmpEntry e = getEntry(tbl, cr);
                        foreach (ArmpTableColumn c in cols)
                        {
                            e.SetValueFromColumn(c, ConvertForColumn(c, cr[c.Name]));
                        }
                    }
                }
            }
            ArmpFileWriter.WriteARMPToFile(data, outpath);
        }

        internal static object ConvertForColumn(ArmpTableColumn c, string value)
        {
            Type dst = c.GetDataType();

            if (String.CompareOrdinal(dst.FullName, typeof(String).FullName) == 0) return value;
            if (String.CompareOrdinal(dst.FullName, typeof(SByte).FullName) == 0) return SByte.Parse(value);
            if (String.CompareOrdinal(dst.FullName, typeof(Int16).FullName) == 0) return Int16.Parse(value);
            if (String.CompareOrdinal(dst.FullName, typeof(Int32).FullName) == 0) return Int32.Parse(value);
            if (String.CompareOrdinal(dst.FullName, typeof(Int64).FullName) == 0) return Int64.Parse(value);
            if (String.CompareOrdinal(dst.FullName, typeof(Byte).FullName) == 0) return Byte.Parse(value);
            if (String.CompareOrdinal(dst.FullName, typeof(UInt16).FullName) == 0) return UInt16.Parse(value);
            if (String.CompareOrdinal(dst.FullName, typeof(UInt32).FullName) == 0) return UInt32.Parse(value);
            if (String.CompareOrdinal(dst.FullName, typeof(UInt64).FullName) == 0) return UInt64.Parse(value);
            if (String.CompareOrdinal(dst.FullName, typeof(Single).FullName) == 0) return Single.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
            if (String.CompareOrdinal(dst.FullName, typeof(Double).FullName) == 0) return Double.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
            if (String.CompareOrdinal(dst.FullName, typeof(Decimal).FullName) == 0) return Decimal.Parse(value, CultureInfo.InvariantCulture.NumberFormat);

            throw new InvalidCastException($"Unsupported column type '{dst.FullName}' for column '{c.Name}'");
        }
    }
}
