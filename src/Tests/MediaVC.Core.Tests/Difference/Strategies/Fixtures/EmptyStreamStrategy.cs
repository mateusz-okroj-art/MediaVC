using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace MediaVC.Core.Tests.Difference.Strategies
{
    public class EmptyStreamStrategy
    {
        private readonly MediaVC.Difference.Strategies.EmptyStreamStrategy fixture = new();

        [Fact]
        public void Length_ShouldReturnValidValue()
        {
            Assert.Equal(0, this.fixture.Length);
        }

        [Fact]
        public void Position_get_ShouldReturnValidValue()
        {
            Assert.Equal(-1, this.fixture.Position);
        }

        [Fact]
        public void Position_set_ShouldThrowException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => this.fixture.Position = 0);
        }

        [Fact]
        public void Equals_WhenArgumentLengthIsEquals_ShouldReturnTrue()
        {
            var mock = 
        }

        [Fact]
        public void Equals_WhenArgumentLengthIsNotEqual_ShouldReturnFalse()
        {

        }

        [Fact]
        public void Read_ShouldThrowException()
        {

        }

        [Fact]
        public void ReadByte_ShouldThrowException()
        {

        }
    }
}
