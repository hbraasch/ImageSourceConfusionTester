using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSourceConfusionTester
{
    internal class StartupPage : ContentPage
    {
        Image image1;
        Image image2;
        public StartupPage()
        {
            image1 = new Image { HeightRequest = 100, WidthRequest = 100 };
            image2 = new Image { HeightRequest = 100, WidthRequest = 100 };

            Content = new VerticalStackLayout { Children = { image1, image2 } };
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            var commonFilename = "common_name_image.png";

            // Set image1 source simply to "common_name_image.png"
            image1.Source = commonFilename;

            // Ensure there is a image in the file system to be displayed using [ImageSource.FromFile]
            string targetFullFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, commonFilename);
            WriteMauiAssetToFile("file_system_file.png", targetFullFile);

            // Set image2 source to "[AppDataDirectoryPathname]/common_name_image.png". Notice, the filename is the same as that for image1
            image2.Source = ImageSource.FromFile(targetFullFile); 

        }

        private static Task WriteMauiAssetToFile(string sourceFilename, string destinationFullFileName)
        {
            var tcs = new TaskCompletionSource();
            Task.Factory.StartNew(async () => {
                try
                {
                    using (var resource = await FileSystem.Current.OpenAppPackageFileAsync(sourceFilename))
                    {
                        using (var file = new FileStream(destinationFullFileName, FileMode.Create, FileAccess.Write))
                        {
                            resource.CopyTo(file);
                        }
                    }
                    tcs.SetResult();
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
        }
    }
}
