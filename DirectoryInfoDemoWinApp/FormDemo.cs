using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text; 
using System.Windows.Forms;

namespace DirectoryInfoDemo
{
    public partial class FormDemo : Form
    {
        public FormDemo()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string katalognavn = tbxKatalognavn.Text.Trim();

            // Viser brugen af DirectoryInfo til at hente info for et katalog

            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(katalognavn);         // objekt med information for det angive katalognavn
            
            if (dir.Exists)                                                                 // tjek om katalog findes
            {
                textBox2.Text = dir.CreationTime.ToString() + Environment.NewLine;


                System.IO.DirectoryInfo[] subDirs = dir.GetDirectories();                   // Hent underliggende kataloginformationer for det angive katalog 

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    textBox2.Text = textBox2.Text + "Name:<" + dirInfo.Name + "> Extension:<" + dirInfo.Extension +">" + Environment.NewLine;
                }
                

                dataGridView1.DataSource = subDirs;     // vis elementerne i grid formatet for demo af alle properties
            }
            else
                textBox2.Text = "Katalog findes ikke";
        }
    }
}
