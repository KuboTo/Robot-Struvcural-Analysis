using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RobotOM;

namespace GetStarted
{
    public partial class Form1 : Form
    {
       
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // new object Robot application
            IRobotApplication robApp;
            robApp = new RobotApplicationClass();

            //if (robApp.Project.IsActive == 0)
            //{
            //    // we need an opened project to get names of available supports and bar sections
            //    // create a new project if not connected to existing one
            //    //robApp.Interactive = 1;
            //    //robApp.Visible = 1;
            //    robApp.Project.New(IRobotProjectType.I_PT_SHELL);
            //}

            #region 入口

            // if Robot is not visible
            if (robApp.Visible == 0)
            {
                // set robot visible and allow user interaction
                robApp.Interactive = 1;
                robApp.Visible = 1;
            }
            // create a project concrete beam
            robApp.Project.New(IRobotProjectType.I_PT_SHELL);

            #endregion

            RobotProjectPreferences ProjectPrefs;
            ProjectPrefs = robApp.Project.Preferences;
            ProjectPrefs.SetActiveCode(IRobotCodeType.I_CT_RC_THEORETICAL_REINF, "BAEL91");

            RobotMeshParams MeshParams;
            MeshParams = ProjectPrefs.MeshParams;
            MeshParams.SurfaceParams.Method.Method = IRobotMeshMethodType.I_MMT_DELAUNAY;
            MeshParams.SurfaceParams.Generation.Type = IRobotMeshGenerationType.I_MGT_AUTOMATIC;
            MeshParams.SurfaceParams.Generation.Division1 = 0.5;
            MeshParams.SurfaceParams.Delaunay.Type = IRobotMeshDelaunayType.I_MDT_DELAUNAY;

            //定义几何结构
            //IRobotStructure str;
            //str = robApp.Project.Structure;
            //str.Nodes.Create(1, 0, 0, 0);
            //str.Nodes.Create(2, 3, 0, 0);
            //str.Nodes.Create(3, 3, 3, 0);
            //str.Nodes.Create(4, 0, 3, 0);
            //str.Nodes.Create(5, 0, 0, 4); 
            //str.Nodes.Create(6, 3, 0, 4);
            //str.Nodes.Create(7, 3, 3, 4);
            //str.Nodes.Create(8, 0, 3, 4);

            //str.Bars.Create(1, 1, 5);
            //str.Bars.Create(2, 2, 6);
            //str.Bars.Create(3, 3, 7);
            //str.Bars.Create(4, 4, 8);
            //str.Bars.Create(5, 5, 6);
            //str.Bars.Create(6, 7, 8);

            //Output.AddItem "Structure Generation..."
            IRobotStructure str;
            str = robApp.Project.Structure;
            str.Nodes.Create(1, 0, 0, 0);
            str.Nodes.Create(2, 3, 0, 0);
            str.Nodes.Create(3, 3, 3, 0);
            str.Nodes.Create(4, 0, 3, 0);
            str.Nodes.Create(5, 0, 0, 4);

            str.Nodes.Create(6, 3, 0, 4);
            str.Nodes.Create(7, 3, 3, 4);
            str.Nodes.Create(8, 0, 3, 4);

            str.Bars.Create(1, 1, 5);
            str.Bars.Create(2, 2, 6);
            str.Bars.Create(3, 3, 7);
            str.Bars.Create(4, 4, 8);
            str.Bars.Create(5, 5, 6);
            str.Bars.Create(6, 7, 8);


            RobotLabelServer labels;
            labels = str.Labels;
            string ColumnSectionName = "Rect. Column 30*30";
            IRobotLabel label =
            labels.Create(IRobotLabelType.I_LT_BAR_SECTION, ColumnSectionName);
            RobotBarSectionData section;
            section = (RobotBarSectionData)label.Data;
            section.ShapeType =
            IRobotBarSectionShapeType.I_BSST_CONCR_COL_R;
            RobotBarSectionConcreteData concrete;
            concrete = (RobotBarSectionConcreteData)section.Concrete;

            concrete.SetValue(IRobotBarSectionConcreteDataValue.I_BSCDV_COL_B, 0.3);

            concrete.SetValue(IRobotBarSectionConcreteDataValue.I_BSCDV_COL_H, 0.3);

