using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Data.Common;

namespace OnlinePayTools
{


    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            LoadAdonetProvider();
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //MainFrm mf = new MainFrm();
            //Application.Run(mf);
            //Application.DoEvents();
            //while (MainFrm.IsRunning)
            //{
            //    Application.Run(MainFrm.runningForm);
            //}
            //Application.Exit();
            Application.Run(new MainFrm());
        }

        static void LoadAdonetProvider()
        {
            AssemblyName[] assemblyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            foreach (AssemblyName assemblyName in assemblyNames)
            {
                if (assemblyName.Name.StartsWith("Interop."))
                {
                    continue;
                }
                try
                {
                    Assembly.Load(assemblyName);
                }
                catch
                {

                }
            }

            // 加载程序目录下的adonet提供程序
            String path = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            String[] files = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
            foreach (String x in files)
            {
                if (x.StartsWith("Interop."))
                {
                    continue;
                }
                try
                {
                    Assembly.LoadFile(x);
                }
                catch
                {

                }
            }


        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString(), "程序错误");
        }
    }
}
