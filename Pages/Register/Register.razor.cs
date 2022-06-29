using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Net.Http;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Todo.Pages.Register
{
    public class Kayit
    {
        public string? KId { get; set; }
        public string? KAd { get; set; }
        public string? KSoy { get; set; }
        public string? KullaniciAdi { get; set; }
        public string? KPassword { get; set; }


    }
    public class RegisterBase : ComponentBase
    {
        static string connectionString = ("Server=.;Database=TodoDb;Encrypt=False;Integrated Security=SSPI;");

        static SqlConnection conn = new SqlConnection(connectionString);
        static string sql = "select KId,KAd,KSoy,KullaniciAdi,KPassword,KRole from Kullanici";
        static SqlDataAdapter daps = new SqlDataAdapter(sql, conn);
        SqlCommandBuilder cb = new SqlCommandBuilder(daps);
        DataSet dsps = new DataSet();
        public List<Kayit> people = new List<Kayit>();
        public string? error { get; set; }
        public string? inputSoyad { get; set; }
        public string? inputAd { get; set; }
        public string? inputKullaniciAdi { get; set; }
        public string? inputPassword { get; set; }



        public async Task InsertAccount()
        {

            if (inputAd != null && inputSoyad != null && inputKullaniciAdi != null && inputPassword != null)
            {

                dsps.Tables[0].Rows.Add(null, inputAd, inputSoyad, inputKullaniciAdi, inputPassword,null);
                daps.Update(dsps, "people");
                dsps.Tables["people"].Clear();
                inputAd = "";
                inputSoyad = "";
                inputKullaniciAdi = "";
                inputPassword = "";
                
            }
            else
            {

            }
            await OnInitializedAsync();

        }

        protected override async Task OnInitializedAsync()
        {
            people.Clear();
            await selectproc();
        }
        public Task selectproc()
        {
            return Task.Run(() =>
            {
                try
                {
                    daps.Fill(dsps, "people");
                    error += dsps.Tables[0].Rows.Count.ToString();
                    for (int i = 0; i < dsps.Tables[0].Rows.Count; i++)
                    {
                        Kayit pm = new Kayit();
                        pm.KId = dsps.Tables[0].Rows[i]["KId"].ToString();
                        pm.KAd = dsps.Tables[0].Rows[i]["KAd"].ToString();
                        pm.KSoy = dsps.Tables[0].Rows[i]["KSoy"].ToString();
                        pm.KullaniciAdi = dsps.Tables[0].Rows[i]["KullaniciAdi"].ToString();
                        pm.KPassword = dsps.Tables[0].Rows[i]["KPassword"].ToString();

                        people.Add(pm);

                    }
                }
                catch (Exception ex)
                {
                    error = ex.ToString();
                }
            });
        }

    }
}