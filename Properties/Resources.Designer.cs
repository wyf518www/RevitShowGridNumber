using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace ShowGridNumber.Properties
{
	// Token: 0x02000013 RID: 19
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[CompilerGenerated]
	[DebuggerNonUserCode]
	internal class Resources
	{
		// Token: 0x06000066 RID: 102 RVA: 0x000038E7 File Offset: 0x00001AE7
		internal Resources()
		{
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000067 RID: 103 RVA: 0x000038F0 File Offset: 0x00001AF0
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(Resources.resourceMan, null))
				{
					ResourceManager resourceManager = new ResourceManager("ShowGridNumber.Properties.Resources", typeof(Resources).Assembly);
					Resources.resourceMan = resourceManager;
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000068 RID: 104 RVA: 0x0000392F File Offset: 0x00001B2F
		// (set) Token: 0x06000069 RID: 105 RVA: 0x00003936 File Offset: 0x00001B36
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600006A RID: 106 RVA: 0x00003940 File Offset: 0x00001B40
		internal static Bitmap cirl
		{
			get
			{
				object @object = Resources.ResourceManager.GetObject("cirl", Resources.resourceCulture);
				return (Bitmap)@object;
			}
		}

		// Token: 0x0400005F RID: 95
		private static ResourceManager resourceMan;

		// Token: 0x04000060 RID: 96
		private static CultureInfo resourceCulture;
	}
}
