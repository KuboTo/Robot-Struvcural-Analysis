using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RobotOM;

namespace Simply_supported_beam
{
    public partial class Form1 : Form
    {
        // main reference to Robot Application object
        private IRobotApplication robot;


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // connect to Robot
            robot = new RobotApplicationClass();

            if (robot.Project.IsActive == 0)
            {
                // we need an opened project to get names of available supports and bar sections
                // create a new project if not connected to existing one
                robot.Project.New(IRobotProjectType.I_PT_FRAME_2D);
            }

            //RobotApplication robot = null;


            //定义节点
            robot.Project.Structure.Nodes.Create(1,0,0,0);
            robot.Project.Structure.Nodes.Create(2, 3, 0, 0);
            //创建杆件
            robot.Project.Structure.Bars.Create(1, 1, 2);

            //定义约束条件
            IRobotLabel label01 = null;
            label01 = robot.Project.Structure.Labels.Create(IRobotLabelType.I_LT_SUPPORT, "Support");

            RobotNodeSupportData SupportData;
            SupportData = label01.Data as RobotNodeSupportData;
            SupportData.UX = 1;
            SupportData.UY = 1;
            SupportData.UZ = 1;
            SupportData.RX = 0;
            SupportData.RY = 0;
            SupportData.RZ = 0;

            robot.Project.Structure.Labels.Store(label01);
            robot.Project.Structure.Nodes.Get(1).SetLabel(IRobotLabelType.I_LT_SUPPORT, "Support");
            robot.Project.Structure.Nodes.Get(2).SetLabel(IRobotLabelType.I_LT_SUPPORT, "Support");

            //定义杆件截面
            IRobotLabel label02;
            label02 = robot.Project.Structure.Labels.Create(IRobotLabelType.I_LT_BAR_SECTION, "Beam 50X50");
            RobotBarSectionData section;
            section = label02.Data as RobotBarSectionData;
            section.ShapeType = IRobotBarSectionShapeType.I_BSST_CONCR_BEAM_RECT;

            RobotBarSectionConcreteData concrete;
            concrete = section.Concrete;
            concrete.SetValue(IRobotBarSectionConcreteDataValue.I_BSCDV_BEAM_B, 0.5);
            concrete.SetValue(IRobotBarSectionConcreteDataValue.I_BSCDV_BEAM_H, 0.5);
            section.CalcNonstdGeometry();
            robot.Project.Structure.Labels.Store(label02);
            robot.Project.Structure.Bars.Get(1).SetLabel(IRobotLabelType.I_LT_BAR_SECTION, "Beam 50X50");

            //定义工况
            RobotSimpleCase caseSW;
            caseSW = robot.Project.Structure.Cases.CreateSimple(1, "SW", IRobotCaseNature.I_CN_PERMANENT, IRobotCaseAnalizeType.I_CAT_STATIC_LINEAR);
            caseSW.Records.New(IRobotLoadRecordType.I_LRT_DEAD);

            IRobotLoadRecord loadRec;
            loadRec = caseSW.Records.Get(1);
            loadRec.SetValue((short)IRobotDeadRecordValues.I_DRV_Z, -1);
            loadRec.SetValue((short)IRobotDeadRecordValues.I_DRV_ENTIRE_STRUCTURE, (double)1);


            //计算并输出结果
            //if (robot.Project.CalcEngine.Calculate() != null)
            //{
            //    MessageBox.Show(robot.Project.Structure.Results.Nodes.Reactions.Value(1, 1).ToString());
            //}

