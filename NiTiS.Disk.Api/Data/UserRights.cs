using System;

namespace NiTiS.Disk.Api.Data;

[Flags]
public enum UserRights : uint
{
	None = 0,
	AdminPanelAccess = 1u << 31,
	Admin =	0xFFFFFFFFu,
}