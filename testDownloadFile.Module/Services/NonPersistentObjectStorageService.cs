using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.ExpressApp;

using testDownloadFile.Module.Parameters;

namespace testDownloadFile.Module.Services;


public class NonPersistentObjectStorageService
{
    public Dictionary<Guid, NonPersistentBaseObject> ObjectCache { get; } = new();

    public NonPersistentObjectStorageService()
    {
    }
    public NonPersistentBaseObject CreateObject<T>(string value) where T : NonPersistentBaseObject, new()
    {
        var result = new T();
        ObjectCache.Add(result.Oid, result);
        return result;
    }
}
