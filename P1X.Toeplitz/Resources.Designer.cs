﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace P1X.Toeplitz {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("P1X.Toeplitz.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Normalized matrix should contains 1 at main diagonal (at {0} index of the array)..
        /// </summary>
        internal static string NormalizedToeplitzMatrix_ArrayDataNotNormilized {
            get {
                return ResourceManager.GetString("NormalizedToeplitzMatrix_ArrayDataNotNormilized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can&apos;t set main diagonal values for the normalized matrix..
        /// </summary>
        internal static string NormalizedToeplitzMatrix_CantSetMainDiagonal {
            get {
                return ResourceManager.GetString("NormalizedToeplitzMatrix_CantSetMainDiagonal", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The index should be between -N and N, where N is the size of the matrix..
        /// </summary>
        internal static string NormalizedToeplitzMatrix_IndexOutOfRange {
            get {
                return ResourceManager.GetString("NormalizedToeplitzMatrix_IndexOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Array length should be odd..
        /// </summary>
        internal static string NormalizedToeplitzMatrix_InvalidArrayLength {
            get {
                return ResourceManager.GetString("NormalizedToeplitzMatrix_InvalidArrayLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Size should be grater then 1..
        /// </summary>
        internal static string NormalizedToeplitzMatrix_InvalidSize {
            get {
                return ResourceManager.GetString("NormalizedToeplitzMatrix_InvalidSize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Matrix size is insufficient for current iteration..
        /// </summary>
        internal static string Solver_InsufficientMatrixSize {
            get {
                return ResourceManager.GetString("Solver_InsufficientMatrixSize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Vector size is insufficient for current iteration..
        /// </summary>
        internal static string Solver_InsufficientVectorSize {
            get {
                return ResourceManager.GetString("Solver_InsufficientVectorSize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Vectors and the matrix should be the same size..
        /// </summary>
        internal static string Solver_InvalidVectorSize {
            get {
                return ResourceManager.GetString("Solver_InvalidVectorSize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The matrix should be initialized (non-default)..
        /// </summary>
        internal static string Solver_MatrixNotInitialized {
            get {
                return ResourceManager.GetString("Solver_MatrixNotInitialized", resourceCulture);
            }
        }
    }
}
