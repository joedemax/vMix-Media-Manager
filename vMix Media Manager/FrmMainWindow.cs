using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Forms;

namespace vMix_Media_Manager
{
    public partial class FrmMainWindow : MetroForm
    {
        vMixMediaList vMixMediaList;

        public FrmMainWindow()
        {
            InitializeComponent();
            MetroStyleManager metroStyleManager = new MetroStyleManager();
            metroStyleManager.Theme = MetroThemeStyle.Dark;
            metroStyleManager.Owner = this;

            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                Title = "Open vMix project",

                CheckFileExists = true,
                CheckPathExists = true,

                Filter = "vmix files (*.vmix)|*.vmix",
                RestoreDirectory = true,
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                vMixMediaList = new vMixMediaList();
                vMixMediaList.Open(openFileDialog1.FileName);
                setDgv();
                btnSave.Enabled = true;
            }

        }


        private void setDgv()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = vMixMediaList.Inputs;

            DataGridViewColumn column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "Name";
            column.Name = "Input Name";
            column.ReadOnly = true;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            column.HeaderCell.Style.BackColor = Color.FromArgb(17, 17, 17);
            column.HeaderCell.Style.ForeColor = Color.White;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "FileName";
            column.Name = "File Name";
            column.ReadOnly = true;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            column.HeaderCell.Style.BackColor = Color.FromArgb(17, 17, 17);
            column.HeaderCell.Style.ForeColor = Color.White;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "Path";
            column.Name = "Path";
            column.ReadOnly = true;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            column.HeaderCell.Style.BackColor = Color.FromArgb(17, 17, 17);
            column.HeaderCell.Style.ForeColor = Color.White;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.HeaderCell.Style.BackColor = Color.FromArgb(17, 17, 17);
            column.HeaderCell.Style.ForeColor = Color.White;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            column.Width = 50;
            column.DataPropertyName = "Online";
            column.Name = "Online";
            dataGridView1.Columns.Add(column);

            DataGridViewButtonColumn buttonCol = new DataGridViewButtonColumn();
            buttonCol.Text = "Relink Media";
            buttonCol.Name = "Relink Media";
            buttonCol.UseColumnTextForButtonValue = true;
            buttonCol.HeaderText = "";
            dataGridView1.Columns.Add(buttonCol);
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {

                vMixInput vmixInput = (vMixInput)senderGrid.Rows[e.RowIndex].DataBoundItem;
                fixPath(vmixInput);
            }
        }


        private void fixPath(vMixInput input)
        {
            string extension = Path.GetExtension(input.Path);
            string fileDir;

            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Find " + Path.GetFileName(input.Path),

                CheckFileExists = true,
                CheckPathExists = true,

                Filter = extension + "files (*" + extension + ")|*" + extension,
                RestoreDirectory = true,
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                input.SetNewPath(ofd.FileName);
                fileDir = Path.GetDirectoryName(ofd.FileName);


                DialogResult result = MessageBox.Show("Look for other files in this directory?", "vMix Media Manager", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    searchForFilesInDir(fileDir);
                }

            }

        }


        private void searchForFilesInDir(string path)
        {
            Debug.WriteLine(path);

            string[] entries = Directory.GetFileSystemEntries(path, "*", SearchOption.AllDirectories);

            List<string> missingFiles = new List<string>();

            foreach (var filePath in entries)
            {
                missingFiles.Add(Path.GetFileName(filePath));
            }

            foreach (var item in vMixMediaList.Inputs)
            {
                //don't care if it's not missing
                if(!item.Online)
                {
                    //do we have it
                    foreach (var file in entries)
                    {
                        if(Path.GetFileName(file) == Path.GetFileName(item.Path))
                        {
                            item.SetNewPath(file);
                        }
                    }
                }
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            vMixMediaList.Save();
        }

        private void centralControl_Click(object sender, EventArgs e)
        {
            Process.Start("http://centralcontrol.io");
        }

        private void centralControl_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
            Debug.WriteLine("hand");

        }


        private void centralControl_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
            Debug.WriteLine("default");
        }
    }
}
