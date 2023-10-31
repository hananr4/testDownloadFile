﻿using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testDownloadFile.Module.Parameters;

[DomainComponent]
public class ExportXmlZipParameter: NonPersistentBaseObject
{

    FileData file;

    public ExportXmlZipParameter()
    {

    }
     
    public ExportXmlZipParameter(Guid oid) : base(oid)
    {

    }

    public FileData File
    {
        get => file;
        internal set
        {
            if (file == value)
                return;
            file = value;
            OnPropertyChanged(nameof(File));
        }
    }
    
}
