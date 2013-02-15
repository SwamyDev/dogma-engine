using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace PontAnimator
{
    public partial class MainForm : Form
    {
        public static MainForm Instance;
        public static string SettingsFileName = "settings.xml";

        public int CurrentFrames;
        public int CurrentFramesPerSecond;

        public MainForm()
        {
            Instance = this;
            InitializeComponent();

            // Load settings
            Settings.Instance.Import(SettingsFileName);

            if (Settings.Instance.ContentPath != null)
            {
                Settings.Instance.SetContentPath(Settings.Instance.ContentPath);
                SetContentFolder(Settings.Instance.ContentPath);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        public IntPtr getHandle()
        {
            return showcase.Handle;
        }

        private void showcase_Resize(object sender, EventArgs e)
        {
            if (Animator.Instance != null)
            {
                Animator.Instance.resizebackbuffer(showcase.Width, showcase.Height);
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Save settings before we leave.
            Settings.Instance.Export(SettingsFileName);
            FolderSettings.Instance.Export(Settings.Instance.ContentPath + "\\" + SettingsFileName);
            Animator.Instance.Exit();
        }

        private void btnSelectContentFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog selectFolderDialog = new FolderBrowserDialog();
            selectFolderDialog.SelectedPath = txbContentPath.Text;

            if (selectFolderDialog.ShowDialog() == DialogResult.OK)
            {
                cobAnimations.Items.Clear();
                SetContentFolder(selectFolderDialog.SelectedPath);

                // Cleanup content folder settings if necessary.
                List<string> deleteElemtens = new List<string>();
                foreach (var pair in FolderSettings.Instance.StripeAnimationName)
                {
                    bool found = false;
                    for (int i = 0; i < cobAnimations.Items.Count; i++)
                    {
                        if (pair.Key == (string)cobAnimations.Items[i])
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        deleteElemtens.Add(pair.Key);
                }
                foreach (string key in deleteElemtens)
                {
                    FolderSettings.Instance.StripeAnimationName.Remove(key);
                }
                deleteElemtens.Clear();

                foreach (var pair in FolderSettings.Instance.StripeFrames)
                {
                    bool found = false;
                    for (int i = 0; i < cobAnimations.Items.Count; i++)
                    {
                        if (pair.Key == (string)cobAnimations.Items[i])
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        deleteElemtens.Add(pair.Key);
                }
                foreach (string key in deleteElemtens)
                {
                    FolderSettings.Instance.StripeFrames.Remove(key);
                }
                deleteElemtens.Clear();

                foreach (var pair in FolderSettings.Instance.StripeFramesPerSecond)
                {
                    bool found = false;
                    for (int i = 0; i < cobAnimations.Items.Count; i++)
                    {
                        if (pair.Key == (string)cobAnimations.Items[i])
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        deleteElemtens.Add(pair.Key);
                }
                foreach (string key in deleteElemtens)
                {
                    FolderSettings.Instance.StripeFramesPerSecond.Remove(key);
                }
                deleteElemtens.Clear();

                foreach (var pair in FolderSettings.Instance.StripeColumn)
                {
                    bool found = false;
                    for (int i = 0; i < cobAnimations.Items.Count; i++)
                    {
                        if (pair.Key == (string)cobAnimations.Items[i])
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        deleteElemtens.Add(pair.Key);
                }
                foreach (string key in deleteElemtens)
                {
                    FolderSettings.Instance.StripeColumn.Remove(key);
                }
                deleteElemtens.Clear();
            }
        }

        private void SetContentFolder(string path)
        {
            txbContentPath.Text = path;
            FolderSettings.Instance.Export(Settings.Instance.ContentPath + "\\" + SettingsFileName);
            Settings.Instance.SetContentPath(path);

            // Get .png stripes.
            DirectoryInfo dirContent = new DirectoryInfo(path);
            FileInfo[] fileList = dirContent.GetFiles("*.png");
            
            // Populate combo box.
            for (int i = 0; i < fileList.Length; i++)
            {
                cobAnimations.Items.Add(fileList[i].Name);
            }

            // Set first element as actiave.
            if (cobAnimations.Items.Count > 0)
            {
                cobAnimations.SelectedIndex = 0;
                SetAnimation();
                
                // Load texture files in memory
                if (Animator.Instance != null)
                    Animator.Instance.LoadTexturesFromContentFolder();
            }
        }

        private bool GrabAnimationSettings(out int frames, out int framesPerSecond)
        {
            var stripeName = (string)cobAnimations.SelectedItem;

            if (int.TryParse(txbFramesPerSecond.Text, out framesPerSecond) &&
                int.TryParse(txbFrames.Text, out frames) &&
                CurrentFramesPerSecond > 0
                && CurrentFrames > 0)
            {
                FolderSettings.Instance.SetFrames(frames, stripeName);
                FolderSettings.Instance.SetFramesPerSecond(framesPerSecond, stripeName);

                return true;
            }

            frames = 0;
            framesPerSecond = 0;
            return false;
        }

        private void SetAnimation()
        {
            if (Animator.Instance != null && txbContentPath != null && cobAnimations != null && cobAnimations.Items.Count > 0)
            {
                bool grabSuccessful;
                if (grabSuccessful = GrabAnimationSettings(out CurrentFrames, out CurrentFramesPerSecond))
                {
                    btnPlay.Enabled = true;
                }
                else
                {
                    CurrentFrames = 1;
                    CurrentFramesPerSecond = 1;
                    btnPlay.Enabled = false;
                }

                Animator.Instance.SetAnimation(txbContentPath.Text + "\\" + (string)cobAnimations.SelectedItem, CurrentFrames, CurrentFramesPerSecond);

                // Stop animation if settings are incorrect;
                if (!grabSuccessful)
                    Animator.Instance.StopAnimation();
            }
        }

        private void cobAnimations_SelectedIndexChanged(object sender, EventArgs e)
        {
            var stripeName = (string)cobAnimations.SelectedItem;

            txbFrames.Text = "";
            txbFramesPerSecond.Text = "";

            // If we have stored default values in settings set them accordingly.
            int frames = FolderSettings.Instance.GetFrames(stripeName);
            int fps = FolderSettings.Instance.GetFramesPerSecond(stripeName);
            if (frames > 0 && fps > 0)
            {
                CurrentFrames = frames;
                CurrentFramesPerSecond = fps;

                if (Animator.Instance != null)
                    Animator.Instance.UpdateAnimationSettings(CurrentFrames, CurrentFramesPerSecond);

                txbFrames.Text = CurrentFrames.ToString();
                txbFramesPerSecond.Text = CurrentFramesPerSecond.ToString();
            }

            string animationName = FolderSettings.Instance.GetAnimationName(stripeName);
            if (animationName != string.Empty)
            {
                txbAnimationName.Text = animationName;
            }
            else
            {
                txbAnimationName.Text = stripeName.Split('.')[0];
            }

            int column = FolderSettings.Instance.GetColumn(stripeName);
            if (column >= 0)
            {
                txbColumn.Text = column.ToString();
            }
            else
            {
                txbColumn.Text = string.Empty;
            }

            SetAnimation();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            Animator.Instance.UpdateAnimationSettings(CurrentFrames, CurrentFramesPerSecond);
            Animator.Instance.PlayAnimation();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            Animator.Instance.PauseAnimation();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Animator.Instance.StopAnimation();
        }

        private void txbFramesPerSecond_TextChanged(object sender, EventArgs e)
        {
            int frames; int framesPerSecond;
            if (GrabAnimationSettings(out frames, out framesPerSecond))
            {
                CurrentFrames = frames;
                CurrentFramesPerSecond = framesPerSecond;

                if (Animator.Instance != null)
                    Animator.Instance.UpdateAnimationSettings(CurrentFrames, CurrentFramesPerSecond);
                btnPlay.Enabled = true;
            }
            else
            {
                // Invalid frame width, stop animation and grey out play button and stop animation
                if (Animator.Instance != null)
                    Animator.Instance.StopAnimation();
                btnPlay.Enabled = false;
            }
        }

        private void txbFrames_TextChanged(object sender, EventArgs e)
        {
            int frames; int framesPerSecond;

            if (GrabAnimationSettings(out frames, out framesPerSecond))
            {
                CurrentFrames = frames;
                CurrentFramesPerSecond = framesPerSecond;

                if (Animator.Instance != null)
                    Animator.Instance.UpdateAnimationSettings(CurrentFrames, CurrentFramesPerSecond);
                btnPlay.Enabled = true;
            }
            else
            {
                // Invalid frame width, stop animation and grey out play button and stop animation
                if (Animator.Instance != null)
                    Animator.Instance.StopAnimation();
                btnPlay.Enabled = false;
            }
        }

        private void txbAnimationName_TextChanged(object sender, EventArgs e)
        {
            var stripeName = (string)cobAnimations.SelectedItem;

            FolderSettings.Instance.SetAnimationName(txbAnimationName.Text, stripeName);
        }

        private void txbColumn_TextChanged(object sender, EventArgs e)
        {
            var stripeName = (string)cobAnimations.SelectedItem;

            if (txbColumn.Text == string.Empty)
                return;

            var column = int.Parse(txbColumn.Text);

            if (column >= 0)
            {
                FolderSettings.Instance.SetColumn(column, stripeName);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            // Save all png files which have settings referenced to them to a new folder according to the naming convention.
            Dictionary<string, string> fileNames = new Dictionary<string, string>();
            FolderSettings settings = FolderSettings.Instance;

            prbExport.Visible = true;

            for (int i = 0; i < cobAnimations.Items.Count; i++)
            {
                var stripe = (string)cobAnimations.Items[i];

                if (settings.StripeConfigured(stripe))
                {
                    // Create new file name according to the naming convention.
                    fileNames.Add(stripe, settings.GetAnimationName(stripe) + "_frames_" + settings.GetFrames(stripe).ToString() + "_fps_" + settings.GetFramesPerSecond(stripe).ToString());
                }
                else
                {
                    MessageBox.Show("You have to control and configure all animations before you can export them");
                    return;
                }
            }

            // All files are configured - Now save them to new files.
 
            // Create new folder that contains the images following the naming convention
            string path = Application.StartupPath + "\\SendToBernhard";
            if (Directory.Exists(path))
            {
                // Delete files in directory if already exists.
                DirectoryInfo dirInfo = new DirectoryInfo(path);

                foreach (FileInfo fi in dirInfo.GetFiles())
                {
                    fi.Delete();
                }
            }
            else
            {
                Directory.CreateDirectory(path);
            }

            int totalFiles = fileNames.Count;
            int savedFiles = 0;
            foreach (var pair in fileNames)
            {
                Texture2D texture = Animator.Instance.LoadedTextures[pair.Key];

                // Save to disk
                using (Stream stream = File.OpenWrite(path + "\\" + pair.Value + ".png"))
                {
                    if (stream != null)
                    {
                        texture.SaveAsPng(stream, texture.Width, texture.Height);
                        stream.Close();

                        // Update progress bar.
                        savedFiles++;
                        prbExport.Value = (savedFiles / totalFiles) * 100;
                    }
                }
            }

            // Also save the settings file in the folder
            settings.Export(path + "\\" + SettingsFileName);

            //prbExport.Visible = false;
        }

        private void pnlColorPicker_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                pnlColorPicker.BackColor = colorDialog.Color;
            }
        }

        public Microsoft.Xna.Framework.Color GetBackgroundColor()
        {
            Color curColor = colorDialog.Color;

            var bgColor = new Microsoft.Xna.Framework.Color((int)curColor.R, (int)curColor.G, (int)curColor.B, (int)curColor.A);

            return bgColor;
        }

        private void btnExportSpriteSheet_Click(object sender, EventArgs e)
        {
            if (FolderSettings.Instance.ColumnsDistributed())
            {
                // Create a animation sprite sheet orderd by the column value and save it to the disk.
                Animator.Instance.GetAnimationSheet();
            }
            else
            {
                MessageBox.Show("You need to assign a unique column value to each stripe before you can export to a sprite sheet");
            }
        }
    }
}
