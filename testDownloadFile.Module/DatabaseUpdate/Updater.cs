using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using testDownloadFile.Module.BusinessObjects;
using System.Text;

namespace testDownloadFile.Module.DatabaseUpdate;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Updating.ModuleUpdater
public class Updater : ModuleUpdater {
    public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
        base(objectSpace, currentDBVersion) {
    }
    public override void UpdateDatabaseAfterUpdateSchema() {
        base.UpdateDatabaseAfterUpdateSchema();
        
        var xos = (XPObjectSpace)ObjectSpace;
        DocumentLibrary doc = xos.FirstOrDefault<DocumentLibrary>(d=>true);
        if(doc == null)
        {
            CreateDocument(xos, "test description 1", "test1.txt");
            CreateDocument(xos, "test description 2", "test2.txt");
            CreateDocument(xos, "test description 3", "test3.txt");
        }

        xos.CommitChanges(); 
    }

    private static void CreateDocument(XPObjectSpace xos, string description, string filename)
    {
        DocumentLibrary doc = new DocumentLibrary(xos.Session)
        {
            Description = description,
            File = new FileData(xos.Session)
        };
        doc.File.LoadFromStream(filename, new MemoryStream(Encoding.UTF8.GetBytes($"{filename} - {description}")));

        doc.Save();
    }

    public override void UpdateDatabaseBeforeUpdateSchema() {
        base.UpdateDatabaseBeforeUpdateSchema();
        //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
        //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
        //}
    }
}
