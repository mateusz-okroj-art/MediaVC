using System;
using System.IO;
using System.Threading.Tasks;

using Xunit;

namespace MediaVC.Core.Tests.Difference.Strategies
{
    public class StreamStrategy
    {
        [Fact]
        public void Constructor_WhenArgumentIsNull_ShouldThrowException()
        {
            FileStream file = null;

            Assert.Throws<ArgumentNullException>(() => new MediaVC.Difference.Strategies.StreamStrategy(file));
        }

        [Fact]
        public void File_ShouldReturnValid()
        {
            using var file = GenerateTempFile();
            var result = new MediaVC.Difference.Strategies.StreamStrategy(file);

            Assert.Equal(file, result.Stream);
        }

        [Fact]
        public void Length_ShouldReturnValid()
        {
            using var file = GenerateTempFile();
            var result = new MediaVC.Difference.Strategies.StreamStrategy(file);

            Assert.Equal(file.Length, result.Length);
        }

        [Fact]
        public void Position_ShouldSetAndReturnValid()
        {
            using var file = GenerateTempFile();
            var result = new MediaVC.Difference.Strategies.StreamStrategy(file);

            var halfPosition = (long)Math.Floor(file.Length / 2.0);

            result.Position = halfPosition;
            Assert.Equal(halfPosition, file.Position);
            Assert.Equal(file.Position, result.Position);
        }

        [Fact]
        public async Task ReadByteAsync_WhenStreamIsLongerThanBufferLength_ShouldNotThrowWhenRead()
        {
            var data = new byte[MediaVC.Difference.Strategies.StreamStrategy.bufferLength+1];
            using var stream = new MemoryStream(data);

            var strategy = new MediaVC.Difference.Strategies.StreamStrategy(stream);
            strategy.Position = 0;
            Assert.Equal(0, await strategy.ReadByteAsync());
            strategy.Position = MediaVC.Difference.Strategies.StreamStrategy.bufferLength;
            Assert.Equal(0, await strategy.ReadByteAsync());
            Assert.Equal(0, await strategy.ReadByteAsync());
        }

        private static FileStream GenerateTempFile()
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
