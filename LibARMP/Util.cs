using BinaryExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace LibARMP
{
    internal static class Util
    {
        /// <summary>
        /// Stores a determined amount of offsets into a list.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/>.</param>
        /// <param name="ptrOffsetList">Pointer to the Offset List.</param>
        /// <param name="amount">Amount of offsets to store.</param>
        /// <param name="isBigEndian"><see langword="true"/> to read as Big Endian, <see langword="false"/> for Little Endian.</param>
        /// <returns>A <see cref="uint"/> list.</returns>
        internal static List<UInt32> IterateOffsetList (BinaryReader reader, UInt32 ptrOffsetList, int amount, bool isBigEndian)
        {
            List<UInt32> offsetList = new List<UInt32>();
            reader.BaseStream.Seek(ptrOffsetList);

            for (int i=0; i < amount; i++)
            {
                offsetList.Add(reader.ReadUInt32(isBigEndian));
            }
            return offsetList;
        }


        /// <summary>
        /// Reads strings based on an offset list.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/>.</param>
        /// <param name="offsetList">The String Offset List.</param>
        /// <returns>A <see cref="string"/> list.</returns>
        internal static List<string> IterateStringList (BinaryReader reader, List<UInt32> offsetList)
        {
            List<string> stringList = new List<string>();

            foreach (uint offset in offsetList) 
            {
                reader.BaseStream.Seek(offset);
                stringList.Add(reader.ReadStringNullTerminated());
            }
            return stringList;
        }


        /// <summary>
        /// Reads a bitmask.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/>.</param>
        /// <param name="ptrBitmask">The pointer to the bitmask.</param>
        /// <param name="bitCount">The amount of values in the bitmask.</param>
        /// <returns>A <see cref="Boolean"/> list.</returns>
        internal static List<bool> IterateBooleanBitmask (BinaryReader reader, UInt32 ptrBitmask, int bitCount, bool isBigEndian)
        {
            List<bool> boolList = new List<bool>(bitCount);

            reader.BaseStream.Seek(ptrBitmask);

            for (int i = 0; i < bitCount; i += 32)
            {
                int bitmask = reader.ReadInt32(isBigEndian);
                
                for (int j = 0; j < 32 && boolList.Count < bitCount; j++)
                {
                    boolList.Add((bitmask & (1 << j)) != 0);
                }
            }

            return boolList;
        }


        /// <summary>
        /// Reverses a <see cref="string"/>.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to reverse.</param>
        /// <returns>A reversed <see cref="string"/>.</returns>
        internal static string ReverseString (string str)
        {
            char[] charArray = str.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }


        /// <summary>
        /// Reads an array of type T.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/>.</param>
        /// <param name="ptrArray">The pointer to the array.</param>
        /// <param name="amount">The amount of values in the array.</param>
        /// <param name="isBigEndian"><see langword="true"/> to read as Big Endian, <see langword="false"/> for Little Endian.</param>
        /// <returns>A list of type T.</returns>
        internal static dynamic IterateArray<T> (BinaryReader reader, UInt32 ptrArray, int amount, bool isBigEndian)
        {
            List<T> returnList = new List<T>();
            reader.BaseStream.Seek(ptrArray);

            for (int i=0; i<amount; i++)
            {
                T val = (T)reader.Read<T>(isBigEndian);
                returnList.Add(val);
            } 

            return returnList;
        }


        /// <summary>
        /// Writes a <see cref="Boolean"/> list to a stream.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/>.</param>
        /// <param name="boolList">The <see cref="Boolean"/> list.</param>
        internal static void WriteBooleanBitmask (BinaryWriter writer, List<bool> boolList, bool isBigEndian)
        {
            int index = 0;

            while (index < boolList.Count)
            {
                int bitmask = 0;

                for (int i = 0; i < 32 && index < boolList.Count; i++)
                {
                    if (boolList[index])
                    {
                        bitmask |= (1 << i);
                    }
                    index++;
                }

                writer.Write(bitmask, isBigEndian);
            }
        }


        /// <summary>
        /// Writes a text list and its offset table to the stream.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/>.</param>
        /// <param name="textList">The text list.</param>
        /// <param name="isBigEndian"><see langword="true"/> to write as Big Endian, <see langword="false"/> for Little Endian.</param>
        /// <returns>The pointer to the text offset table.</returns>
        internal static int WriteText (BinaryWriter writer, List<string> textList, bool isBigEndian)
        {
            List<int> ptrList = new List<int>();
            
            foreach (string text in textList)
            {
                ptrList.Add((int)writer.BaseStream.Position);
                writer.Write(text, true);
            }
            writer.WritePadding(0, 0x10);
            int ptrOffsetTable = (int)writer.BaseStream.Position;

            foreach(int pointer in ptrList)
            {
                writer.Write(pointer, isBigEndian);
            }

            return ptrOffsetTable;
        }


        /// <summary>
        /// Counts the amount of occurences of a substring in a <see cref="string"/> list.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to look for.</param>
        /// <param name="strList">The <see cref="string"/> list to look for occurrences in.</param>
        /// <returns>The amount of occurrences.</returns>
        internal static int CountStringOccurrences(string str, List<string> strList)
        {
            int count = 0;

            foreach(string str2 in strList)
            {
                if (str2.Contains(str)) count++;
            }

            return count;
        }


        /// <summary>
        /// Deep copy an object.
        /// </summary>
        internal static T DeepCopy<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
#pragma warning disable SYSLIB0011
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
#pragma warning restore SYSLIB0011
            }
        }



        ///// EXTENSIONS /////

        /// <summary>
        /// Attempts to add the specified key and value to the dictionary.
        /// </summary>
        /// <param name="dictionary">The Dictionary.</param>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. It can be <c>null</c>.</param>
        /// <returns><c>true</c> if the key/value pair was added to the dictionary successfully; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <c>null</c>.</exception>
        internal static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                return true;
            }

            return false;
        }
    }
}
