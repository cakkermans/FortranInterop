using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FortranInterop
{
    public class StringTests
    {
        
        [Fact]
        public void ArrayString()
        {

            
            var input1 = "Test      ".ToCharArray();
            var input2 = "not Test  ".ToCharArray();
            int status = 0;

            // Ensure that both character arrays contain 10 elements.
            Assert.Equal(10, input1.Length);
            Assert.Equal(10, input2.Length);

            Interop.StringInput(input1, ref status);
            Assert.Equal(1, status);

            Interop.StringInput(input2, ref status);
            Assert.Equal(-1, status);
        }

        [Fact]
        public void StringBuilderStrings()
        {

            // This does not really work due to the padding being 'wrong'. Fortran
            // expects blank ' ' padding as .NET adds a trailing null?
            var input1 = new StringBuilder("Test", 10);
            var input2 = new StringBuilder(10);
            int status = 0;

            input2.Append("not ");
            input2.Append("Test");

            Interop.StringInput(input1, ref status);
            Assert.Equal(1, status);

            Interop.StringInput(input2, ref status);
            Assert.Equal(-1, status);
        }

        [Fact]
        public void VariableLengthString()
        {

            var input1 = "Test";
            var input2 = "not Test";
            int status = 0;

            Interop.StringInput(input1, input1.Length, ref status);
            Assert.Equal(1, status);

            Interop.StringInput(input2, input2.Length, ref status);
            Assert.Equal(-1, status);
        }

        [Fact]
        public void ReturnArrayString()
        {

            var result1 = new char[20];
            string result;

            // Call the function and convert the character array to a string.
            Interop.StringOutput(result1);
            result = new string(result1);

            // Note that we need to take into account the ' ' padding Fortran uses.
            Assert.Equal("Hello from Fortran  ", result);
        }

        [Fact]
        public void ReturnVariableLengthString()
        {

            int length = 20;
            var result1 = new char[length];
            string result;

            // Call the function and convert the character array to a string.
            Interop.StringOutput(result1, ref length);
            result = new string(result1, 0, length);

            Assert.Equal("Hello from Fortran", result);
        }
    }
}
