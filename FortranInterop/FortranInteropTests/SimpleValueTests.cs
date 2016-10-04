using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FortranInterop
{
    public class SimpleValueTests
    {

        [Fact]
        public void TestReturnInteger()
        {

            int value = 3656756;

            var input = Interop.ReturnInteger(ref value);
            Assert.Equal(value, input);
        }

        [Fact]
        public void TestReturnIntegerIsoC()
        {

            int value = 78034423;

            var input = Interop.ReturnIntegerIsoC(ref value);
            Assert.Equal(value, input);
        }

        [Fact]
        public void TestReturnDouble()
        {

            double input = 1D / 3D;

            var result = Interop.ReturnDouble(ref input);
            Assert.Equal(input, result);
        }

        [Fact]
        public void TestReturnDoubleIsoC()
        {

            double input = 1D / 3D;

            var result = Interop.ReturnDoubleIsoC(ref input);
            Assert.Equal(input, result);
        }

        [Fact]
        public void CopyInteger()
        {

            int input = 678235945;
            int result;

            Interop.CopyInteger(ref input, out result);
            Assert.Equal(result, input);
        }

        [Fact]
        public void CopyIntegerIsoC()
        {

            int input = 678235945;
            int result;

            Interop.CopyIntegerIsoC(ref input, out result);
            Assert.Equal(result, input);
        }
    }
}
