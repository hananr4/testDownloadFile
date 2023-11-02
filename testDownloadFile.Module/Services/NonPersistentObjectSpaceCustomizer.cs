using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;

namespace testDownloadFile.Module.Services;

//Scoped service
public class NonPersistentObjectSpaceCustomizer : IObjectSpaceCustomizer
{
    private readonly IObjectSpaceProviderService objectSpaceProvider;
    private readonly IObjectSpaceCustomizerService objectSpaceCustomizerService;

    private Dictionary<Guid, NonPersistentBaseObject> ObjectCache { get; }

    public NonPersistentObjectSpaceCustomizer(
      NonPersistentObjectStorageService nonPersistentStorage,
      IObjectSpaceProviderService objectSpaceProvider,
      IObjectSpaceCustomizerService objectSpaceCustomizerService)
    {
        ObjectCache = nonPersistentStorage.ObjectCache;
        this.objectSpaceProvider = objectSpaceProvider;
        this.objectSpaceCustomizerService = objectSpaceCustomizerService;
    }

    public void OnObjectSpaceCreated(IObjectSpace objectSpace)
    {
        if (objectSpace is NonPersistentObjectSpace nonPersistentObjectSpace)
        {
            nonPersistentObjectSpace.ObjectsGetting += NonPersistentObjectSpace_ObjectsGetting;
            nonPersistentObjectSpace.ObjectByKeyGetting += NonPersistentObjectSpace_ObjectByKeyGetting;
            nonPersistentObjectSpace.Committing += NonPersistentObjectSpace_Committing;
            nonPersistentObjectSpace.PopulateAdditionalObjectSpaces(objectSpaceProvider, objectSpaceCustomizerService);
        }
    }

    private void NonPersistentObjectSpace_ObjectsGetting(object? sender, ObjectsGettingEventArgs e)
    {
        if (typeof(NonPersistentBaseObject).IsAssignableFrom(e.ObjectType))
        {
            ArgumentNullException.ThrowIfNull(sender);
            IObjectSpace objectSpace = (IObjectSpace)sender;
            var objects = new BindingList<NonPersistentBaseObject>()
            {
                AllowNew = true,
                AllowEdit = true,
                AllowRemove = true
            };
            foreach (var obj in ObjectCache.Values)
            {
                if (e.ObjectType.IsAssignableFrom(obj.GetType()))
                    objects.Add(objectSpace.GetObject(obj));
            }
            e.Objects = objects;
        }
    }
    private void NonPersistentObjectSpace_ObjectByKeyGetting(object? sender, ObjectByKeyGettingEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(sender);
        IObjectSpace objectSpace = (IObjectSpace)sender;
        if (typeof(NonPersistentBaseObject).IsAssignableFrom(e.ObjectType))
        {
            if (ObjectCache.TryGetValue((Guid)e.Key, out NonPersistentBaseObject obj))
            {
                e.Object = objectSpace.GetObject(obj);
            }
        }
    }
    private void NonPersistentObjectSpace_Committing(object? sender, CancelEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(sender);
        IObjectSpace objectSpace = (IObjectSpace)sender;
        foreach (var obj in objectSpace.ModifiedObjects)
        {
            if (obj is NonPersistentBaseObject nonPersistentObject)
            {
                if (objectSpace.IsNewObject(obj))
                {
                    ObjectCache.Add(nonPersistentObject.Oid, nonPersistentObject);
                }
                else if (objectSpace.IsDeletedObject(obj))
                {
                    ObjectCache.Remove(nonPersistentObject.Oid);
                }
            }
        }
    }
}
