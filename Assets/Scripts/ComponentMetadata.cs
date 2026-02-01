using System;
using System.Collections.Generic;
using UnityEngine;



[Serializable] public class ComponentMetadata { public string id; public string type; public string parentCabinet; public string name; public string description; public string imagePath; public string ecadPath; }

[Serializable] public class ComponentMetadataList { public List<ComponentMetadata>components; }