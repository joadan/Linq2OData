using Linq2OData.Generator;
using Linq2OData.Generator.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.IO.Compression;
using System.Text;
using TabBlazor;
using TabBlazor.Services;

namespace Linq2OData.Docs.Components
{
    public partial class GenerateClient
    {
        // private readonly List<GenerateOptions> demoOptions = new();
        private bool isLoading;


        private ClientRequest request = new ClientRequest { Name = "MyClient", Namespace = "MyNamespace" };

        [Inject] private TablerService tablerService { get; set; } =  default!;
        [Inject] private ToastService toastService { get; set; } = default!;
        [Inject] private IModalService modalService { get; set; } = default!;

        protected override void OnInitialized()
        {
            Refresh();
        }

        private void Refresh()
        {
            request = new ClientRequest { Name = "MyClient", Namespace = "MyNamespace", MetadataList = [] };
        }

        private async Task LoadFilesAsync(InputFileChangeEventArgs e)
        {
            const long maxSize = 10 * 1024 * 1024; // must be >= file size
            const int bufferSize = 8 * 1024;      // 8 KB

            foreach (var file in e.GetMultipleFiles(10))
            {

                if(file.Size > maxSize)
                {
                    await toastService.AddToastAsync(new()
                    {
                        Title = "File Too Large",
                        Message = $"{file.Name} exceeds the [10] MB size limit and will be skipped.",
                         SubTitle = "Please upload a smaller file."
                         
                    });
                    continue;
                }

               
                using var stream = file.OpenReadStream(maxSize);
                using var reader = new StreamReader(stream);

                var sb = new StringBuilder((int)Math.Min(file.Size, int.MaxValue));
                char[] buffer = new char[bufferSize];

                int read;
                while ((read = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    sb.Append(buffer, 0, read);

                    // optional: keep UI responsive
                    await Task.Yield();
                }

                //string xml = sb.ToString();

                request.MetadataList.Add(sb.ToString());
            }
        }

        private async Task SaveEntriesAsync(List<FileEntry> entries)
        {
            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var entry in entries)
                {
                    var file = archive.CreateEntry("Generated/" + entry.FolderPath + "/" + entry.FileName);
                    using var entryStream = file.Open();
                    using var streamWriter = new StreamWriter(entryStream);
                    streamWriter.Write(entry.Content);
                }
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            await tablerService.SaveAsBinary($"{request.Name}.zip", "application/zip", memoryStream.ToArray());

            await toastService.AddToastAsync(new()
            {
                Title = "Generate Complete",
                Message = $"{request.Name}.zip has been been created! Please check you downloads."
            });
        }

        private async Task GenerateClientJson()
        {
            try
            {
                isLoading = true;
                var generator = new ClientGenerator(request);
                var entries = generator.GenerateClient();
                await SaveEntriesAsync(entries);
            }
            catch (Exception ex)
            {
                var component = new RenderComponent<ExceptionModal>().Set(e => e.Exception, ex);
                var result = await modalService.ShowAsync("Error", component, new() { Size = ModalSize.Large });
            }
            finally
            {
                isLoading = false;
            }
        }


        private async Task GenerateClientAsync()
        {
            try
            {
                isLoading = true;
                var generator = new ClientGenerator(request);
                var entries = generator.GenerateClient();
                await SaveEntriesAsync(entries);
            }
            catch (Exception ex)
            {
                var component = new RenderComponent<ExceptionModal>().Set(e => e.Exception, ex);
                var result = await modalService.ShowAsync("Error", component, new() { Size = ModalSize.Large });
            }
            finally
            {
                isLoading = false;
            }
        }
    }
}
