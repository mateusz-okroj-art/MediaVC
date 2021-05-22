using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediaVC.Difference;
using MediaVC.Tools.Difference;

namespace MediaVC.Tools.Tests
{
    public sealed class DifferenceCalculatorTestFixture : IDifferenceCalculatorTestFixture
    {
        #region Constructor

        public DifferenceCalculatorTestFixture()
        {

        }


        #endregion

        #region Fields



        #endregion

        #region Properties

        public IInputSource OneZero { get; }

        public IInputSource MFullBytes { get; }

        #endregion

        #region Methods

        public void Dispose()
        {
            OneZero.Dispose();
            MFullBytes.Dispose();
        }

        #endregion
    }
}