            section.CalcNonstdGeometry();
            labels.Store(label);
            RobotSelection selectionBars;
            selectionBars = str.Selections.Get(IRobotObjectType.I_OT_BAR);
            selectionBars.FromText("1 2 3 4");
            str.Bars.SetLabel(selectionBars,
            IRobotLabelType.I_LT_BAR_SECTION, ColumnSectionName);
            RobotSectionDatabaseList steelSections;
            steelSections = ProjectPrefs.SectionsActive;

            if (steelSections.Add("RCAT") == 1)
            {
                MessageBox.Show("Steel section base RCAT not found...");
            }
            selectionBars.FromText("5 6");
            label = labels.Create(IRobotLabelType.I_LT_BAR_SECTION, "HEA340");
            str.Labels.Store(label);
            str.Bars.SetLabel(selectionBars,
            IRobotLabelType.I_LT_BAR_SECTION, "HEA 340");
            string MaterialName = "Concrete 30";
            label = labels.Create(IRobotLabelType.I_LT_MATERIAL, MaterialName);

            RobotMaterialData Material;
            Material = (RobotMaterialData)label.Data;
            Material.Type = IRobotMaterialType.I_MT_CONCRETE;
            Material.E = 30000000000; // Young
            Material.NU = 1 / 6; // Poisson 

            Material.RO = 25000; // Unit weight
            Material.Kirchoff = Material.E / (2 * (1 + Material.NU));
            str.Labels.Store(label);
            RobotPointsArray points;
            points =(RobotPointsArray)robApp.CmpntFactory.Create(IRobotComponentType.I_CT_POINTS_ARRAY);

            points.SetSize(5);
            points.Set(1, 0, 0, 4);
            points.Set(2, 3, 0, 4);
            points.Set(3, 3, 3, 4);
            points.Set(4, 0, 3, 4);
            points.Set(5, 0, 0, 4);

            string SlabSectionName = "Slab 30";
            label = labels.Create(IRobotLabelType.I_LT_PANEL_THICKNESS,
            SlabSectionName);
            RobotThicknessData thickness;
            thickness = (RobotThicknessData)label.Data;
            thickness.MaterialName = MaterialName;
            thickness.ThicknessType = IRobotThicknessType.I_TT_HOMOGENEOUS;
            RobotThicknessHomoData thicknessData;
            thicknessData = (RobotThicknessHomoData)thickness.Data;
            thicknessData.ThickConst = 0.3;
            labels.Store(label);

            RobotObjObject slab;
            int ObjNumber = str.Objects.FreeNumber;
            str.Objects.CreateContour(ObjNumber, points);
            slab = (RobotObjObject)str.Objects.Get(ObjNumber);
            slab.Main.Attribs.Meshed = 1;
            slab.SetLabel(IRobotLabelType.I_LT_PANEL_THICKNESS, SlabSectionName);
            slab.Initialize();
            points.Set(1, 1.1, 1.1, 4);
            points.Set(2, 2.5, 1.1, 4);
            points.Set(3, 2.5, 2.5, 4);
            points.Set(4, 1.1, 2.5, 4);
            points.Set(5, 1.1, 1.1, 4);

            RobotObjObject hole;
            int HoleNumber = str.Objects.FreeNumber;
            str.Objects.CreateContour(HoleNumber, points);
            hole = (RobotObjObject)str.Objects.Get(HoleNumber);
            hole.Main.Attribs.Meshed = 0;
            hole.Initialize();
            string FootName = "Foot";
            label = labels.Create(IRobotLabelType.I_LT_SUPPORT, FootName);
            RobotNodeSupportData footData;
            footData = (RobotNodeSupportData)label.Data;
            footData.UX = 1;
            footData.UY = 1;
            footData.UZ = 0;
            footData.KZ = 80000000;
            footData.RX = 1;
            footData.RY = 1;
            footData.RZ = 1;
            labels.Store(label);

            RobotSelection SelectionNodes;
            SelectionNodes = str.Selections.Get(IRobotObjectType.I_OT_NODE);
            SelectionNodes.FromText("1 2 3 4");
            str.Nodes.SetLabel(SelectionNodes, IRobotLabelType.I_LT_SUPPORT, FootName);
            RobotLoadRecord LoadRecord;

