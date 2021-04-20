using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaVC.Difference.Strategies
{
    internal sealed class FileStreamStrategy : IInputSourceStrategy
    {
        public long Length { get; }
        public long Position { get; set; }

        public bool Equals(IInputSourceStrategy? other)
        {
            throw new NotImplementedException();
        }
    }
}
