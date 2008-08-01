namespace Rhino.Queues.Tests.Storage.Disk
{
	using System.IO;
	using System.Transactions;
	using MbUnit.Framework;
	using Rhino.Queues.Storage.Disk;

	[TestFixture]
	public class MutliFileQueueTests : PersistentQueueTestsBase
	{
		[Test]
		public void Can_limit_amount_of_items_in_queue_file()
		{
			using (var queue = new PersistentQueue(path, 10))
			{
				Assert.AreEqual(10, queue.MaxFileSize);
			}
		}

		[Test]
		public void Entering_more_than_count_of_items_will_work()
		{
			using (var queue = new PersistentQueue(path, 10))
			{
				for (byte i = 0; i < 11; i++)
				{
					using (var session = queue.OpenSession())
					using (var tx = new TransactionScope())
					{
						session.Enqueue(new[] { i });
						tx.Complete();
					}
				}
				Assert.AreEqual(11, queue.EstimatedCountOfItemsInQueue);
			}
		}

		[Test]
		public void When_creating_more_items_than_allowed_in_first_file_will_create_additional_file()
		{
			using (var queue = new PersistentQueue(path, 10))
			{
				for (byte i = 0; i < 11; i++)
				{
					using (var session = queue.OpenSession())
					using (var tx = new TransactionScope())
					{
						session.Enqueue(new[] { i });
						tx.Complete();
					}
				}
				Assert.AreEqual(1, queue.CurrentFileNumber);
			}
		}

		[Test]
		public void Can_resume_writing_to_second_file_when_restart_queue()
		{
			using (var queue = new PersistentQueue(path, 10))
			{
				for (byte i = 0; i < 11; i++)
				{
					using (var session = queue.OpenSession())
					using (var tx = new TransactionScope())
					{
						session.Enqueue(new[] { i });
						tx.Complete();
					}
				}
				Assert.AreEqual(1, queue.CurrentFileNumber);
			}
			using (var queue = new PersistentQueue(path, 10))
			{
				for (byte i = 0; i < 2; i++)
				{
					using (var session = queue.OpenSession())
					using (var tx = new TransactionScope())
					{
						session.Enqueue(new[] { i });
						tx.Complete();
					}
				}
				Assert.AreEqual(1, queue.CurrentFileNumber);
			}
		}

		[Test]
		public void Can_dequeue_from_all_files()
		{
			using (var queue = new PersistentQueue(path, 10))
			{
				for (byte i = 0; i < 12; i++)
				{
					using (var session = queue.OpenSession())
					using (var tx = new TransactionScope())
					{
						session.Enqueue(new[] { i });
						tx.Complete();
					}
				}
				Assert.AreEqual(1, queue.CurrentFileNumber);
			}

			using (var queue = new PersistentQueue(path, 10))
			{
				for (byte i = 0; i < 12; i++)
				{
					using (var session = queue.OpenSession())
					using (var tx = new TransactionScope())
					{
						Assert.AreEqual(i, session.Dequeue()[0]);
						tx.Complete();
					}
				}
			}
		}

		[Test]
		public void Can_dequeue_from_all_files_after_restart()
		{
			using (var queue = new PersistentQueue(path, 10))
			{
				for (byte i = 0; i < 12; i++)
				{
					using (var session = queue.OpenSession())
					using (var tx = new TransactionScope())
					{
						session.Enqueue(new[] { i });
						tx.Complete();
					}
				}
				Assert.AreEqual(1, queue.CurrentFileNumber);
			}

			using (var queue = new PersistentQueue(path, 10))
			{
				for (byte i = 0; i < 3; i++)
				{
					using (var session = queue.OpenSession())
					using (var tx = new TransactionScope())
					{
						session.Enqueue(new[] { i });
						tx.Complete();
					}
				}
				Assert.AreEqual(1, queue.CurrentFileNumber);
			}


			using (var queue = new PersistentQueue(path, 10))
			{
				using (var session = queue.OpenSession())
				using (var tx = new TransactionScope())
				{
					for (byte i = 0; i < 12; i++)
					{
						Assert.AreEqual(i, session.Dequeue()[0]);
					}

					for (byte i = 0; i < 3; i++)
					{
						Assert.AreEqual(i, session.Dequeue()[0]);
					}
					tx.Complete();
				}
			}
		}

		[Test]
		public void After_reading_all_items_from_file_that_is_not_the_active_file_should_delete_file()
		{
			using (var queue = new PersistentQueue(path, 10))
			{
				for (byte i = 0; i < 12; i++)
				{
					using (var session = queue.OpenSession())
					using (var tx = new TransactionScope())
					{
						session.Enqueue(new[] { i });
						tx.Complete();
					}
				}
				Assert.AreEqual(1, queue.CurrentFileNumber);
			}

			using (var queue = new PersistentQueue(path, 10))
			{
				for (byte i = 0; i < 12; i++)
				{
					using (var session = queue.OpenSession())
					using (var tx = new TransactionScope())
					{
						Assert.AreEqual(i, session.Dequeue()[0]);
						tx.Complete();
					}
				}
			}

			Assert.IsFalse(
				File.Exists(Path.Combine(path, "data.0"))
				);
		}
	}
}