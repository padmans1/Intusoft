using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using System.Management;
using System.Diagnostics;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using INTUSOFT.Data.NewDbModel;
using Microsoft.Win32;
using System.IO;
using Common;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace INTUSOFT.Data.Repository
{
    public class NHibernateHelper_MySQL
    {
        public static string dbPath = string.Empty;
        public static string userName = "";
        public static string password = "";
        public static string serverPath = "localhost";
        public static string connectionString = string.Empty;//;
        //public static string connectionString = "Server=localhost;Database=intusoftmrs;User ID=root;Password=toor;CharSet=latin1";
        public static string WarningText = "";
        public static string WarningHeader = "";
        public static string dbfilePath = "";// added this variable to be used to get or set db path by sriram on feb 9th 2016 
        public static string dbName = "";// added this variable to be used to get or set db name by sriram on feb 9th 2016
        private static string dbfileNameExt = ".db3";
        private static string createDB = @"SQLs\create_Intsoft_MRS.sql";
        private static string alterDB = @"SQLs\Alter_Intsoft_MRS.sql";
        private static string grantPrivileges = @"SQLs\Grant_All_Previleges.sql";
        private static string change_size_person_attribute_value = @"SQLs\Change_person_atrribute_valueSize.sql";
        private static string observationAttributeTable = "obs_attribute";
        private static string dbConnectionString = "Server=localhost;Database = " + dbName + "; User ID=" + userName + ";Password=" + password + ";CharSet=latin1";
        public static string oldDbName = "IntuNewModel1";
        public static ISession hibernateSession;
        public static string batchFileName = @"BatchScripts\InsertReportTypes.bat";
        public static string eyeFundusImageTableName = "eye_fundus_image";
        public static string maskSettingsColumnName = "mask_settings";
        public static string reportTypeColumnName = "report_type";
        public static IntuSoftRuntimeProperties IntuSoftRuntimeProperties;


        public static ISessionFactory _sessionFactory;

        private static ISessionFactory SessionFactory
        {
           
            get
            {
                {
                    if (_sessionFactory == null)
                    {
                        EvaluateConnectionString();
                        //try
                        {
                            //serverPath = "192.168.0.146";
                            //dbName = "intunewmodel";
                            dbConnectionString = "Server="+serverPath+";Database = " + dbName + "; User ID=" + userName + ";Password=" + password + ";CharSet=latin1";
                            _sessionFactory = Fluently.Configure().Database(MySQLConfiguration.Standard
                                                           .ConnectionString(
                                                               dbConnectionString)
                                             )
                                             .Mappings(m =>
                                                       m.HbmMappings
                                                           .AddFromAssemblyOf<Person>()).ExposeConfiguration(config =>
                                                           {
                                                               config.SetInterceptor(new CustomInterceptor());
                                                           })
                                              .ExposeConfiguration(BuildSchema)
                                             .BuildSessionFactory();
                        }
                        //catch (Exception ex)
                        //{
                        //    int x = 0;
                        //    int i = x;
                        //}
                    }
                    return _sessionFactory;
                }
            }
            set
            {
                _sessionFactory = value;
            }
        }

        public static void EvaluateConnectionString()
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                //var runtimePath = @"C:\Users\Kishore\AppData\Roaming\Intuvision Labs Pvt Ltd\Intusoft-runtime.json";
                ///var data = new Dictionary<string, string>();
                if(string.IsNullOrEmpty(IntuSoftRuntimeProperties.filePath))
                {
                    IntuSoftRuntimeProperties.filePath =  new FileInfo(Application.ExecutablePath).Directory.FullName + Path.DirectorySeparatorChar + "Intusoft - runtime.json";
                }
                if (File.Exists(IntuSoftRuntimeProperties.filePath))
                {
                    //foreach (var row in File.ReadAllLines(runtimePath))
                    //    data.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
                    var data = File.ReadAllText(IntuSoftRuntimeProperties.filePath);
                    IntuSoftRuntimeProperties = (IntuSoftRuntimeProperties)JsonConvert.DeserializeObject(data, typeof(IntuSoftRuntimeProperties));


                }
                else
                {
                    IntuSoftRuntimeProperties = new IntuSoftRuntimeProperties();
                    var data = JsonConvert.SerializeObject(IntuSoftRuntimeProperties);
                    File.WriteAllText(IntuSoftRuntimeProperties.filePath, data);
                }
                dbName = IntuSoftRuntimeProperties.dbName;
                userName = IntuSoftRuntimeProperties.userName;
                password = IntuSoftRuntimeProperties.password;
                serverPath = IntuSoftRuntimeProperties.server_path;
                connectionString = "Server=" + serverPath + ";User ID=" + userName + ";Password=" + password + ";CharSet=latin1";
            }
        }

        public static void CloseSession()
        {
            SessionFactory = null;
        }


        public static void OpenSession()
        {
            if (hibernateSession == null)
            {
                CreateOrAlterDB(string.Empty);
                hibernateSession = SessionFactory.OpenSession();

            }
            else
                if (!hibernateSession.IsOpen)
                    hibernateSession = SessionFactory.OpenSession();
        }


        private static void BuildSchema(NHibernate.Cfg.Configuration config)
        {
            
        }

        public static void CreateOrAlterDB(string runtimePath)
        {
            IntuSoftRuntimeProperties.filePath = runtimePath;
            EvaluateConnectionString();
            if (!DbExists(dbName))// to check whether the db exists if not create a db using sql from the file
                ExecuteSqlScriptFromFile(createDB);
            else
            {
                ExecuteSqlScriptFromFile(alterDB);
            }
            ExecuteSqlScriptFromFile(grantPrivileges);
        }


        public static void ExecuteSqlScriptFromFile(string fileName)
        {
            MySqlConnection connection;
            connection = new MySqlConnection(connectionString);
            connection.Open();
            if (File.Exists(fileName))
            {
                FileInfo file = new FileInfo(fileName);
                string script = file.OpenText().ReadToEnd();
                script = script.Replace("dbName", dbName);//This code will replace the database name in the Intusoft-runtime.properties file to the dbname in create_Inntusoft_MRS.sql file.
                MySqlScript mysqlscript = new MySqlScript(connection, script);
                 mysqlscript.Execute();
                connection.Close();
            }
        }

        public static bool DbExists(string dbName)
        {
            try
            {
                MySqlConnection connection;
                connection = new MySqlConnection(connectionString);
                connection.Open();
                string cmdStr = "show databases";
                MySqlCommand cmd = new MySqlCommand(cmdStr, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                List<string> dbs = new List<string>();
                while (reader.Read())
                {
                    dbs.Add(reader.GetString(0));
                }
                connection.Close();
                if (dbs.Contains(dbName.ToLower()))
                {
                    connectionString = "Server=" + serverPath + ";Database = " + dbName + "; User ID=" + userName + ";Password=" + password + ";CharSet=latin1";
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                DbExists(dbName);//Run till connection is established.
                return false;
            }
        }

       // public static int NoOfRowsOfaTable(string tableName)
       // {
       //     string commandLine = "SELECT COUNT(*) FROM " + tableName + "";
       //     using (MySqlConnection connect = new MySqlConnection(dbConnectionString))
       //     using (MySqlCommand cmd = new MySqlCommand(commandLine, connect))
       //     {
       //         connect.Open();
       //         int count = 0;
       //          count = Convert.ToInt32(cmd.ExecuteScalar());
       //         return count;
       //     }
       // }

       // public static void ExecuteBatchFile(string batchFileName)
       // {
       //     if (File.Exists(batchFileName))
       //     {
       //         Process proc = new Process();
       //         proc.StartInfo.FileName = batchFileName;
       //         proc.StartInfo.CreateNoWindow = false;
       //         proc.Start();
       //         proc.WaitForExit(5000);
       //     }
       // }

       //public static bool CheckForMySQLServer()
       //{
       //    string query = "SELECT Name FROM Win32_Product WHERE Name LIKE '%MySQL Server%'";
       //    var searcher = new ManagementObjectSearcher(query);
       //    var collection = searcher.Get();
       //    CustomMessageBox.Show(collection.Count.ToString());
       //    return collection.Count > 0;
       //}
    }
}
