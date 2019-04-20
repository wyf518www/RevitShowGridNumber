using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShowGridNumber
{
    public partial class SettingDlg : Form
    {
        private Autodesk.Revit.DB.Color m_Color = new Autodesk.Revit.DB.Color(255, 0, 0);              //颜色
        public SettingDlg()
        {
            InitializeComponent();
        }

        private void btn_Switcher_Click(object sender, EventArgs e)
        {
            ShowGridApplication.m_ThisApp.Switcher();
        }

        private void btn_Color_Click(object sender, EventArgs e)
        {
            ColorDialog loColorForm = new ColorDialog();
            if (loColorForm.ShowDialog() == DialogResult.OK)
            {
                System.Drawing.Color loResultColor = loColorForm.Color;
                btn_Color.ForeColor = loResultColor;                
            }
            System.Drawing.Color color = btn_Color.ForeColor;
            ShowGridApplication.m_gridNumberShowForm.fontColor = color;
            ShowGridApplication.m_gridNumberShowForm.DrawGridNumText();
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            
            ShowGridApplication.m_gridNumberShowForm.fontSize = Convert.ToInt32(numericUpDown.Value);
            ShowGridApplication.m_gridNumberShowForm.DrawGridNumText();
        }

        private void SettingDlg_Load(object sender, EventArgs e)
        {

            numericUpDown.Value = Convert.ToDecimal(ShowGridApplication.m_gridNumberShowForm.fontSize);
            btn_Color.ForeColor = ShowGridApplication.m_gridNumberShowForm.fontColor;
        }

    }
}
