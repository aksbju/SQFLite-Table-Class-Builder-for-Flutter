using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQFLite_Table_Class_Builder_for_Flutter
{
    public partial class FrmApp : Form
    {
        Translator translator;
        List<Column> columns;
        public FrmApp()
        {
            InitializeComponent();
        }

        private void tBoxTable_TextChanged(object sender, EventArgs e)
        {
            if (this.tBoxTable.TextLength > 0)
            {
                this.lblTableFileName.Text = $"{this.tBoxTable.Text.Replace(" ", "_").ToLower()}.dart";
                this.lblTableProviderFileName.Text = $"{this.tBoxTable.Text.Replace(" ", "_").ToLower()}_provider.dart";

                doTranslate();

            }
            else
            {
                this.lblTableFileName.Text = $"table.dart";
                this.lblTableProviderFileName.Text = $"table_provider.dart";
            }
        }

        private void doTranslate()
        {
            this.translator = new Translator(this.tBoxTable.Text, columns, this.cBoxPrimaryKey.SelectedIndex, this.chkBoxAutoIncrement.Checked);
            CodeModel code = translator.Translate();
            this.tBoxTableCode.Text = code.TableCode;
            this.tBoxTableProviderCode.Text = code.TableProviderCode;
        }

        private void btnCopyTable_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.tBoxTableCode.Text ?? "");
            MessageBox.Show("Copied to clipboard", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCopyTableProvider_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.tBoxTableProviderCode.Text ?? "");
            MessageBox.Show("Copied to clipboard", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSaveTable_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();   
            saveFileDialog1.Title = "Save dart Files";
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "dart";
            saveFileDialog1.Filter = "Dart files (*.dart)|*.dart|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileName = this.lblTableFileName.Text;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog1.FileName, this.tBoxTableCode.Text);
            }
        }

        private void btnSaveTableProvider_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();     
            saveFileDialog1.Title = "Save dart Files";
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "dart";
            saveFileDialog1.Filter = "Dart files (*.dart)|*.dart|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileName = this.lblTableProviderFileName.Text;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog1.FileName, this.tBoxTableProviderCode.Text);
            }
        }

        private void FrmApp_Load(object sender, EventArgs e)
        {
            //this.tBoxTable.Text = "table";
            this.columns = new List<Column>();
            this.columns.Add(new SQFLite_Table_Class_Builder_for_Flutter.Column
            {
                ColumnName = "_id",
                DataType = "INTEGER"
            });
            this.dtbColumns.DataSource = this.columns;
            dtbColumns_CellEndEdit(sender, null);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FrmAddColumn frm = new FrmAddColumn();
            frm.ShowDialog();
            if (frm.ColumnName != null && frm.DataType != null)
            {
                this.columns.Add(new SQFLite_Table_Class_Builder_for_Flutter.Column 
                { 
                    ColumnName = frm.ColumnName, DataType = frm.DataType
                });
            }
            this.dtbColumns.DataSource = null;
            this.dtbColumns.DataSource = this.columns;
            dtbColumns_CellEndEdit(sender, null);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {

                this.columns.RemoveAt(this.dtbColumns.CurrentCell.RowIndex);
                this.dtbColumns.DataSource = null;
                this.dtbColumns.DataSource = this.columns;
                dtbColumns_CellEndEdit(sender, null);
            }
            catch (Exception ex){ MessageBox.Show(ex.Message); }
        }

        private void dtbColumns_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            List<string> keys = new List<string>();
            foreach (var each in this.columns)
            {
                keys.Add(each.ColumnName + $"({each.DataType})");
            }
            this.cBoxPrimaryKey.DataSource = keys;
            doTranslate();
            if (this.columns.Count > 0)
            {
                this.cBoxPrimaryKey.SelectedIndex = 0;
            }
            else { this.cBoxPrimaryKey.Text = ""; }
        }

        private void cBoxPrimaryKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.columns[this.cBoxPrimaryKey.SelectedIndex].DataType == "INTEGER")
            {
                this.chkBoxAutoIncrement.Enabled = true;
                this.chkBoxAutoIncrement.Checked = true;
            }
            else
            {
                this.chkBoxAutoIncrement.Enabled = false;
                this.chkBoxAutoIncrement.Checked = false;
            }
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            about _about = new about();
            _about.ShowDialog();
        }
    }
}
