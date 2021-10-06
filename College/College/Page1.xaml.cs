
using MongoDB.Driver;
using System;
using Xamarin.Essentials;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MongoDB.Driver.GridFS;
using System.IO;
using MongoDB.Bson;

namespace College
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        private static string uri = "mongodb+srv://kveeq:2554781@cluster0.lfe3e.mongodb.net/Base?retryWrites=true&w=majority";
        public static MongoClient client = new MongoClient(uri);
        static IMongoDatabase db = client.GetDatabase("Base");
        static IMongoCollection<Base> data = db.GetCollection<Base>("collection");
        public List<Base> lst;
        string Name;
        string link;
        string Surname;
        string Patronymic;

        public Page1()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            lst = data.AsQueryable().ToList<Base>();
            lst_view.ItemsSource = lst;
        }

        private void SplitName()
        {
            string[] arr = new string[3];
            arr = FIO.Text.ToString().Split(' ');
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

        private void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                SplitName();
                Base basa = new Base(Name, Surname, Patronymic, Date.Text, Activity.Text);
                Base.Add(basa);

                using (Stream fs = new FileStream(link, FileMode.Open))
                {
                    IGridFSBucket gridFS = new GridFSBucket(db);
                    SplitName();
                    ObjectId id = gridFS.UploadFromStream(Name + " " + Surname, fs);
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Не удалось добавить в базу", ex.Message, "OK");
            }
        }

        async void Button_Clicked_1(object sender, EventArgs e)
        {
            var pickResult = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Pick an image"
            });

            link = pickResult.FileName;
            if (pickResult != null)
            {
                if (FIO.Text != null && Date.Text != null && Activity.Text != null)
                {
                    var stream = await pickResult.OpenReadAsync();
                    resultImage.Source = ImageSource.FromStream(() => stream);
                    link = resultImage.Source.ToString();
                }

                else
                {
                    await DisplayAlert("Предупреждение", "Заполните текстовые данные", "OK");
                }
            }
        }
    }
}