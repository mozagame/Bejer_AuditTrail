//--------------------------------------------------------------
// ScriptModuleAuditTrail.cs - Audit Trail Database Module
// Beijer iX Developer
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
	using System.Windows.Forms;
	using System;
	using System.Data.SQLite;
	using System.IO;
	using System.Data;

	public partial class ScriptModuleAuditTrail
	{
		// Database paths
	//	private const string DB_PATH = @"\\Flash\AuditTrail\audit.db";
	//	private const string DB_BACKUP_PATH = @"\\Flash\AuditTrail\backup";
	//	private const string DB_EXPORT_PATH = @"\\Flash\AuditTrail\export";
		// Đổi đường dẫn - dùng đường dẫn tương đối
		private const string DB_PATH = @".\AuditTrail\audit.db";
		private const string DB_BACKUP_PATH = @".\AuditTrail\backup";
		private const string DB_EXPORT_PATH = @".\AuditTrail\export";
		/// <summary>
		/// Initialize database - Call from first screen load
		/// </summary>
		public void Initialize()
		{
			try
			{
				// Create directories
				string dbDir = Path.GetDirectoryName(DB_PATH);
				if (dbDir != "" && !Directory.Exists(dbDir))
					Directory.CreateDirectory(dbDir);

				if (!Directory.Exists(DB_BACKUP_PATH))
					Directory.CreateDirectory(DB_BACKUP_PATH);

				if (!Directory.Exists(DB_EXPORT_PATH))
					Directory.CreateDirectory(DB_EXPORT_PATH);

				using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + DB_PATH + ";Version=3;"))
				{
					conn.Open();
                    
					// Create table - split long SQL string
					string sql = "CREATE TABLE IF NOT EXISTS AuditTrail (" 
						+ "Id INTEGER PRIMARY KEY AUTOINCREMENT" 
						+ ", TimestampUtc INTEGER NOT NULL" 
						+ ", EventCategory TEXT NOT NULL" 
						+ ", EventType TEXT NOT NULL" 
						+ ", UserName TEXT NOT NULL" 
						+ ", Description TEXT NOT NULL" 
						+ ", ParameterName TEXT" 
						+ ", OldValue TEXT" 
						+ ", NewValue TEXT" 
						+ ", Reason TEXT" 
						+ ", BatchNumber TEXT" 
						+ ", AlarmCode TEXT" 
						+ ", StationId TEXT DEFAULT 'HMI-01'" 
						+ ", CreatedAt INTEGER NOT NULL)";
                        
					SQLiteCommand cmd = new SQLiteCommand(sql, conn);
					cmd.ExecuteNonQuery();

					// Create indexes
					sql = "CREATE INDEX IF NOT EXISTS idx_TimestampUtc ON AuditTrail(TimestampUtc)";
					cmd = new SQLiteCommand(sql, conn);
					cmd.ExecuteNonQuery();

					sql = "CREATE INDEX IF NOT EXISTS idx_EventCategory ON AuditTrail(EventCategory)";
					cmd = new SQLiteCommand(sql, conn);
					cmd.ExecuteNonQuery();

					sql = "CREATE INDEX IF NOT EXISTS idx_BatchNumber ON AuditTrail(BatchNumber)";
					cmd = new SQLiteCommand(sql, conn);
					cmd.ExecuteNonQuery();
				}

			//	MessageBox.Show("AuditTrail: Database initialized successfully!");
			}
			catch (Exception ex)
			{
				MessageBox.Show("AuditTrail Init Error: " + ex.Message);
			}
		}

		/// <summary>
		/// Log an event to audit trail
		/// </summary>
		public void LogEvent(string eventCategory, string eventType, string userName, string description, string batchNumber, string alarmCode)
		{
			try
			{
				using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + DB_PATH + ";Version=3;"))
				{
					conn.Open();

					string sql = "INSERT INTO AuditTrail (" 
						+ "TimestampUtc, EventCategory, EventType, UserName, Description, BatchNumber, AlarmCode, StationId) VALUES (" 
						+ "@TimestampUtc, @EventCategory, @EventType, @UserName, @Description, @BatchNumber, @AlarmCode, @StationId)";

					SQLiteCommand cmd = new SQLiteCommand(sql, conn);
					cmd.Parameters.AddWithValue("@TimestampUtc", DateTimeToUnixMs(DateTime.Now));
					cmd.Parameters.AddWithValue("@EventCategory", eventCategory);
					cmd.Parameters.AddWithValue("@EventType", eventType);
					cmd.Parameters.AddWithValue("@UserName", userName);
					cmd.Parameters.AddWithValue("@Description", description);
					cmd.Parameters.AddWithValue("@BatchNumber", batchNumber ?? "");
					cmd.Parameters.AddWithValue("@AlarmCode", alarmCode ?? "");
					cmd.Parameters.AddWithValue("@StationId", "HMI-01");
					cmd.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("AuditTrail Log Error: " + ex.Message);
			}
		}

		/// <summary>
		/// Get alarms for a specific batch
		/// </summary>
		public DataTable GetAlarmsForBatch(string batchNumber)
		{
			DataTable dt = new DataTable();

			try
			{
				using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + DB_PATH + ";Version=3;"))
				{
					conn.Open();

					string sql = "SELECT " 
						+ "datetime(TimestampUtc/1000, 'unixepoch', 'localtime') as Timestamp" 
						+ ", EventType, Description, UserName, AlarmCode" 
						+ " FROM AuditTrail" 
						+ " WHERE EventCategory = 'Alarms'" 
						+ " AND BatchNumber = @BatchNumber" 
						+ " ORDER BY TimestampUtc ASC";

					using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
					{
						cmd.Parameters.AddWithValue("@BatchNumber", batchNumber);

						using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
						{
							adapter.Fill(dt);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error loading alarms: " + ex.Message);
			}

			return dt;
		}

		/// <summary>
		/// Convert DateTime to Unix timestamp (milliseconds)
		/// </summary>
		public long DateTimeToUnixMs(DateTime dt)
		{
			return (long)dt.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
		}

		/// <summary>
		/// Convert Unix timestamp to DateTime
		/// </summary>
		public DateTime UnixMsToDateTime(long unixMs)
		{
			return new DateTime(1970, 1, 1).AddMilliseconds(unixMs);
		}
	}
}
