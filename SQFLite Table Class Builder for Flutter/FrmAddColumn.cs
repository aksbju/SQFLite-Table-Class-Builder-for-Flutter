using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQFLite_Table_Class_Builder_for_Flutter
{
    public partial class FrmAddColumn : Form
    {
        public string ColumnName;
        public string DataType;
        public FrmAddColumn()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.ColumnName = this.tBoxColumnName.Text;
            this.DataType = this.cBoxDataType.Text;
            this.Close();
        }
    }
}
