﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace MediaVC.Core.Tests.Difference.Strategies
{
    public class FileStreamStrategy
    {
        [Fact]
        public void Constructor_WhenArgumentIsNull_ShouldThrowException()
        {
            FileStream file = null;

            Assert.Throws<ArgumentNullException>(() => new MediaVC.Difference.Strategies.FileStreamStrategy(file));
        }

        [Fact]
        public void File_ShouldReturnValid()
        {
            using var file = GenerateTempFile();
            var result = new MediaVC.Difference.Strategies.FileStreamStrategy(file);

            Assert.Equal(file, result.File);
        }

        [Fact]
        public void Length_ShouldReturnValid()
        {
            using var file = GenerateTempFile();
            var result = new MediaVC.Difference.Strategies.FileStreamStrategy(file);

            Assert.Equal(file.Length, result.Length);
        }

        [Fact]
        public void Position_ShouldSetAndReturnValid()
        {
            using var file = GenerateTempFile();
            var result = new MediaVC.Difference.Strategies.FileStreamStrategy(file);

            var halfPosition = (long)Math.Floor(file.Length / 2.0);

            result.Position = halfPosition;
            Assert.Equal(halfPosition, file.Position);
            Assert.Equal(file.Position, result.Position);
        }

        private FileStream GenerateTempFile()
        {
            var rand = new Random(5);

            var guid = Guid.NewGuid();
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"{guid}.tmp");
            var file = File.Create(path, 1000, FileOptions.DeleteOnClose);

            var count = rand.Next(1, 15);

            for(byte i = 1; i <= count; ++i)
                file.WriteByte(i);

            file.Flush();
            file.Position = 0;

            return file;
        }
    }
}
