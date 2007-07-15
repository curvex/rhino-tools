﻿#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using System.Reflection;
using MbUnit.Framework;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Tests.Expectations;
using Rhino.Mocks.Tests.Utilities;
using Castle.Core.Interceptor;

namespace Rhino.Mocks.Tests.Impl
{
	[TestFixture]
	public class ReplayMockStateTests
	{
		private static MockRepository mocks;
		private static MethodInfo startsWith;
		private static RecordMockState record;
		private ReplayMockState replay;
		private ProxyInstance proxy;

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			proxy = new ProxyInstance(mocks);
			startsWith = CreateMethodInfo();
			record = new RecordMockState(proxy, ReplayMockStateTests.mocks);
            record.MethodCall(new FakeInvocation(startsWith), startsWith, "2");
			replay = new ReplayMockState(record);
		}


		[Test]
		public void CreatingReplayMockStateFromRecordMockStateCopiesTheExpectationList()
		{
			Assert.AreEqual(1, Get.Recorder(mocks).GetAllExpectationsForProxy(proxy).Count);
		}

		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "String.StartsWith(\"2\"); Expected #1, Actual #0.")]
		public void ExpectedMethodCallOnReplay()
		{
			ReplayMockState replay = new ReplayMockState(record);
			replay.Verify();
		}

		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "String.EndsWith(\"2\"); Expected #0, Actual #1.")]
		public void UnexpectedMethodCallOnReplayThrows()
		{
			MethodInfo endsWith = MethodCallTests.GetMethodInfo("EndsWith", "2");
            replay.MethodCall(new FakeInvocation(endsWith), endsWith, "2");
		}

		[Test]
		public void VerifyWhenAllExpectedCallsWereCalled()
		{
			MethodInfo methodInfo = CreateMethodInfo();
			this.replay.MethodCall(new FakeInvocation(methodInfo), methodInfo, "2");
			this.replay.Verify();
		}

		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "String.StartsWith(\"2\"); Expected #1, Actual #0.")]
		public void VerifyWhenNotAllExpectedCallsWereCalled()
		{
			ReplayMockState replay = new ReplayMockState(record);
			replay.Verify();
		}

		[Test]
		[ExpectedException(typeof (ExpectationViolationException), "String.EndsWith(null); Expected #0, Actual #1.")]
		public void VerifyWhenMismatchArgsContainsNull()
		{
			MethodInfo endsWith = MethodCallTests.GetMethodInfo("EndsWith", "2");
            replay.MethodCall(new FakeInvocation(endsWith), endsWith, new object[1] { null });
		}

		[Test]
		public void VerifyReportsAllMissingExpectationsWhenCalled()
		{
			record.LastExpectation.ReturnValue = true;
			MethodInfo method = CreateMethodInfo();
			record.MethodCall(new FakeInvocation(method), method, "r");
			record.LastExpectation.ReturnValue = true;
			record.MethodCall(new FakeInvocation(method), method, "y");
			record.LastExpectation.ReturnValue = true;
			record.LastExpectation.Expected = new Range(2, 2);
			ReplayMockState replay = new ReplayMockState(record);
			try
			{
				replay.Verify();
			}
			catch (Exception e)
			{
				string message = "String.StartsWith(\"2\"); Expected #1, Actual #0.\r\n" +
					"String.StartsWith(\"r\"); Expected #1, Actual #0.\r\n" +
					"String.StartsWith(\"y\"); Expected #2, Actual #0.";
				Assert.AreEqual(message, e.Message);
			}
		}

		[Test]
		public void VerifyReportsAllMissingExpectationWhenCalledOnOrdered()
		{
			using (mocks.Ordered())
			{
				record.LastExpectation.ReturnValue = true;
				MethodInfo method = CreateMethodInfo();
                record.MethodCall(new FakeInvocation(method), method, "r");
				record.LastExpectation.ReturnValue = true;
                record.MethodCall(new FakeInvocation(method), method, "y");
				record.LastExpectation.ReturnValue = true;
				record.LastExpectation.Expected = new Range(2, 2);
			}
			ReplayMockState replay = new ReplayMockState(record);
			try
			{
				replay.Verify();
			}
			catch (Exception e)
			{
				string message = "String.StartsWith(\"2\"); Expected #1, Actual #0.\r\n" +
					"String.StartsWith(\"r\"); Expected #1, Actual #0.\r\n" +
					"String.StartsWith(\"y\"); Expected #2, Actual #0.";
				Assert.AreEqual(message, e.Message);
			}
		}

		#region Implementation

		private static MethodInfo CreateMethodInfo()
		{
			return MethodCallTests.GetMethodInfo("StartsWith", "");
		}

		#endregion
	}
}