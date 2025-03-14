﻿using System;
using System.IO;

namespace LibARMP
{
    public class ARMP
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ARMP"/> class.
        /// </summary>
        internal ARMP ()
        {

        }


        /// <summary>
        /// Gets the version number.
        /// </summary>
        public Int16 Version { get; internal set; }

        /// <summary>
        /// Gets the revision number.
        /// </summary>
        public Int16 Revision { get; internal set; }

        /// <summary>
        /// Which specific version it is using.
        /// </summary>
        /// <remarks><para>Version and Revision numbers are shared between multiple different format versions.</para></remarks>
        public Version FormatVersion { get; internal set; }

        /// <summary>
        /// The <see cref="ARMP"/> file's main table.
        /// </summary>
        internal ArmpTableMain MainTable { get; set; }

        /// <summary>
        /// Original armp file for patching.
        /// </summary>
        internal MemoryStream File = new MemoryStream();



        /// <summary>
        /// Gets this <see cref="ARMP"/> file's main table.
        /// </summary>
        /// <returns>An <see cref="ArmpTableMain"/> object.</returns>
        public ArmpTableMain GetMainTable()
        {
            return MainTable;
        }
    }
}
