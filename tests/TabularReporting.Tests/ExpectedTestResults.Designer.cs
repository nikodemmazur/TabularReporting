﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TabularReporting.Tests {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ExpectedTestResults {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExpectedTestResults() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TabularReporting.Tests.ExpectedTestResults", typeof(ExpectedTestResults).Assembly);
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
        ///   Looks up a localized string similar to +------------------------------------+
        ///¦ Report                             ¦
        ///¦    header.                         ¦
        ///¦------------------------------------¦
        ///¦ Date           ¦ 00.00.0000        ¦
        ///¦----------------+-------------------¦
        ///¦ Result         ¦ FAIL              ¦
        ///¦------------------------------------¦
        ///¦ a3x                    ¦ b3x ¦ c2  ¦
        ///¦------------------------+-----+-----¦
        ///¦ d4xx                   ¦ e2  ¦ f2  ¦
        ///¦------------------------+-----+-----¦
        ///¦ g2 ¦ h6xxxx  ¦ i6xxxx  ¦ p   ¦ [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ExpectedAntonioReport {
            get {
                return ResourceManager.GetString("ExpectedAntonioReport", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Column&gt;
        ///  &lt;Row&gt;
        ///    &lt;Column&gt;
        ///      &lt;Row&gt;
        ///        &lt;Column&gt;Test station&lt;/Column&gt;
        ///        &lt;Column&gt;Universal Test Station no 5.&lt;/Column&gt;
        ///      &lt;/Row&gt;
        ///      &lt;Row&gt;
        ///        &lt;Column&gt;Serial number&lt;/Column&gt;
        ///        &lt;Column&gt;XYZ123&lt;/Column&gt;
        ///      &lt;/Row&gt;
        ///    &lt;/Column&gt;
        ///  &lt;/Row&gt;
        ///  &lt;Row&gt;
        ///    &lt;Column&gt;
        ///      &lt;Row&gt;
        ///        &lt;Column&gt;Value&lt;/Column&gt;
        ///        &lt;Column&gt;Unit&lt;/Column&gt;
        ///      &lt;/Row&gt;
        ///      &lt;Row&gt;
        ///        &lt;Column&gt;False&lt;/Column&gt;
        ///        &lt;Column&gt;1&lt;/Column&gt;
        ///      &lt;/Row&gt;
        ///      &lt;Row&gt;
        ///        &lt;Column&gt;7&lt;/C [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ExpectedReportAsXml {
            get {
                return ResourceManager.GetString("ExpectedReportAsXml", resourceCulture);
            }
        }
    }
}
