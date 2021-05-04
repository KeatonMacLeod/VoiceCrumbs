using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace VoiceCrumbs
{
    public partial class Form1 : Form
    {
        private const string openAudio = "open new Type waveaudio alias recsound";

        private const string saveAudio = @"save recsound {0}\{1}.wav";

        private const string recordAudio = "record recsound";

        private const string closeAudio = "close recsound ";

        private const string recordingNameHintText = "Recording name (eg. MyAudioRecording)";

        private const string filePathNameHintText = @"Filepath to save recording (eg. C:\Users\Jeeves\Audio)";

        private const string storedFilepathName = "filePathName.txt";

        private static string executableDirectory;

        [DllImport("winmm.dll")]
        private static extern long mciSendString(
            string command,
            StringBuilder retString,
            int ReturnLenth,
            IntPtr callback);

        public Form1()
        {
            InitializeComponent();
            mciSendString(openAudio, null, 0, IntPtr.Zero);
            button1.Click += new EventHandler(this.button1_Click);
            textBox1.Text = recordingNameHintText;
            textBox2.Text = filePathNameHintText;

            textBox1.GotFocus += new EventHandler(RemoveText);
            textBox1.LostFocus += new EventHandler(AddTextRecordingName);

            textBox2.GotFocus += new EventHandler(RemoveText);
            textBox2.LostFocus += new EventHandler(AddTextFilepathHintName);

            executableDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            // Pre-populate the last directory they stored an audio file in
            if (File.Exists(executableDirectory + "\\" + storedFilepathName))
            {
                textBox2.Text = File.ReadLines(executableDirectory + "\\" + storedFilepathName).First();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mciSendString(recordAudio, null, 0, IntPtr.Zero);
            button2.Click += new EventHandler(this.button2_Click);
            mciSendString(openAudio, null, 0, IntPtr.Zero);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mciSendString(string.Format(saveAudio, textBox2.Text, textBox1.Text), null, 0, IntPtr.Zero);
            mciSendString(closeAudio, null, 0, IntPtr.Zero);

            // Update the user's last used filepath name
            string executableFileLocation = System.Reflection.Assembly.GetEntryAssembly().Location;
            string executableDirectory = Path.GetDirectoryName(executableFileLocation);
            using (StreamWriter sw = new StreamWriter(executableDirectory + "\\" + storedFilepathName))
            {
                sw.WriteLine(textBox2.Text);
            }

            StringCollection files = new StringCollection();
            files.Add(string.Format(@"{0}\{1}.wav", textBox2.Text, textBox1.Text));
            Clipboard.SetFileDropList(files);
        }

        public void RemoveText(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text == recordingNameHintText || ((TextBox)sender).Text == filePathNameHintText)
            {
                ((TextBox)sender).Text = "";
            }
        }

        public void AddTextRecordingName(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
                textBox1.Text = recordingNameHintText;
        }

        public void AddTextFilepathHintName(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
                textBox2.Text = filePathNameHintText;
        }
    }
}
