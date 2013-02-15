using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Pontification.Components
{
    public class DialogTrigger : Component
    {
        #region Private attributes
        private PhysicsComponent _sensor;
        private string _dialogText;
        private string _folderName = "Dialogs";
        private string _fileEnding = "txt";
        #endregion

        #region Public properties
        public string DialogFileName { get; set; }
        public int Paragraph { get; set; }
        #endregion

        #region Public methods
        public override void Start()
        {
            // Load dialog file.
            readFromFile();

            // Set up sensor.
            _sensor = GetComponent<PhysicsComponent>();

            if (_sensor == null)
                throw new ArgumentNullException("DialogTrigger needs a physic component attached");
            if (!_sensor.IsSensor)
                throw new ArgumentException("Attached physics component needs to be a sensor");

            _sensor.OnCollisionEnter += onEnter;
        }
        #endregion

        #region Private methods
        private void onEnter(GameObject go)
        {
            if (go == SceneManagement.SceneInfo.Player)
            {   // It's the player so show the dialog.
                SceneManagement.Subtitles.ShowDialog(_dialogText);

                Monitoring.Logger.Instance.Log("Dialog triggered", Monitoring.MessageType.MT_STATISTICS);
            }
        }

        private void readFromFile()
        {
            using (StreamReader sr = new StreamReader(string.Format("{0}/{1}.{2}", _folderName, DialogFileName, _fileEnding)))
            {
                int currentParagraph = 1;
                var text = new StringBuilder();
                // Read file line by line.
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    if (line == string.Empty)
                        currentParagraph++;

                    if (currentParagraph == Paragraph)
                    {
                        text.Append(line);
                    }
                }
                sr.Close();

                _dialogText = text.ToString();
            }
        }
        #endregion
    }
}
