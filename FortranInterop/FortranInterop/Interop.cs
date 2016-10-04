using System;
using System.Text;
using System.Runtime.InteropServices;

namespace FortranInterop
{
    public class Interop
    {
        public Interop()
        {
        }

        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "__interop_MOD_copy_integer")]
        public static extern void CopyInteger(ref int a, out int b);

        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "copy_integer_c")]
        public static extern void CopyIntegerIsoC(ref int a, out int b);

        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "__interop_MOD_return_double")]
        public static extern double ReturnDouble(ref double input);

        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "return_double_c")]
        public static extern double ReturnDoubleIsoC(ref double input);

        /// <summary>
        /// Returns the passed integer value.
        /// </summary>
        /// <remarks>
        /// External function definition for the simple return_integer Fortran function.
        /// Using CDECL calling convention and the gFortran mangled function name.
        /// </remarks>
        /// <param name="input">Integer value to return.</param>
        /// <returns>Returns the input value.</returns>
        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "__interop_MOD_return_integer")]
        public static extern int ReturnInteger(ref int input);

        /// <summary>
        /// Returns the passed integer value.
        /// </summary>
        /// <remarks>
        /// External function definition for the simple return_integer_c Fortran function.
        /// Using CDECL calling convention and the function name as specified using the ISO C Binding module.
        /// </remarks>
        /// <param name="input">Integer value to return.</param>
        /// <returns>Returns the input value.</returns>
        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "return_integer_c")]
        public static extern int ReturnIntegerIsoC(ref int input);

        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "callback_example_c")]
        public static extern void CallbackExampleIsoC(ref int total, ref int step,
            [MarshalAs(UnmanagedType.FunctionPtr)] CallbackExampleHandler callback);

        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "__interop_MOD_callback_example")]
        public static extern void CallbackExample(ref int total, ref int step,
            [MarshalAs(UnmanagedType.FunctionPtr)] CallbackExampleHandler callback);

        /// <summary>
        /// Demonstrates passing a fixed length string to Fortran as a raw array.
        /// </summary>
        /// <param name="text">Character array of length 10.</param>
        /// <param name="status">Set to 1 if passed text equals "test", -1 otherwise.</param>
        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "__interop_MOD_string_input_fixlen", CharSet = CharSet.Ansi)]
        public static extern void StringInput(char[] text, ref int status);

        /// <summary>
        /// Demonstrates passing a fixed length string to Fortran using the StringBuilder.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="status">Set to 1 if passed text equals "test", -1 otherwise.</param>
        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "__interop_MOD_string_input_fixlen", CharSet = CharSet.Ansi)]
        public static extern void StringInput(StringBuilder text, ref int status);

        /// <summary>
        /// Demonstrates passing a variable length string to Fortran.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="length"></param>
        /// <param name="status">Set to 1 if passed text equals "test", -1 otherwise.</param>
        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "__interop_MOD_string_input_varlen", CharSet = CharSet.Ansi)]
        public static extern void StringInput([MarshalAs(UnmanagedType.LPStr)] string text, int length, ref int status);

        /// <summary>
        /// Wrapper method for variable length string passing.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="status">Set to 1 if passed text equals "test", -1 otherwise.</param>
        public static void StringInput(string text, ref int status)
        {
            StringInput(text, text.Length, ref status);
        }

        /// <summary>
        /// Demonstrates receiving a fixed length string from Fortran as a raw array.
        /// </summary>
        /// <param name="text">Character array of length 20.</param>
        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "__interop_MOD_string_output_fixlen", CharSet = CharSet.Ansi)]
        public static extern void StringOutput([Out] char[] text);

        /// <summary>
        /// Demonstrates returning a variable length string from Fortran.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="length">Size of the buffer when calling, and length of the string in the buffer after calling.</param>
        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "__interop_MOD_string_output_varlen", CharSet = CharSet.Ansi)]
        public static extern void StringOutput([Out] char[] text, ref int length);

        /// <summary>
        /// Returns sum of the values in the passed structure.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="extra_value"></param>
        /// <returns></returns>
        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "__interop_MOD_pass_structure", CharSet = CharSet.Ansi)]
        public static extern double PassStruct(ref FortranInteropStruct data, ref double extra_value);

        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "pass_structure_c", CharSet = CharSet.Ansi)]
        public static extern double PassStructIsoC(ref FortranInteropStruct data, ref double extra_value);

        /// <summary>
        /// Modifies the values in the structure by adding the passed change amount.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="change"></param>
        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "__interop_MOD_modify_structure", CharSet = CharSet.Ansi)]
        public static extern void ModifyStruct([In, Out] ref FortranInteropStruct data, ref double change);

        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "__interop_MOD_return_structure", CharSet = CharSet.Ansi)]
        public static extern FortranInteropStruct ReturnStruct();

        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "__interop_MOD_pass_structure", CharSet = CharSet.Ansi)]
        public static extern double PassClass(FortranInteropClass data, ref double extra_value);

        /// <summary>
        /// Sums the values of the passed array of the specified length and returns the result.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="inputLength"></param>
        /// <returns></returns>
        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "__interop_MOD_sum_array")]
        public static extern double SumArray(double[] input, ref int inputLength);

        public static double SumArray(double[] input)
        {
            int length = input.Length;
            return SumArray(input, ref length);
        }

        /// <summary>
        /// Sums the values of the passed array of the specified length and returns the result.
        /// </summary>
        /// <remarks>This external method declaration contains extra marshalling instructions which are optional. These will however present arrays of incorrect length being passed.</remarks>
        /// <param name="input">Input values. Must be of length = 10.</param>
        /// <returns></returns>
        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "__interop_MOD_sum_array_fixed")]
        public static extern double SumArrayFixed([MarshalAs(UnmanagedType.LPArray, SizeConst = 10)] double[] input);

        /// <summary>
        /// Sums the values of the passed array of the specified length and returns the result.
        /// </summary>
        /// <param name="input">Input values. Must be of length = 10.</param>
        /// <returns></returns>
        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "__interop_MOD_sum_array_fixed")]
        public static extern double SumArrayFixedSimple(double[] input);

        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sum_array_c")]
        public static extern double SumArrayIsoC(double[] input, ref int inputLength);

        /// <summary>
        /// Sums the values of the passed 2D array of the specified dimensions and returns the result.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="inputLength1"></param>
        /// <param name="inputLength2"></param>
        /// <returns></returns>
        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sum_array_2d_c")]
        public static extern double SumArrayIsoC(double[,] input, ref int inputLength1, ref int inputLength2);

        /// <summary>
        /// Applies a rotation of the specified angle over Z-axis on the passed array of 3D vectors.
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="vectorCount">Number of 3D vectors in vectors.</param>
        /// <param name="angle">Rotation angle in radiants</param>
        /// <returns></returns>
        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "__interop_MOD_rotate_vectors")]
        public static extern double RotateVectors([In, Out] double[,] vectors, ref int vectorCount, ref double angle);

        /// <summary>
        /// Applies a rotation of the specified angle over Z-axis on the passed array of 3D vectors.
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="vectorCount">Number of 3D vectors in vectors.</param>
        /// <param name="angle">Rotation angle in radiants</param>
        /// <returns></returns>
        [DllImport("Interop.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "rotate_vectors_c")]
        public static extern double RotateVectorsIsoC([In, Out] double[,] vectors, ref int vectorCount, ref double angle);
    }

    /// <summary>
    /// Definition of the callback delegate. Must match the interface description in Fortran.
    /// </summary>
    /// <param name="value"></param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void CallbackExampleHandler(ref int value);

    /// <summary>
    /// </summary>
    /// <param name="value"></param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool CallbackExampleStatusHandler(ref int progress, string status);

    /// <summary>
    /// Structure used to demonstrate marshalling complex data types between .NET and Fortran.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct FortranInteropStruct
    {
        public int Id;

        /// <summary>
        /// Fixed length array holding double values.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public double[] Values;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] Name;
    }

    /// <summary>
    /// Class used to demonstrate marshalling complex data types between .NET and Fortran.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public class FortranInteropClass
    {

        internal const int ValuesSize = 10;
        internal const int NameSize = 16;

        public int Id;

        /// <summary>
        /// Fixed length array holding double values.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ValuesSize)]
        public double[] Values;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NameSize)]
        public char[] Name;

        public FortranInteropClass()
        {

            Values = new double[ValuesSize];
            Name = new char[NameSize];
        }
    }
}


