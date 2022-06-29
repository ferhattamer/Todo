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
using Todo.Data;
using System.Text.Json;

namespace Todo.Pages.Moderator
{

    public class TodoModel
    {
        public string? Id { get; set; }
        public string? TodoDetay { get; set; }
        public string? TodoDurum { get; set; }
        public string? TodoPersonel { get; set; }
        public string? TodoSDate { get; set; }
        public string? TodoFDate { get; set; }

    }
    public class PersonModel
    {
        public string? KId { get; set; }
        public string? KullaniciAdi { get; set; }
    }
    public class ModeratorBase : ComponentBase
    {
        private string? kullanici = "";
        [Inject]
        public ProtectedLocalStorage? localstr1 { get; set; }
        public string? Kullanici
        {
            get
            {
                KullaniciAdi();
                return kullanici;
            }
            set
            {
                kullanici = value;
            }
        }
        public string? mystr1 { get; set; }
        UserModel user = new UserModel();

        private async void KullaniciAdi()
        {
            var result = await localstr.GetAsync<string>("myUser");
            mystr = result.Success ? result.Value : "";
            UserModel user = new UserModel();
            user = JsonSerializer.Deserialize<UserModel>(mystr);
            kullanici = user.UserName;

        }
        static string connectionString = ("Server=.;Database=TodoDb;Encrypt=False;Integrated Security=SSPI;");
        static string connectionStringPerson = ("Server=.;Database=TodoDb;Encrypt=False;Integrated Security=SSPI;");
        static SqlConnection conn = new SqlConnection(connectionString);
        static SqlConnection connPerson = new SqlConnection(connectionStringPerson);
        SqlCommandBuilder cb = new SqlCommandBuilder(daps);
        SqlCommandBuilder cbPerson = new SqlCommandBuilder(dapsPerson);

        static string sql = "select TodoId,TodoDetay,TodoDurum,TodoPersonel,TodoSDate,TodoFDate from Todo";
        static string sqlPerson = "select KId,KullaniciAdi from Kullanici";
        static SqlDataAdapter dapsPerson = new SqlDataAdapter(sqlPerson, connPerson);
        static SqlDataAdapter daps = new SqlDataAdapter(sql, conn);
        DataSet dsps = new DataSet();
        DataSet dspsPerson = new DataSet();
        public List<TodoModel> people = new List<TodoModel>();
        public List<PersonModel> person = new List<PersonModel>();
        public string? error { get; set; }
        public string? WhoLogin { get; set; }
        public string? error2 { get; set; }
        public string? inputId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public string? inputDetay { get; set; }
        public string? inputDurum { get; set; }
        public string? inputPersonel { get; set; }
        public string? inputYorumsahibi { get; set; }

        private string? kontrol = "";
        [Inject]
        public ProtectedLocalStorage? localstr { get; set; }
        public string? Kontrol
        {
            get
            {
                AdminModerator();
                return kontrol;
            }
            set
            {
                kontrol = value;
            }
        }
        public string? mystr { get; set; }
        [Inject]
        protected NavigationManager? Navigation { get; set; }

        private async void AdminModerator()
        {
            var result = await localstr.GetAsync<string>("myUser");
            mystr = result.Success ? result.Value : "";
            UserModel user = new UserModel();
            user = JsonSerializer.Deserialize<UserModel>(mystr);
            WhoLogin = user.Role;
            if (user.Role == "Admin" || user.Role == "Moderator")
                Navigation.NavigateTo("/mod");
            else
                Navigation.NavigateTo("/Giris");
        }

