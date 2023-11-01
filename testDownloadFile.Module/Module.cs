using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using testDownloadFile.Module.Parameters;

namespace testDownloadFile.Module;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
public sealed class testDownloadFileModule : ModuleBase {
    public testDownloadFileModule() {
		// 
		// testDownloadFileModule
		// 
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule));
    }
    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
        ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
        return new ModuleUpdater[] { updater };
    }
    public override void Setup(XafApplication application) {
        base.Setup(application);
        application.SetupComplete += Application_SetupComplete;
    }
    private void Application_SetupComplete(object sender, EventArgs e)
    {
        Application.ObjectSpaceCreated += Application_ObjectSpaceCreated;
    }
    private void Application_ObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs e)
    {
        if (e.ObjectSpace is NonPersistentObjectSpace nonPersistentObjectSpace)
        {
            nonPersistentObjectSpace.ObjectsGetting += NonPersistentObjectSpace_ObjectsGetting;
            nonPersistentObjectSpace.ObjectByKeyGetting += NonPersistentObjectSpace_ObjectByKeyGetting;
            nonPersistentObjectSpace.Committing += NonPersistentObjectSpace_Committing;
        }
    }

    private void NonPersistentObjectSpace_ObjectsGetting(Object sender, ObjectsGettingEventArgs e)
    {
        if (e.ObjectType == typeof(ExportXmlZipParameter))
        {
            IObjectSpace objectSpace = (IObjectSpace)sender;
            var objects = new BindingList<ExportXmlZipParameter>
            {
                AllowNew = true,
                AllowEdit = true,
                AllowRemove = true
            };
            foreach (ExportXmlZipParameter obj in ObjectsCache.Values)
            {
                objects.Add(objectSpace.GetObject<ExportXmlZipParameter>(obj));
            }
            e.Objects = objects;
        }
    }
    private void NonPersistentObjectSpace_ObjectByKeyGetting(object sender, ObjectByKeyGettingEventArgs e)
    {
        IObjectSpace objectSpace = (IObjectSpace)sender;
        if (e.ObjectType == typeof(ExportXmlZipParameter))
        {
            if (ObjectsCache.TryGetValue((Guid)e.Key, out ExportXmlZipParameter obj))
            {
                e.Object = objectSpace.GetObject(obj);
            }
        }
    }
    private void NonPersistentObjectSpace_Committing(Object sender, CancelEventArgs e)
    {
        IObjectSpace objectSpace = (IObjectSpace)sender;
        foreach (Object obj in objectSpace.ModifiedObjects)
        {
            ExportXmlZipParameter myobj = obj as ExportXmlZipParameter;
            if (obj != null)
            {
                if (objectSpace.IsNewObject(obj))
                {
                    ObjectsCache.Add(myobj.Oid, myobj);
                }
                else if (objectSpace.IsDeletedObject(obj))
                {
                    ObjectsCache.Remove(myobj.Oid);
                }
            }
        }
    }

    public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
        base.CustomizeTypesInfo(typesInfo);
        CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
    }


    private static Dictionary<Guid, ExportXmlZipParameter> ObjectsCache { get; }
        = new Dictionary<Guid, ExportXmlZipParameter>();

}
