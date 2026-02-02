
using Linq2OData.Core;
using Linq2OData.Core.Metadata;
using Microsoft.AspNetCore.Components;
using TabBlazor;
using TabBlazor.Services;

namespace Linq2OData.Docs.Components.Explorer;

public partial class ODataExplorer
{

    [Inject] private ToastService Toast { get; set; } = null!;
    [Inject] private IModalService Modal { get; set; } = null!;

    private bool isLoading;
    private ODataOptions odataOptions = new() { BaseUrl = "https://services.odata.org/V2/OData/OData.svc/" };
    private HttpClient? httpClient;
    private ODataMetadata? metadata;

    protected override async  Task OnInitializedAsync()
    {
        
      await ConnectAsync();


    }


    private async Task ConnectAsync()
    {

        if (string.IsNullOrWhiteSpace(odataOptions.BaseUrl))
        {
            return;
        }

        try
        {
            isLoading = true;

            var url = $"https://corsproxy.io/?url={odataOptions.BaseUrl}";


            var metadataUrl = $"https://corsproxy.io/?url={odataOptions.BaseUrl}$metadata";
            var sampleUrl = $"https://corsproxy.io/?url={odataOptions.BaseUrl}Products?$top=4";



            httpClient = new HttpClient() {  };
            var metadata = await httpClient.GetStringAsync(sampleUrl);
            this.metadata = MetadataParser.Parse(metadata);

        }
        catch (Exception ex)
        {

            var component = new RenderComponent<ExceptionModal>().Set(e => e.Exception, ex);
            var result = await Modal.ShowAsync("Error", component, new() { Size = ModalSize.Large });

            throw;

        }
        finally
        {
            isLoading = false;
        }







    }




}