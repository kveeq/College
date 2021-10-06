
/*
* !!!!!!!!!!!!!!!!!!!!!!!! Проверить NavigationPage !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
* изменен путь к подключению на хостинг (11293)
*/

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace College
{
    public partial class MainPage : ContentPage
    {
        private static string uri = "mongodb+srv://kveeq:2554781@cluster0.lfe3e.mongodb.net/Base?retryWrites=true&w=majority";
        public static MongoClient client = new MongoClient(uri); //11293
        static IMongoDatabase db = client.GetDatabase("Base");
        static IMongoCollection<Base> data = db.GetCollection<Base>("collection");
        public List<Base> lst;
        static string path = Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        public string link;
        string Name;
        string Surname;
        string Patronymic;
        bool b;
        string fname;
        int x = 0;

        public void GetUnivPath()
        {
            // error in UWP
            if (!File.Exists($@"{path}/2021-03-23.png"))
            {
                File.Create($@"{path}/2021-03-23.png");
                link = $@"{path}/2021-03-23.png";
            }
            else
            {
                link = $@"{path}/2021-03-23.png";
            }
        }

        public MainPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            lst = data.AsQueryable().ToList<Base>();
            GetUnivPath();
            lbl_activitiess.Text = lst[x].Activities;
            txt_birthday.Text = lst[x].BirthDay;
            JoinedName.Text = lst[x].Name + " " + lst[x].SurName + " " + lst[x].Patronymic;
            fname = lst[x].Name + " " + lst[x].SurName;
            ChoosingImage();
        }

        private void Prev_Clicked(object sender, EventArgs e)
        {
            if (x - 1 >= 0)
            {
                x--;
                lbl_activitiess.Text = lst[x].Activities;
                txt_birthday.Text = lst[x].BirthDay;
                JoinedName.Text = lst[x].Name + " " + lst[x].SurName + " " + lst[x].Patronymic;
                fname = lst[x].Name + " " + lst[x].SurName;
                ChoosingImage();
            }
            else
            {
                x = lst.Count - 1;
                lbl_activitiess.Text = lst[x].Activities;
                txt_birthday.Text = lst[x].BirthDay;
                JoinedName.Text = lst[x].Name + " " + lst[x].SurName + " " + lst[x].Patronymic;
                fname = lst[x].Name + " " + lst[x].SurName;
                ChoosingImage();
            }
        }


        private void Next_Clicked(object sender, EventArgs e)
        {
            prgbar.IsVisible = true;
            if (x + 1 <= lst.Count - 1)
            {
                x++;
                lbl_activitiess.Text = lst[x].Activities;
                txt_birthday.Text = lst[x].BirthDay;
                JoinedName.Text = lst[x].Name + " " + lst[x].SurName + " " + lst[x].Patronymic;
                fname = lst[x].Name + " " + lst[x].SurName;
                ChoosingImage();
            }
            else
            {
                x = 0;
                lbl_activitiess.Text = lst[x].Activities;
                txt_birthday.Text = lst[x].BirthDay;
                JoinedName.Text = lst[x].Name + " " + lst[x].SurName + " " + lst[x].Patronymic;
                fname = lst[x].Name + " " + lst[x].SurName;
            }
            ChoosingImage();
        }

        private void ChoosingImage()
        {
            IGridFSBucket gridFS = new GridFSBucket(db);
            using (Stream fs = new FileStream(link, FileMode.Open))
            {
                gridFS.DownloadToStreamByName(fname, fs);
                img.Source = link;
            }
        }

        private void SplitName(Entry txtFind)
        {
            txtFind.Text += " ";
            string[] arr = new string[3];
            arr = txtFind.Text.ToString().Split(' ');
            try
            {
                Name = arr[0];
                Surname = arr[1];
                Patronymic = arr[2];
            }
            catch
            {
                DisplayAlert("Внимание", "Введите корректное ФИО", "OK");
            }
        }

        private void JoinName()
        {
            // изменения
            JoinedName.Text = $"{Surname} {Name} {Patronymic}";
        }

        private void find_Clicked(object sender, EventArgs e)
        {
            b = false;
            SplitName(name);
            foreach (var f in lst)
            {
                if (f.Name == Name && f.SurName == Surname && f.Patronymic == Patronymic)
                {
                    Name = f.Name;
                    Surname = f.SurName;
                    Patronymic = f.Patronymic;
                    txt_birthday.Text = f.BirthDay;
                    lbl_activitiess.Text = f.Activities;
                    JoinName();
                    fname = Name + " " + Surname;
                    ChoosingImage();
                    b = true;
                }
            }
            if (b == false)
                DisplayAlert("Внимание", "Человек с таким ФИО не найден", "OK");
            b = false;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            await Navigation.PushModalAsync(new Page1());
        }
    }

    public class Base
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId _id { get; set; }
        [BsonElement]
        public string Name { get; set; }
        [BsonElement]
        public string SurName { get; set; }
        [BsonElement]
        public string Patronymic { get; set; }
        [BsonElement]
        public string BirthDay { get; set; }
        [BsonElement]
        public string Activities { get; set; }

        public Base(string name, string surname, string patronymic, string birthday, string activities)
        {
            Name = name;
            SurName = surname;
            Patronymic = patronymic;
            BirthDay = birthday;
            Activities = activities;
        }
        public static async void Add(Base basa)
        {
            var db = MainPage.client.GetDatabase("Base");
            var collection = db.GetCollection<Base>("collection");

            await collection.InsertOneAsync(basa);
        }
        public static List<Base> TakeList()
        {
            var db = MainPage.client.GetDatabase("Base");
            var collection = db.GetCollection<Base>("collection");
            return collection.Find(x => true).ToList();
        }
    }
}

