using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FortranInterop
{
    public class GlueTests
    {
        
        [Fact]
        public void ReturnInterger()
        {

            var expected = 5;
            var result = FortranWrapper.ReturnInteger(expected);
            Assert.Equal(expected, result);

            result = FortranWrapper.ReturnIntegerIsoC(expected);
            Assert.Equal(expected, result);
        }
    }
}
