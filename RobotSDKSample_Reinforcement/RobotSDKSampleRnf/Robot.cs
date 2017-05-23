//
// (C) Copyright 2009 by Autodesk, Inc. All rights reserved.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RobotOM;
using bbm;
using butl;

namespace RobotSDKSampleRnf 
{
    public partial class Robot : Form
    {
        
        private IRobotApplication robApp;
        private RCBeam beam ;
        private RCSteel steel;
        private RCBeamGeometry geometry;
        private RCBeamSpan span;
        private RCBeamSegment segment;
        private RCConcrete concrete; 
      
        public Robot()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                SetRobot();
                SetMaterialAndCodeAndCover();

            }
            catch
            {
                MessageBox.Show("Error during initialization");  
            }
 
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                SetGeometry();
                SetCracking();
                SetLoads();
                Calculate();
            }
            catch
            {
                MessageBox.Show("Error during generation or calculation");  
            }
        }

        private void cboCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SetMaterialAndCodeAndCover();
            }
            catch
            {
                MessageBox.Show("Error during parameters set","Robot Reinforcement");  
            }
        }
        /// <summary>
        /// This method allow user to 
        /// </summary>
        private void SetMaterialAndCodeAndCover() 
        {
            robApp.Project.Preferences.SetActiveCode(IRobotCodeType.I_CT_RC_REAL_REINF, cboCode.Text);
            // Get steel charateristics for longitunal and transversal rebars
            // Internal units are SI. Stress in in Pascal and UI is in MPa
            steel = (RCSteel)beam.Steel;
            editSteelStandard.Text = System.Convert.ToString(steel.get_CharacteristicStrength(IRCSteelType.I_RC_STEEL_TYPE_LONG) / 1000000);
            editSteelStirrup.Text = System.Convert.ToString(steel.get_CharacteristicStrength(IRCSteelType.I_RC_STEEL_TYPE_TRANS) / 1000000);

            // Get beam charateristics 
            concrete = (RCConcrete)beam.Concrete;
            editConcrete.Text = System.Convert.ToString(concrete.CharacteristicStrength / 1000000);
        
            // Get calculation option
            editCoverVertical.Text = System.Convert.ToString(beam.CalculationOptions.CoverBottom*100);
            editCoverLateral.Text = System.Convert.ToString(beam.CalculationOptions.CoverSide  * 100);
        }

        private void SetRobot()
        {
            // new object Robot application
            robApp = new RobotApplicationClass();

            // if Robot is not visible
            if (robApp.Visible == 0)
            {
                // set robot visible and allow user interaction
                robApp.Interactive = 1;
                robApp.Visible = 1;
            }

            // create a project concrete beam
            robApp.Project.New(IRobotProjectType.I_PT_CONCRETE_BEAM);

            // search an active beam to modify on the project manager 
            // per default a beam is present 
            beam = GetActiveBeam();

            // select the first index of the list of design code
            // and set it active
            // cboCode is filled with these design codes:
            //              ACI 318-08
            //              ACI 318-08 metric
            //              BAEL 91 mod. 99
            //              BS 8110
            //              EHE 99
            //              EN 1992-1-1:2004 AC:2008
            //              IS 456 : 2000
            //              NBN B 15-002:1995
            //              NS 3473:2003
            //              PN-EN 1992-1-1:2008
            //              SFS-EN 1992-1-1
            //              SNiP 2.03.01-84
            //              STAS 10107/0-90

            cboCode.SelectedIndex = 1;
            CboCrack.SelectedIndex = 1; 
 
            editSupportLeft.Text= System.Convert.ToString(0.3);  
            editSupportRight.Text= System.Convert.ToString(0.3);  
            editWidth.Text = System.Convert.ToString(0.3);  
            editHeight.Text= System.Convert.ToString(0.5);  
                
                
        }

        private void SetGeometry()
        {
            // Set beam parameters 
            geometry = beam.Geometry;
            // Allow automatic spans naming
            geometry.AutoNameSpans = true;
            // Allow automatic supports naming
            geometry.AutoNameSupports = true;
            // Set number of spans
            geometry.SpanNumber = System.Convert.ToInt16(editNbSpan.Text);
            // Disable cantilever at the start and end of beam
            geometry.LeftCantilever = false;
            geometry.RightCantilever = false;

            // Set dimension to the beam spans
            for (short lspan = 1; lspan <= (short)geometry.SpanNumber; lspan++)
            {
                span = geometry.get_Span(lspan);
                // Set span length
                span.Length = System.Convert.ToDouble(editLength.Text);
                // Set support dimensions
                   span.LeftSupport.Width = System.Convert.ToDouble(editSupportLeft.Text);
                   span.RightSupport.Width = System.Convert.ToDouble(editSupportRight.Text);
                // Set a rectangular section to the span 
                segment = span.get_Segment(1);
                segment.SectionType = IRCBeamSectionType.I_RC_BEAM_NG_NP;
                segment.set_Dim(IRCBeamSectionDim.I_RC_BEAM_B, System.Convert.ToDouble(editWidth.Text));
                segment.set_Dim(IRCBeamSectionDim.I_RC_BEAM_H1, System.Convert.ToDouble(editHeight.Text));
                segment.set_Dim(IRCBeamSectionDim.I_RC_BEAM_H2, System.Convert.ToDouble(editHeight.Text));
            }
        }

        private void SetCracking()
        {
            //  Set cracking type 
            switch (CboCrack.SelectedIndex)
            {
                case 1:
                    beam.StoryOptions.Cracking = bbm.IRCBeamCrackingType.I_RC_BEAM_CRACK_PERMISSIBLE;
                    break;
                case 2:
                    beam.StoryOptions.Cracking = bbm.IRCBeamCrackingType.I_RC_BEAM_CRACK_LIMITED;
                    break;
                case 3:
                    beam.StoryOptions.Cracking = bbm.IRCBeamCrackingType.I_RC_BEAM_CRACK_NOT_PERMISSIBLE;
                    break;
                default:
                    beam.StoryOptions.Cracking = bbm.IRCBeamCrackingType.I_RC_BEAM_CRACK_UNDEFINED;
                    break;
            }
        }
        
        private void SetLoads()
        {
            
            beam.LinearLoadsCount = 2;
            
            RCBeamLinearLoad SelfWeightLoad = beam.get_LinearLoad(1);
            RCBeamLinearLoad uniformLiveLoad = beam.get_LinearLoad(2);
          
            SelfWeightLoad.Nature = IRCBeamLoadNature.I_RC_BEAM_NAT_DEAD;
            SelfWeightLoad.Type = IRCBeamLinearLoadType.I_RC_BMLL_SELF_WEIGHT;
            
            uniformLiveLoad.Nature = IRCBeamLoadNature.I_RC_BEAM_NAT_LIVE;
            uniformLiveLoad.Type = IRCBeamLinearLoadType.I_RC_BMLL_UNIFORM;   
            uniformLiveLoad.RelativeCoordinates = false;
            uniformLiveLoad.Value1 = System.Convert.ToDouble(editUniformLive.Text) * 1000;
        }

        private void Calculate()
        {
            beam.Calculate(true);
            MessageBox.Show("Calculations completed","Robot Reinforcement"); 
        }

        private RCBeam GetActiveBeam()
        {
            RCBeam mBeam = null;

                RobotProjectComponentMngr componentManager = this.robApp.Project.ComponentMngr;
                int level, beam;
                for (level = 1; level <= componentManager.LevelCount; level++)
                {
                    string levelName = componentManager.GetLevelName(level);
                    IRobotCollection beams = componentManager.Get(IRobotProjectComponentType.I_PCT_BEAM, levelName);

                    for (beam = 1; beam <= beams.Count; beam++)
                    {
                        mBeam = (RCBeam)((IRobotProjectComponent)beams.Get(beam)).Data;

                        if (!mBeam.IsActive) continue;
                        return mBeam;
                    }
                }
                return mBeam;
        }

        private void CboCrack_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SetMaterialAndCodeAndCover();
            }
            catch
            {
                MessageBox.Show("Error during parameter set");
            }
        }

  
     
    }
}
