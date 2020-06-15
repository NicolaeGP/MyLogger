using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;

namespace LogComponent.Test
{
    public class Tests
    {
        AsyncFileLogger _logger;

        [Test, Sequential]
        public void TestWriteLog()
        {
            _logger = new AsyncFileLogger();
            var id = Guid.NewGuid();
            _logger.Log(LogLevel.Information, $"Hello from testing {id.ToString()}");
            _logger.StopAndFlush();
            var logFiles = Directory.GetFiles("logs");
            var fileContents = File.ReadAllText(logFiles[0]);
            Assert.IsTrue(fileContents.Contains(id.ToString()));
        }

        [Test, Sequential]
        public void TestMidnightLog()
        {
            var dateTimeMock = new Mock<IDateTime>();
            dateTimeMock.Setup(fake => fake.Now()).Returns(DateTime.Now);
            _logger = new AsyncFileLogger(dateTimeProvider: dateTimeMock.Object);

            _logger.Log(LogLevel.Information, $"Hello from testing midnight now");
            Thread.Sleep(1000);

            dateTimeMock.Setup(fake => fake.Now()).Returns(DateTime.Now.AddDays(1));

            _logger.Log(LogLevel.Information, $"Hello from testing midnight tomorrow");
            _logger.StopAndFlush();

            var logFiles = Directory.GetFiles("logs");
            Assert.AreEqual(logFiles.Length, 2);
            var fileContents1 = File.ReadAllText(logFiles[0]);
            var fileContents2 = File.ReadAllText(logFiles[1]);
            Assert.IsTrue(fileContents1.Contains("Hello from testing midnight now"));
            Assert.IsTrue(fileContents2.Contains("Hello from testing midnight tomorrow"));
        }

        [Test, Sequential]
        public void TestStopAndFlush()
        {
            _logger = new AsyncFileLogger();

            for (int i = 0; i < 15; i++)
            {
                _logger.Log(LogLevel.Information, "Number with Flush: " + i.ToString());
            }

            _logger.StopAndFlush();
            var logFiles = Directory.GetFiles("logs");
            var fileContents = File.ReadAllLines(logFiles[0]);

            Assert.AreEqual(fileContents.Length, 15);
        }

        [Test, Sequential]
        public void TestStop()
        {
            _logger = new AsyncFileLogger();

            for (int i = 0; i < 15; i++)
            {
                _logger.Log(LogLevel.Information, "Number with Flush: " + i.ToString());
                Thread.Sleep(10);
            }

            _logger.Stop();
            var logFiles = Directory.GetFiles("logs");
            var fileContents = File.ReadAllLines(logFiles[0]);

            Assert.IsTrue(fileContents.Length < 15);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete("logs", true);
        }
    }
}