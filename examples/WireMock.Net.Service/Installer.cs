// Copyright © WireMock.Net

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace Wiremock.Net.Service
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }
    }
}
