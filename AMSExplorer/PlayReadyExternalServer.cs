﻿
//----------------------------------------------------------------------- 
// <copyright file="PlayReadyExternalServer.cs" company="Microsoft">Copyright (c) Microsoft Corporation. All rights reserved.</copyright> 
// <license>
// Azure Media Services Explorer Ver. 3.1
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
//  
// http://www.apache.org/licenses/LICENSE-2.0 
//  
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License. 
// </license> 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.WindowsAzure.MediaServices.Client.ContentKeyAuthorization;
using Microsoft.WindowsAzure.MediaServices.Client.DynamicEncryption;

namespace AMSExplorer
{
    public partial class PlayReadyExternalServer : Form
    {
        private readonly string _PlayReadyTestLAURL = "http://playready.directtaps.net/pr/svc/rightsmanager.asmx?PlayRight=1&UseSimpleNonPersistentLicense=1";
        private readonly string _PlayReadyTestKeySeed = "XVBovsmzhP9gRIZxWfFta3VVRPzVEWmJsazEJ46I";
        private bool multiassets;

        public string PlayReadyKeySeed
        {
            get
            {
                return textBoxkeyseed.Text;
            }
            set
            {
                textBoxkeyseed.Text = value;
            }
        }
        public string PlayReadyLAurl
        {
            get
            {
                return textBoxLAurl.Text;
            }
            set
            {
                textBoxLAurl.Text = value;
            }
        }
        public string PlayReadyContentKey
        {
            get
            {
                return textBoxcontentkey.Text;
            }
            set
            {
                textBoxcontentkey.Text = value;
            }
        }
        public Guid? PlayReadyKeyId
        {
            get
            {
                return multiassets ? null : (Guid?)new Guid(textBoxkeyid.Text);
            }
            set
            {
                textBoxkeyid.Text = value.ToString();
            }
        }

        public PlayReadyExternalServer(bool Multiassets, bool DoNotAskURL)
        {
            InitializeComponent();
            this.Icon = Bitmaps.Azure_Explorer_ico;
            multiassets = Multiassets;

            if (multiassets) // batch mode for dyn enc so user can only input the seed
            {
                textBoxkeyid.Enabled = false;
                buttonGenKeyID.Enabled = false;
                textBoxcontentkey.Enabled = false;
                buttongenerateContentKey.Enabled = false;
            }
            if (DoNotAskURL)
            {
                textBoxLAurl.Enabled = false;
                panelPlayReadyTest.Visible = false;
            }
        }

        private void buttonPlayReadyTestSettings_Click(object sender, EventArgs e)
        {
            textBoxLAurl.Text = _PlayReadyTestLAURL;
            textBoxkeyseed.Text = _PlayReadyTestKeySeed;
            if (!multiassets) textBoxkeyid.Text = Guid.NewGuid().ToString();

            textBoxcontentkey.Text = string.Empty;
        }

        private void buttonGenKeyID_Click_1(object sender, EventArgs e)
        {
            textBoxkeyid.Text = Guid.NewGuid().ToString();
        }

        private void moreinfotestserver_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }

        private void PlayReadyExternalServer_Load(object sender, EventArgs e)
        {
            moreinfotestserver.Links.Add(new LinkLabel.Link(0, moreinfotestserver.Text.Length, "http://playready.directtaps.net/"));
        }

        private void buttongenerateContentKey_Click(object sender, EventArgs e)
        {
            textBoxcontentkey.Text = Convert.ToBase64String(DynamicEncryption.GetRandomBuffer(16));
            textBoxkeyseed.Text = string.Empty;
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            bool validation = false;
            if (multiassets) // multi assets selectyed. We need at least the key seed
            {
                if (!string.IsNullOrEmpty(textBoxkeyseed.Text))
                {
                    validation = true;
                }
            }
            else // single asset selected
            {
                if (!string.IsNullOrEmpty(textBoxkeyid.Text) && (!string.IsNullOrEmpty(textBoxkeyseed.Text) || (!string.IsNullOrEmpty(textBoxcontentkey.Text))))
                {
                    validation = true;
                }
            }

            buttonOk.Enabled = validation;
        }

        private void textBoxkeyseed_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxkeyseed.Text)) // if a seed is set, let's make sure no content key is defined
            {
                textBoxcontentkey.Text = string.Empty;
            }
            textBox_TextChanged(sender, e);
        }

        private void textBoxcontentkey_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxcontentkey.Text)) // if a content key, let's make sure no seed is defined
            {
                textBoxkeyseed.Text = string.Empty;
            }
            textBox_TextChanged(sender, e);
        }
    }
}
