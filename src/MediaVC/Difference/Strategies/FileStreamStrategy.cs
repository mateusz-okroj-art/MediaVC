using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaVC.Difference.Strategies
{
    internal sealed class FileStreamStrategy : IInputSourceStrategy
    {
        public FileStreamStrategy(FileStream file)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));

            if (!File.CanRead)
                throw new IOException("Stream is not readable.");
        }

        public FileStream File { get; }

        public long Length => File.Length;

        public long Position
        {
            get => File.Position;
            set => File.Position = value;
        }

        public bool Equals(IInputSourceStrategy? other)
        {
            throw new NotImplementedException();
        }
    }
}
