//--------------------------------------------------------------
// Press F1 to get help about using script.
// To access an object that is not located in the current class, start the call with Globals.
// When using events and timers be cautious not to generate memoryleaks,
// please see the help for more information.
//---------------------------------------------------------------
	
    /*
	*******************************************************************
	All examples in this support document are only intended
	to improve understanding of the functionality and 
	handling of the equipment. 
	Beijer Electronics AB cannot assume any liability if these
	examples are used in real applications.
	In view of the wide range of applications for this 
	equipment, users must acquire sufficient knowledge 
	themselves in order to ensure that it is correctly used in 
	their specific application. Persons responsible for the 
	application and the equipment must themselves 
	ensure that each application is in compliance with all 
	relevant requirements, standards and legislation in 
	respect to configuration and safety.
	Beijer Electronics AB will accept no liability for any 
	damage incurred during the installation or use of this 
	equipment.
	Beijer Electronics AB prohibits all modification, changes
	or conversion of the equipment.
	*******************************************************************
	*/

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
		void btnAckAll_Click(System.Object sender, System.EventArgs e)
		{
			AlarmViewer1.AcknowledgeAll();	
		}
		
		void btnAckSel_Click(System.Object sender, System.EventArgs e)
		{
			AlarmViewer1.AcknowledgeSelected();	
		}
		
		void btnClearAlarms_Click(System.Object sender, System.EventArgs e)
		{
			AlarmViewer1.ClearNormalAlarms();			
		}
		
		void btnInfo_Click(System.Object sender, System.EventArgs e)
		{
			AlarmViewer1.ShowInfo();
		}
		
	
    }

}