        public async Task InsertData()
        {
            dsps.Tables[0].Rows.Add(null, inputDetay, inputDurum, inputPersonel, StartDate, FinishDate);
            daps.Update(dsps, "people");
            dspsPerson.Tables["person"].Clear();
            dsps.Tables["people"].Clear();
            //cb.Dispose();
            inputDetay = "";
            inputDurum = "";
            inputPersonel = "";
            await OnInitializedAsync();
        }
        public async Task GetData()
        {
            DataRowCollection itemColumns = dsps.Tables[0].Rows;
            for (int i = 0; i < dsps.Tables[0].Rows.Count; i++)
            {
                if (dsps.Tables[0].Rows[i]["TodoId"].ToString() == inputId)
                {
                    inputDetay = dsps.Tables[0].Rows[i]["TodoDetay"].ToString();
                    inputDurum = dsps.Tables[0].Rows[i]["TodoDurum"].ToString();
                    inputPersonel = dsps.Tables[0].Rows[i]["TodoPersonel"].ToString();

                }
            }
            dsps.Tables[0].Clear();
            dspsPerson.Tables[0].Clear();
            await OnInitializedAsync();
        }
        public async Task UpdateData()
        {
            DataRowCollection itemColumns = dsps.Tables[0].Rows;
            for (int i = 0; i < dsps.Tables[0].Rows.Count; i++)
            {
                if (dsps.Tables[0].Rows[i]["TodoId"].ToString() == inputId)
                {
                    dsps.Tables[0].Rows[i]["TodoDetay"] = inputDetay;
                    dsps.Tables[0].Rows[i]["TodoDurum"] = inputDurum;
                    dsps.Tables[0].Rows[i]["TodoPersonel"] = inputPersonel;
                    dsps.Tables[0].Rows[i]["TodoSDate"] = StartDate;
                    dsps.Tables[0].Rows[i]["TodoFDate"] = FinishDate;
                }
            }
            daps.Update(dsps, "people");
            dsps.Tables[0].Clear();
            dspsPerson.Tables[0].Clear();
            inputId = "";
            inputDetay = "";
            inputDurum = "";
            inputPersonel = "";
            await OnInitializedAsync();
        }
        public async Task DeleteData()
        {
            foreach (DataRow row in dsps.Tables[0].Rows)
            {
                if (row["Id"].ToString() == inputId)

                    row.Delete();
            }
            daps.Update(dsps, "people");
            dsps.Tables["people"].Clear();
            dspsPerson.Tables["person"].Clear();
            inputId = "";
            inputDetay = "";
            inputDurum = "";
            await OnInitializedAsync();

        }
        protected override async Task OnInitializedAsync()
        {
            people.Clear();
            person.Clear();
            await selectproc();
            await selectprocPerson();
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
                        TodoModel pm = new TodoModel();
                        pm.Id = dsps.Tables[0].Rows[i]["TodoId"].ToString();
                        pm.TodoDetay = dsps.Tables[0].Rows[i]["TodoDetay"].ToString();
                        pm.TodoDurum = dsps.Tables[0].Rows[i]["TodoDurum"].ToString();
                        pm.TodoPersonel = dsps.Tables[0].Rows[i]["TodoPersonel"].ToString();
                        pm.TodoSDate = dsps.Tables[0].Rows[i]["TodoSDate"].ToString();
                        pm.TodoFDate = dsps.Tables[0].Rows[i]["TodoFDate"].ToString();

                        people.Add(pm);

                    }
                }
                catch (Exception ex)
                {
                    error = ex.ToString();
                }
            });
        }
        public Task selectprocPerson()
        {
            return Task.Run(() =>
            {
                try
                {
                    dapsPerson.Fill(dspsPerson, "person");
                    error2 += dspsPerson.Tables[0].Rows.Count.ToString();
                    for (int i = 0; i < dspsPerson.Tables[0].Rows.Count; i++)
                    {
                        PersonModel xm = new PersonModel();
                        xm.KId = dspsPerson.Tables[0].Rows[i]["KId"].ToString();
                        xm.KullaniciAdi = dspsPerson.Tables[0].Rows[i]["KullaniciAdi"].ToString();

                        person.Add(xm);
                    }

                }
                catch (Exception ex)
                {
                    error2 = ex.ToString();
                }
            });
        }
    }
}