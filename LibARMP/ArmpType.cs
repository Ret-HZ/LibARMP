﻿using System;

namespace LibARMP
{
    internal class ArmpType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArmpType"/> class.
        /// </summary>
        internal ArmpType() { }

        /// <summary>
        /// C# equivalent of the type.
        /// </summary>
        internal Type CSType { get; set; }

        /// <summary>
        /// ID for Dragon Engine v1
        /// </summary>
        internal sbyte IDv1 { get; set; }

        /// <summary>
        /// Aux ID for Dragon Engine v1
        /// </summary>
        internal sbyte IDAuxv1 { get; set; }

        /// <summary>
        /// ID for Dragon Engine v2
        /// </summary>
        internal sbyte IDv2 { get; set; }

        /// <summary>
        /// Aux ID for Dragon Engine v2
        /// </summary>
        internal sbyte IDAuxv2 { get; set; }

        /// <summary>
        /// ID for Old Engine
        /// </summary>
        internal sbyte IDOE { get; set; }

        /// <summary>
        /// Default value for the type
        /// </summary>
        internal object DefaultValue { get; set; }

        /// <summary>
        /// Size of the type
        /// </summary>
        internal sbyte Size { get; set; }


        
        internal sbyte GetID (Version version)
        {
            switch (version)
            {
                case Version.DragonEngineV1: return IDv1;
                case Version.DragonEngineV2: return IDv2;
                case Version.OldEngine: return IDOE;
                case Version.OldEngineIshin: return IDOE;
                default: return -1;
            }
        }


        internal sbyte GetIDAux (Version version)
        {
            switch (version)
            {
                case Version.DragonEngineV1: return IDAuxv1;
                case Version.DragonEngineV2: return IDAuxv2;
                default: return -1;
            }
        }


        internal sbyte GetID (Version version, bool isAux)
        {
            if (isAux) return GetIDAux(version);
            else return GetID(version);
        }
    }
}
