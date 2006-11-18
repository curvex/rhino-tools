using System;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Util;

namespace Rhino.Commons.Logging
{
	/// <summary>
	/// Default parameters are set for:
	/// @Date, @Thread, @Level, @Logger, @Message, @Exception
	/// </summary>
	public class RollingSqliteAppender : AdoNetAppender, IDisposable
	{
		private long maxNumberOfBackups = 3;
		private string directory = Environment.CurrentDirectory;
		private string connectionStringFormat = "Data Source={0};Version=3;New={1}";
		private string fileNameFormat = "{0}.log4net";
		private string tableName = "Logs";
		private const string selectCount = "SELECT COUNT(*) FROM {0}";
		private int maxNumberOfRows = 50000;

		private string createScript =
			@"CREATE TABLE Logs (
	LogId		INTEGER PRIMARY KEY,
	Date		DATETIME NOT NULL,
	Thread		VARCHAR(255) NOT NULL,
	Level		VARCHAR(50) NOT NULL,
	Logger		VARCHAR(512) NOT NULL,
	Message		TEXT NOT NULL,
	Exception	TEXT NULL	
);";

		long numberOfRows = 0;
		private int curRollBackups;
		private string currentFilePath;

		public RollingSqliteAppender()
		{
			ConnectionType = typeof (SQLiteConnection).AssemblyQualifiedName;
			ReconnectOnError = true;
			CommandText =
				@"INSERT INTO Logs (Date, Thread, Level, Logger, Message, Exception) 
						VALUES (@Date, @Thread, @Level, @Logger, @Message, @Exception)";

			AdoNetAppenderParameter date = new AdoNetAppenderParameter();
			date.DbType = DbType.DateTime;
			date.ParameterName = "Date";
			date.Layout = new RawTimeStampLayout();
			AddParameter(date);

			AdoNetAppenderParameter thread = new AdoNetAppenderParameter();
			thread.DbType = DbType.String;
			thread.ParameterName = "Thread";
			thread.Size = 255;
			thread.Layout = new Layout2RawLayoutAdapter(new PatternLayout("%thread"));
			AddParameter(thread);

			AdoNetAppenderParameter level = new AdoNetAppenderParameter();
			level.DbType = DbType.String;
			level.ParameterName = "Level";
			level.Size = 50;
			level.Layout = new Layout2RawLayoutAdapter(new PatternLayout("%level"));
			AddParameter(level);

			AdoNetAppenderParameter logger = new AdoNetAppenderParameter();
			logger.DbType = DbType.String;
			logger.ParameterName = "Logger";
			logger.Size = 512;
			logger.Layout = new Layout2RawLayoutAdapter(new PatternLayout("%logger"));
			AddParameter(logger);

			AdoNetAppenderParameter message = new AdoNetAppenderParameter();
			message.DbType = DbType.String;
			message.ParameterName = "Message";
			message.Size = 2000;
			message.Layout = new Layout2RawLayoutAdapter(new PatternLayout("%message"));
			AddParameter(message);

			AdoNetAppenderParameter exception = new AdoNetAppenderParameter();
			exception.DbType = DbType.String;
			exception.ParameterName = "Exception";
			exception.Size = 4000;
			exception.Layout = new Layout2RawLayoutAdapter(new PatternLayout("%exception"));
			AddParameter(exception);
		}

		/// <summary>
		/// The creation script for the database.
		/// Create the table needed for logging.
		/// Defaults to creating a table named "Logs" with 
		/// (date, level, logger, message, exception) columns.
		/// </summary>
		public string CreateScript
		{
			get { return createScript; }
			set { createScript = value; }
		}

		/// <summary>
		/// The name of the table (used when calculating row counts).
		/// Defaults to "Logs"
		/// </summary>
		public string TableName
		{
			get { return tableName; }
			set { tableName = value; }
		}

		/// <summary>
		/// The file format that will be created.
		/// The default is for:
		/// {0}.log4net
		/// Where {0} is the current process name
		/// </summary>
		public string FileNameFormat
		{
			get { return fileNameFormat; }
			set { fileNameFormat = value; }
		}

		/// <summary>
		/// The directory to put the logs files at.
		/// Defaults to the current directory.
		/// </summary>
		public string Directory
		{
			get { return directory; }
			set { directory = value; }
		}

