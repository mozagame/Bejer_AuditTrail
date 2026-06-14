//--------------------------------------------------------------
// AlarmScreen.Script.cs
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
	using System.Windows.Forms;
	using System;
	using System.Drawing;
	using Neo.ApplicationFramework.Tools;
	using Neo.ApplicationFramework.Common.Graphics.Logic;
	using Neo.ApplicationFramework.Controls;
	using Neo.ApplicationFramework.Interfaces;
    
    
	public partial class AlarmScreen
	{
		// === THÊM: Declare audit module instance ===
		ScriptModuleAuditTrail audit = new ScriptModuleAuditTrail();

		// === THÊM: Handler khi screen load ===
		void AlarmScreen_Opened(System.Object sender, System.EventArgs e)
		{
			// Initialize database on screen load
			audit.Initialize();
		}

		// === GIỮ NGUYÊN: Các handler cũ ===
		void btnAckAll_Click(System.Object sender, System.EventArgs e)
		{
			AlarmViewer1.AcknowledgeAll();	
		}

		void btnAckSel_Click(System.Object sender, System.EventArgs e)
		{
			AlarmViewer1.AcknowledgeSelected();	
		}

		void btnInfo_Click(System.Object sender, System.EventArgs e)
		{
			AlarmViewer1.ShowInfo();
		}

		// === THÊM (optional): Test logging ===
		void btnTestLog_Click(System.Object sender, System.EventArgs e)
		{
			// Test logging 
			audit.LogEvent("Test", "TestEvent", "Admin", "Test log entry 1", "BATCH-001", "");
			audit.LogEvent("Test", "TestEvent", "Admin", "Test log entry 2", "BATCH-001", "ALM-001");
        
			MessageBox.Show("Test logs recorded!");
		}
				
	}
}