            //self weight on entire structure
            RobotSimpleCase caseSW;
            caseSW = str.Cases.CreateSimple(1, "SW", IRobotCaseNature.I_CN_PERMANENT, IRobotCaseAnalizeType.I_CAT_STATIC_LINEAR);
            caseSW.Records.New(IRobotLoadRecordType.I_LRT_DEAD);
            LoadRecord = (RobotLoadRecord)caseSW.Records.Get(1);
            LoadRecord.SetValue(System.Convert.ToInt16(IRobotDeadRecordValues.I_DRV_Z), -1);
            LoadRecord.SetValue(System.Convert.ToInt16(IRobotDeadRecordValues.I_DRV_ENTIRE_STRUCTURE), 0);

            //contour live load on the slab
            RobotSimpleCase CaseLive;
            CaseLive = str.Cases.CreateSimple(2, "Live", IRobotCaseNature.I_CN_EXPLOATATION, IRobotCaseAnalizeType.I_CAT_STATIC_LINEAR);
            int Uniform = CaseLive.Records.New(IRobotLoadRecordType.I_LRT_UNIFORM);
            LoadRecord = (RobotLoadRecord)CaseLive.Records.Get(Uniform);

            LoadRecord.SetValue(System.Convert.ToInt16(IRobotUniformRecordValues.I_URV_PX), 0);

            LoadRecord.SetValue(System.Convert.ToInt16(IRobotUniformRecordValues.I_URV_PY), 0);

            LoadRecord.SetValue(System.Convert.ToInt16(IRobotUniformRecordValues.I_URV_PZ), -10000);


            //apply created load to the slab

            LoadRecord.Objects.FromText(System.Convert.ToString(ObjNumber));
            //linear wind load on the beam
            RobotSimpleCase CaseWind;
            CaseWind = str.Cases.CreateSimple(3, "Wind",IRobotCaseNature.I_CN_WIND, IRobotCaseAnalizeType.I_CAT_STATIC_LINEAR);
            Uniform =CaseWind.Records.New(IRobotLoadRecordType.I_LRT_BAR_UNIFORM);
            LoadRecord = (RobotLoadRecord)CaseWind.Records.Get(Uniform);

            LoadRecord.SetValue(System.Convert.ToInt16(IRobotUniformRecordValues.I_URV_PX), 0);

            LoadRecord.SetValue(System.Convert.ToInt16(IRobotUniformRecordValues.I_URV_PY), 1000); 

            LoadRecord.SetValue(System.Convert.ToInt16(IRobotUniformRecordValues.I_URV_PZ), 0);

            //apply created load to the beam
            LoadRecord.Objects.FromText("5");
            RobotCalcEngine CalcEngine = robApp.Project.CalcEngine;
            CalcEngine.GenerationParams.GenerateNodes_BarsAndFiniteElems = true;
            //CalcEngine.UseStatusWindow = true;
            this.Activate();

            if (CalcEngine.Calculate() == 1)
            {
                MessageBox.Show("Calculation Failed!", "Calculations");
            }
            else
            {
                MessageBox.Show("Done!", "Calculations");
            }
            CalcEngine = null;

            RConcrCalcEngine concrCalcEngine;
            concrCalcEngine = robApp.Project.ConcrReinfEngine;

            RConcrSlabRequiredReinfEngine concrSlabRequiredReinfEngine;
            concrSlabRequiredReinfEngine = concrCalcEngine.SlabRequiredReinf;

            RConcrSlabRequiredReinfCalcParams slabRnfParams;
            slabRnfParams = concrSlabRequiredReinfEngine.Params;

            slabRnfParams.Method = IRobotReinforceCalcMethods.I_RCM_WOOD_ARMER;
            slabRnfParams.GloballyAvgDesginForces = false;
            slabRnfParams.ForcesReduction = false;
            slabRnfParams.DisplayErrors = false;
            slabRnfParams.CasesULS.FromText("1 2 3 4 5 6 7 8");

            RobotSelection slabs;
            slabs = slabRnfParams.Panels;

            slabs.FromText(System.Convert.ToString(ObjNumber));

            string SlabReinforcementName = "Slab X";
            label = labels.Create(IRobotLabelType.I_LT_PANEL_REINFORCEMENT, SlabReinforcementName);
            labels.Store(label);
            slab.SetLabel(IRobotLabelType.I_LT_PANEL_REINFORCEMENT, SlabReinforcementName);
            slab.Update();
            this.Activate();

            if (!concrSlabRequiredReinfEngine.Calculate())
            {
                MessageBox.Show( "Calculation Failed!", "Concrete Calculations");
            }
            else
            {
                MessageBox.Show("Done!", "Concrete Calculations");
            }

