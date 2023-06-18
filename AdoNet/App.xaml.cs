using AdoNet.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AdoNet
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string ConnectionString = "server=us-east.connect.psdb.cloud;user=vw3ad7aefas2b0b61pnn;database=adonetdb;port=3306;password=pscale_pw_FErHLZrXaihAdFh81Qp5iOdlaU3aDpbDHiuVaiLSMlA;SslMode=VerifyFull";
        internal static readonly ILogger Logger = new FileLogger();
    }
}
