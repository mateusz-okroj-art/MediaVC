using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using Moq;
using MediaVC.Difference;

namespace MediaVC.Tools.Tests.Difference.DifferenceCalculator
{
    public class Constructors
    {
        #region Fields

        public static readonly object[][] constructor1_TestCasesWithNull = new object[][]
        {
            new object[]{ null,null },
            new object[]{ new Mock<IInputSource>().Object, null },
            new object[]{ null, new Mock<IInputSource>().Object },
        };

        public static readonly object[][] constructor1_ValidTestCases = new object[][]
        {
            new object[]{ new Mock<IInputSource>().Object, new Mock<IInputSource>().Object }
        };

        #endregion

        #region Methods

        [Theory]
        [MemberData(nameof(constructor1_TestCasesWithNull))]
        public void Constructor1_WhenHaveAnyNull_ShouldThrowException(IInputSource input1, IInputSource input2)
        {
            Assert.Throws<ArgumentNullException>(() => new Tools.Difference.DifferenceCalculator(input1, input2));
        }

        #endregion
    }
}
