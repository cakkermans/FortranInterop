using System;

namespace FortranInterop
{
    class Program
    {
        static void Main(string[] args)
        {

            int value, result;
            value = 5;

            // Pass the input value as a reference. This is required to comply with the Fortran argument
            // passing method.
            result = Interop.ReturnInteger(ref value);
            Console.WriteLine("Fortran returned value: {0}", result);

            // Pass the input value to a similar function but now using the ISO C binding module on the Fortran side.
            result = Interop.ReturnIntegerIsoC(ref value);
            Console.WriteLine("Fortran returned value using ISO C: {0}", result);

            // Pass the input value using a mixed mode wrapper asembly to Fortran.
            result = FortranWrapper.ReturnInteger(5);
            Console.WriteLine("Fortran returned value: {0}", result);

            Console.Read();
        }
    }
}