            //getting results My and Yz for beam (bar 5) with live load(case 2)
            string txt;
            txt = "Bar 5, Live at 0.5 length:" + "\n\r" + 
                " My = " + str.Results.Bars.Forces.Value(5, 2, 0.5).MY / 1000 + " [kN*m]" + "\n\r" + 
                " Qz = " + -str.Results.Bars.Deflections.Value(5, 2, 0.5).UZ * 1000 + " [mm]" + "\n\r" + 
                " Fz1 = " + str.Results.Bars.Forces.Value(5, 2, 0).FZ / 1000 + " [kN]" + "\n\r" +
                " Fz2 = " + str.Results.Bars.Forces.Value(5, 2, 1).FZ / 1000 + " [kN]" + "\n\r";
            //getting results Fx and Fy for column (bar 4) with wind load(case 3)
            txt += "Bar 4, Wind:" + "\n\r" +
                " Fx = " + str.Results.Bars.Forces.Value(4, 3, 1).FX / 1000 + " [kN]"+ "\n\r" +
                " Fy = " + str.Results.Bars.Forces.Value(4, 3, 1).FY / 1000 + " [kN]"+ "\n\r";
            //getting results Fx, Fy, Fz, Mx, My, Mz for foot (node 1) withself-weight (case 1)
            txt += "Node 1, Self-Weight:" + "\n\r" +
                " Fx = " + str.Results.Nodes.Reactions.Value(1, 1).FX / 1000 + " [kN]" + "\n\r" +
                " Fy = " + str.Results.Nodes.Reactions.Value(1, 1).FY / 1000 + " [kN]" + "\n\r" +
                " Fz = " + str.Results.Nodes.Reactions.Value(1, 1).FZ / 1000 + " [kN]" + "\n\r" +
                " Mx = " + str.Results.Nodes.Reactions.Value(1, 1).MX / 1000 + " [kN]" + "\n\r" +
                " My = " +str.Results.Nodes.Reactions.Value(1, 1).MY / 1000 + " [kN]" + "\n\r" +
                " Mz = " +str.Results.Nodes.Reactions.Value(1, 1).MZ / 1000 + " [kN]" + "\n\r" ;

            //getting results Ax+, Ax-, Ay+, Ay- for slab
            RobotSelection SelectionFE;
            SelectionFE =
            str.Selections.Get(IRobotObjectType.I_OT_FINITE_ELEMENT);
            SelectionFE.FromText(slab.FiniteElems);
            RobotLabelCollection ObjFEs;
            ObjFEs =
            (RobotLabelCollection)str.FiniteElems.GetMany(SelectionFE);
            double AxP;
            double AxM;
            double AyP;
            double AyM;
            double A;
            A = 0;
            AxP = 0;
            AxM = 0;
            AyP = 0;
            AyM = 0;

            RobotFiniteElement FE;
            for (int n = 1; n <= ObjFEs.Count; n++)
            {
                FE = (RobotFiniteElement)ObjFEs.Get(n);
                A = str.Results.FiniteElems.Reinforcement(slab.Number,
                FE.Number).AX_BOTTOM;
                if (A > AxM)
                    AxM = A;
                A = str.Results.FiniteElems.Reinforcement(slab.Number,
                FE.Number).AX_TOP;
                if (A > AxP)
                    AxP = A;
                A = str.Results.FiniteElems.Reinforcement(slab.Number, FE.Number).AY_BOTTOM;
                if (A > AyM)
                    AyM = A;
                A = str.Results.FiniteElems.Reinforcement(slab.Number,
                FE.Number).AY_TOP;
                if (A > AyP)
                    AyP = A;
            }
            //getting results Fx, Fy, Fz, Mx, My, Mz for foot (node 1) withself-weight (case 1)
            txt += "Slab 1, Reinforcemet extreme values:" + "\n\r" +
                " Ax+ = " + AxP * 10000 + " [cm2]" + "\n\r" +
                " Ax- = " + AxM * 10000 + " [cm2]" + "\n\r" +
                " Ay+ = " + AyP * 10000 + " [cm2]" + "\n\r" +
                " Ay- = " + AyM * 10000 + " [cm2]" + "\n\r";
            this.Activate();
            MessageBox.Show(txt, "Results");

        }
    }
}
