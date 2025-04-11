using System;
using System.Collections.Generic;
using Network;
using Network.Messages;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EventLog : MonoBehaviour
    {
        [SerializeField] private Text text;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            List<string> log = new();

            Dictionary<string,int> svLog = (NetworkManager.Instance as ServerNetManager)?.log;
            
            if (svLog != null)
            {
                foreach (KeyValuePair<string, int> entry in svLog)
                    log.Add(entry.Value > 1 ? $"{entry.Key} x{entry.Value}" : entry.Key);
            }

            text.text = "";

            foreach (string displayLogMessage in log)
                text.text += displayLogMessage + Environment.NewLine;
        }
    }
}