		/// <summary>
		/// Max number of rows per file.
		/// Defaults to 50,000
		/// </summary>
		public int MaxNumberOfRows
		{
			get { return maxNumberOfRows; }
			set { maxNumberOfRows = value; }
		}

		/// <summary>
		/// Number of backup files to use.
		/// Defaults to 3
		/// </summary>
		public long MaxNumberOfBackups
		{
			get { return maxNumberOfBackups; }
			set { maxNumberOfBackups = value; }
		}

		protected override void SendBuffer(LoggingEvent[] events)
		{
			numberOfRows += events.Length;
			base.SendBuffer(events);
			AdjustFileAfterAppend();
		}

		private void AdjustFileAfterAppend()
		{
			if (numberOfRows < MaxNumberOfRows)
				return;

			Connection.Close(); //close the file, so we can move it.

			RollOverRenameFiles(CurrentFilePath);

			ActivateOptions();
		}

		protected void RollOverRenameFiles(string baseFileName)
		{
			// If maxBackups <= 0, then there is no file renaming to be done.
			if (MaxNumberOfBackups != 0)
			{
				// Delete the oldest file, to keep Windows happy.
				if (curRollBackups == MaxNumberOfBackups)
				{
					File.Delete(baseFileName + '.' + MaxNumberOfBackups);
					curRollBackups--;
				}

				// Map {(maxBackupIndex - 1), ..., 2, 1} to {maxBackupIndex, ..., 3, 2}
				for (int i = curRollBackups; i >= 1; i--)
				{
					RollFile((baseFileName + "." + i), (baseFileName + '.' + (i + 1)));
				}

				curRollBackups++;

				// Rename fileName to fileName.1
				RollFile(baseFileName, baseFileName + ".1");
			}
			else // no backups specified, just delete and start from scratch
			{
				DeleteFile(baseFileName);
			}
		}

		protected void RollFile(string fromFile, string toFile)
		{
			if (File.Exists(fromFile))
			{
				// Delete the toFile if it exists
				DeleteFile(toFile);

				// We may not have permission to move the file, or the file may be locked
				try
				{
					LogLog.Debug("RollingFileAppender: Moving [" + fromFile + "] -> [" + toFile + "]");
					File.Move(fromFile, toFile);
				}
				catch (Exception moveEx)
				{
					ErrorHandler.Error("Exception while rolling file [" + fromFile + "] -> [" + toFile + "]",
					                   moveEx,
					                   ErrorCode.GenericFailure);
				}
			}
			else
			{
				LogLog.Warn("RollingFileAppender: Cannot RollFile [" + fromFile + "] -> [" + toFile + "]. Source does not exist");
			}
		}

		protected void DeleteFile(string file)
		{
			// We may not have permission to delete the file, or the file may be locked
			try
			{
				LogLog.Debug("RollingFileAppender: Deleting [" + file + "]");
				File.Delete(file);
			}
			catch (Exception moveEx)
			{
				ErrorHandler.Error("Exception while rolling file [" + file + "]", moveEx, ErrorCode.GenericFailure);
			}
		}

		public string CurrentFilePath
		{
			get
			{
				if (currentFilePath == null)
				{
					string processName = Process.GetCurrentProcess().ProcessName;
					string currentFile = string.Format(FileNameFormat, processName);
					currentFilePath = Path.Combine(directory, currentFile);
				}
				return currentFilePath;
			}
		}

		private void CheckNumberOfRows()
		{
			if (Connection == null || Connection.State != ConnectionState.Open)
			{
				LogLog.Debug(
					"RollingSqliteAppender: could not check number of rows in database (usually because of an error in creating / talking to it), assuming 0 rows exists.");
				numberOfRows = 0;
				return;
			}
			using (IDbCommand cmd = Connection.CreateCommand())
			{
				cmd.CommandText = string.Format(selectCount, TableName);
				numberOfRows = (long) cmd.ExecuteScalar();
			}
		}

		public override void ActivateOptions()
		{
			bool shouldCreateFile = !File.Exists(CurrentFilePath);
			ConnectionString = string.Format(connectionStringFormat, CurrentFilePath, shouldCreateFile);
			base.ActivateOptions();
			if (!shouldCreateFile)
				return;
			using (IDbCommand cmd = Connection.CreateCommand())
			{
				cmd.CommandText = CreateScript;
				cmd.ExecuteNonQuery();
			}
			CheckNumberOfRows();
		}

		public void Dispose()
		{
			if (Connection != null)
				Connection.Dispose();
		}
	}
}