            if (robot.Project.Structure.Results.Available == 0)
            {
                robot.Project.CalcEngine.Calculate();
            }
            else
            {
                IRobotReactionData ireact = robot.Project.Structure.Results.Nodes.Reactions.Value(1, 1);
                IRobotDisplacementData idisp = robot.Project.Structure.Results.Nodes.Displacements.Value(1, 1);


                string reactionFX = ireact.FY.ToString("####0.00");
                MessageBox.Show(reactionFX);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // connect to Robot
            robot = new RobotApplicationClass();

            if (robot.Project.IsActive == 0)
            {
                // we need an opened project to get names of available supports and bar sections
                // create a new project if not connected to existing one
                robot.Project.New(IRobotProjectType.I_PT_FRAME_2D);
            }

            //RobotApplication robot = null;


            //定义节点
            robot.Project.Structure.Nodes.Create(1, 0, 0, 0);
            robot.Project.Structure.Nodes.Create(2, 3, 0, 0);
            //创建杆件
            robot.Project.Structure.Bars.Create(1, 1, 2);

            //定义约束条件
            IRobotLabel label01 = null;
            label01 = robot.Project.Structure.Labels.Create(IRobotLabelType.I_LT_SUPPORT, "Support");

            RobotNodeSupportData SupportData;
            SupportData = label01.Data as RobotNodeSupportData;
            SupportData.UX = 1;
            SupportData.UY = 1;
            SupportData.UZ = 1;
            SupportData.RX = 0;
            SupportData.RY = 0;
            SupportData.RZ = 0;

            robot.Project.Structure.Labels.Store(label01);
            robot.Project.Structure.Nodes.Get(1).SetLabel(IRobotLabelType.I_LT_SUPPORT, "Support");
            robot.Project.Structure.Nodes.Get(2).SetLabel(IRobotLabelType.I_LT_SUPPORT, "Support");

            //定义杆件截面
            IRobotLabel label02;
            label02 = robot.Project.Structure.Labels.Create(IRobotLabelType.I_LT_BAR_SECTION, "Beam 50X50");
            RobotBarSectionData section;
            section = label02.Data as RobotBarSectionData;
            section.ShapeType = IRobotBarSectionShapeType.I_BSST_CONCR_BEAM_RECT;

            RobotBarSectionConcreteData concrete;
            concrete = section.Concrete;
            concrete.SetValue(IRobotBarSectionConcreteDataValue.I_BSCDV_BEAM_B, 0.5);
            concrete.SetValue(IRobotBarSectionConcreteDataValue.I_BSCDV_BEAM_H, 0.5);
            section.CalcNonstdGeometry();
            robot.Project.Structure.Labels.Store(label02);
            robot.Project.Structure.Bars.Get(1).SetLabel(IRobotLabelType.I_LT_BAR_SECTION, "Beam 50X50");

            //定义工况
            RobotSimpleCase caseSW;
            caseSW = robot.Project.Structure.Cases.CreateSimple(1, "SW", IRobotCaseNature.I_CN_PERMANENT, IRobotCaseAnalizeType.I_CAT_STATIC_LINEAR);
            caseSW.Records.New(IRobotLoadRecordType.I_LRT_DEAD);

            IRobotLoadRecord loadRec;
            loadRec = caseSW.Records.Get(1);
            loadRec.SetValue((short)IRobotDeadRecordValues.I_DRV_Z, -1);
            loadRec.SetValue((short) IRobotDeadRecordValues.I_DRV_ENTIRE_STRUCTURE, (double) 1);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // connect to Robot
            robot = new RobotApplicationClass();

            if (robot.Project.IsActive == 0)
            {
                // we need an opened project to get names of available supports and bar sections
                // create a new project if not connected to existing one
                robot.Project.New(IRobotProjectType.I_PT_FRAME_2D);
            }

            //RobotApplication robot = null;


            //定义节点
            robot.Project.Structure.Nodes.Create(1, 0, 0, 0);
            robot.Project.Structure.Nodes.Create(2, 3, 0, 0);
            //创建杆件
            robot.Project.Structure.Bars.Create(1, 1, 2);

            //定义约束条件
            IRobotLabel label01 = null;
            label01 = robot.Project.Structure.Labels.Create(IRobotLabelType.I_LT_SUPPORT, "Support");

            RobotNodeSupportData SupportData;
            SupportData = label01.Data as RobotNodeSupportData;
            SupportData.UX = 1;
            SupportData.UY = 1;
            SupportData.UZ = 1;
            SupportData.RX = 0;
            SupportData.RY = 0;
            SupportData.RZ = 0;

            robot.Project.Structure.Labels.Store(label01);
            robot.Project.Structure.Nodes.Get(1).SetLabel(IRobotLabelType.I_LT_SUPPORT, "Support");
            robot.Project.Structure.Nodes.Get(2).SetLabel(IRobotLabelType.I_LT_SUPPORT, "Support");

            //定义杆件截面
            IRobotLabel label02;
            label02 = robot.Project.Structure.Labels.Create(IRobotLabelType.I_LT_BAR_SECTION, "Beam 50X50");
            RobotBarSectionData section;
            section = label02.Data as RobotBarSectionData;
            section.ShapeType = IRobotBarSectionShapeType.I_BSST_CONCR_BEAM_RECT;

            RobotBarSectionConcreteData concrete;
            concrete = section.Concrete;
            concrete.SetValue(IRobotBarSectionConcreteDataValue.I_BSCDV_BEAM_B, 0.5);
            concrete.SetValue(IRobotBarSectionConcreteDataValue.I_BSCDV_BEAM_H, 0.5);
            section.CalcNonstdGeometry();
            robot.Project.Structure.Labels.Store(label02);
            robot.Project.Structure.Bars.Get(1).SetLabel(IRobotLabelType.I_LT_BAR_SECTION, "Beam 50X50");

            //定义工况
            
            //加横载
            RobotSimpleCase caseSW;
            caseSW = robot.Project.Structure.Cases.CreateSimple(1, "SW", IRobotCaseNature.I_CN_PERMANENT, IRobotCaseAnalizeType.I_CAT_STATIC_LINEAR);
            caseSW.Records.New(IRobotLoadRecordType.I_LRT_DEAD);

            IRobotLoadRecord loadRec;
            loadRec = caseSW.Records.Get(1);
            loadRec.SetValue((short)IRobotDeadRecordValues.I_DRV_Z, -1);
            loadRec.SetValue((short)IRobotDeadRecordValues.I_DRV_ENTIRE_STRUCTURE, (double)1);

            //加活载
            RobotSimpleCase caseLL;
            caseLL = robot.Project.Structure.Cases.CreateSimple(2, "LL", IRobotCaseNature.I_CN_PERMANENT, IRobotCaseAnalizeType.I_CAT_STATIC_LINEAR);
            caseLL.Records.New(IRobotLoadRecordType.I_LRT_BAR_UNIFORM);

            IRobotLoadRecord loadRecLL;
            loadRecLL = caseLL.Records.Get(2);
            loadRecLL.SetValue((short)IRobotBarUniformRecordValues.I_BURV_PZ, -1);
            //loadRecLL.SetValue((short)IRobotBarUniformRecordValues.I_BURV_RELATIVE, (double)1);
        }
    }
}
