// Copyright (c) 2005 - 2008 Ayende Rahien (ayende@ayende.com)
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
namespace Rhino.Queues.Storage.InMemory
{
	using System;
	using System.Collections.Generic;
	using Impl;
	using Threading;

	public abstract class MessageStorageBase : IMessageStorage
	{
		protected readonly IBlockingQueue<string> messagesEvents = new BlockingQueue<string>();

		#region IMessageStorage Members

		public void Add(string name, TransportMessage message)
		{
			OnAdd(name, message);
			messagesEvents.Enqueue(name);
		}

		protected abstract void OnAdd(string name, TransportMessage message);

		public abstract IEnumerable<TransportMessage> PullMessagesFor(string name, Predicate<TransportMessage> predicate);
		public abstract IEnumerable<TransportMessage> PullMessagesFor(string name);
		public abstract bool WaitForNewMessages(string name);

		public abstract bool Exists(string name);

		public bool WaitForNewMessages(TimeSpan timeToWait, out string queueWithNewMessages)
		{
			return messagesEvents.Dequeue(timeToWait, out queueWithNewMessages);
		}

		public abstract IEnumerable<string> Queues { get; }

		public void Dispose()
		{
			messagesEvents.Dispose();
		}

		#endregion
	}
}