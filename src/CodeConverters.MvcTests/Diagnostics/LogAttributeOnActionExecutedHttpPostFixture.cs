﻿using System.Linq;
using System.Web.Mvc;
using CodeConverters.Mvc.Diagnostics;
using CodeConverters.MvcTests.Diagnostics.Helpers;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using Moq;
using Xunit;

namespace CodeConverters.MvcTests.Diagnostics
{
    public class LogAttributeOnActionExecutedHttpPostFixture 
    {
        private readonly LogAttribute _sut;
        private readonly MemoryAppender _memoryAppender;
        private readonly ActionExecutedContext _context;

        public LogAttributeOnActionExecutedHttpPostFixture()
        {
            _memoryAppender = new MemoryAppender();
            BasicConfigurator.Configure(_memoryAppender);
            _sut = new LogAttribute();
            _context = ObjectMother.CreateActionActionExecutedContextFake();
            Mock.Get(_context.HttpContext.Request).SetupGet(r => r.HttpMethod).Returns("POST");
        }
        
        [Fact]
        public void UsesTheGivenControllerAsTheLoggerName()
        {
            _sut.OnActionExecuted(_context);

            Assert.True(_memoryAppender.GetEvents().All(e => e.LoggerName == typeof(DummyController).FullName), "Expecting logger name to be that of the controller type");
        }

        [Fact]
        public void PostsAreNotLoggedIfFilterDisabled()
        {
            _sut.Enabled = false;
            _sut.OnActionExecuted(_context);

            Assert.False(_memoryAppender.GetEvents().Any(), "Expected no messages in the logs");
        }

        [Fact]
        public void PostsAreLoggedAtInfoLevel()
        {
            _sut.OnActionExecuted(_context);

            Assert.True(_memoryAppender.GetEvents().Count() == 1, "Expected single messages in the logs");
            Assert.True(_memoryAppender.GetEvents().Count(le => le.Level == Level.Info) == 1, "Expected single Info messages in the logs");
        }
    }
}