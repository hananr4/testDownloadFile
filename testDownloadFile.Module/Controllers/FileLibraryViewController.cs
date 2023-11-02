using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using testDownloadFile.Module.BusinessObjects;
using testDownloadFile.Module.Parameters;

namespace testDownloadFile.Module.Controllers;

public partial class FileLibraryViewController : ViewController
{
    PopupWindowShowAction ZipDownload;
    public FileLibraryViewController()
    {
        InitializeComponent();
        ZipDownload = new PopupWindowShowAction(this, "ZipDownloadAction", "View") { 
            TargetViewType = ViewType.ListView,
            Caption = "Download Zip",
            TargetObjectType = typeof(DocumentLibrary)
        };
        ZipDownload.CustomizePopupWindowParams += ZipDownload_CustomizePopupWindowParams;
        
    }

    private void ZipDownload_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
    {
        var files = new List<InMemoryFile>();
        if (View.SelectedObjects.Count <= 0)
            throw new UserFriendlyException("Seleccione un comprobante");


        foreach (DocumentLibrary item in View.SelectedObjects )
        {
            files.Add(new InMemoryFile()
            {
                FileName = item.File.FileName,
                Content = item.File.Content
            });
        }

        var contentZip = HelperZip.GetZipArchive(files);
        
        var os = Application.CreateObjectSpace(typeof(ExportZipParameter));

        var os2 = Application.CreateObjectSpace(typeof(FileData));

        var param = os.CreateObject<ExportZipParameter>();
        var file = os2.CreateObject<FileData>();

        var filename = "documents.zip";
        using var stream = new MemoryStream(contentZip);
        file.LoadFromStream(filename, stream);
        param.File = file;
        os.CommitChanges();

        

        var view = Application.CreateDetailView(os, param);
        view.ViewEditMode = ViewEditMode.View;
        e.View = view;
        e.DialogController.CancelAction.Active.SetItemValue("desactivarCancelar", false);
        view.Caption = "Export";
    }

    protected override void OnActivated()
    {
        base.OnActivated();
    }
    protected override void OnViewControlsCreated()
    {
        base.OnViewControlsCreated();
    }
    protected override void OnDeactivated()
    {
        base.OnDeactivated();
    }
}
