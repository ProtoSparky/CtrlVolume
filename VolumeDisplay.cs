using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CtrlVolume
{
    public partial class VolumeDisplay : Form
    {
        public VolumeDisplay()
        {
            InitializeComponent();
            volumeBar.Value = 0;

        }
        public void setVolume(int val = 50)
        {
            volumeBar.Value = val;
        }
    }
}
