using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VMB_new
{
    public class SqliteDataAccess
    {

        public static int n;
        public static int nr_zi = 0;
        
        public static List<om> LoadArchive()
        {
             
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<om>("select * from tabel_sezon", new DynamicParameters());
                cnn.Close();
                return output.ToList();
            }
            

            /* // Introduce date de test
            var list = new List<om>();
            list.Add(new om() { nume = "Test1", prezente = 120 });
            list.Add(new om() { nume = "Test2", prezente = 121 });
            list.Add(new om() { nume = "Test3", prezente = 122 });
            list.Add(new om() { nume = "Test4", prezente = 123 });

            return list;
            */
            
        }

        public static void UpdateTabel(om zilier)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string cmnd = "update tabel_sezon set prezente ='" + zilier.prezente.ToString() + "' where nume ='" + zilier.nume.ToString() + "'" ;
                cnn.Execute(cmnd, zilier);
                cnn.Close();
            }
          
        }

        public static void UpdateTabel_nume(om zilier, string nume_nou)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string cmnd = "update tabel_sezon set nume ='" + nume_nou + "' where nume ='" + zilier.nume.ToString() + "'";
                cnn.Execute(cmnd, zilier);
                cnn.Close();
            }

        }

        public static void SaveZilier(om zilier)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into tabel_sezon (nume, prezente) values (@nume, @prezente)", zilier);
                cnn.Close();
            }
        }

        public static DateTime LastZi()
        {
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                
                SQLiteCommand command = cnn.CreateCommand();
                command.CommandText = "select Calendar from tabel_zi";
                cnn.Open();
                SQLiteDataReader rdr = command.ExecuteReader();

                string output = "";
                while (rdr.Read())
                {
                     output = rdr.GetString(0);
                }
                DateTime time = DateTime.Parse(output);
                cnn.Close();
                return time;               
            }
        }

        public static bool validZi(string s)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                SQLiteCommand command = cnn.CreateCommand();
                command.CommandText = "select Calendar from tabel_zi";
                cnn.Open();
                SQLiteDataReader rdr = command.ExecuteReader();

                string[] data = s.Split('-');
                s = data[0] + "/" + data[1] + "/" + data[2];

                string output = "";
                while (rdr.Read())
                {
                    output = rdr.GetString(0);
                    DateTime time = DateTime.Parse(output);
                    string zi_aux = time.ToString("dd/MM/yyyy");
                    if (String.Equals(zi_aux, s))
                        return true;
                }
                cnn.Close();
                return false;
            }
        }

        public static void UpdateCalendar(string s)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into tabel_zi (calendar) values ('"+ s + "')", s);
                cnn.Close();
            }
            
        }

        private static string LoadConnectionString(string id = "Conexiune")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
