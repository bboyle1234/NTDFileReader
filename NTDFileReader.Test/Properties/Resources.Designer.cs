﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NTDFileReader.Test.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NTDFileReader.Test.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] NQTicksInput {
            get {
                object obj = ResourceManager.GetObject("NQTicksInput", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 20191002 130000 0150000;7640;7639.75;7640;1
        ///20191002 130000 4960000;7640.25;7639.75;7640.25;1
        ///20191002 130000 5190000;7640.25;7640;7640.25;1
        ///20191002 130000 6480000;7640.25;7640;7640.25;1
        ///20191002 130001 5000000;7640;7640;7640.5;1
        ///20191002 130001 6640000;7640.5;7640;7640.5;1
        ///20191002 130001 6640000;7640.5;7640;7640.5;1
        ///20191002 130002 6170000;7640.5;7640.25;7640.5;1
        ///20191002 130002 6170000;7640.5;7640.25;7640.5;1
        ///20191002 130003 5350000;7640.25;7640.25;7640.75;4
        ///20191002 130003 5930000;7640.25;764 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string NQTicksOutput {
            get {
                return ResourceManager.GetString("NQTicksOutput", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] ntdInput {
            get {
                object obj = ResourceManager.GetObject("ntdInput", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 2015-08-20 12:00:00	41.43	1
        ///2015-08-20 12:00:00	41.44	1
        ///2015-08-20 12:00:00	41.44	1
        ///2015-08-20 12:00:00	41.44	1
        ///2015-08-20 12:00:00	41.44	1
        ///2015-08-20 12:00:00	41.45	1
        ///2015-08-20 12:00:00	41.44	1
        ///2015-08-20 12:00:00	41.44	1
        ///2015-08-20 12:00:00	41.45	1
        ///2015-08-20 12:00:00	41.45	1
        ///2015-08-20 12:00:01	41.44	1
        ///2015-08-20 12:00:01	41.44	2
        ///2015-08-20 12:00:02	41.45	1
        ///2015-08-20 12:00:02	41.45	1
        ///2015-08-20 12:00:02	41.45	2
        ///2015-08-20 12:00:02	41.45	1
        ///2015-08-20 12:00:02	41.45	1
        ///2015-08-20 12:00:02 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ntdOutput {
            get {
                return ResourceManager.GetString("ntdOutput", resourceCulture);
            }
        }
    }
}
