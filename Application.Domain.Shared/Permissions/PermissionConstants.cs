namespace Application.Domain.Shared.Permissions
{
    public static class PermissionConstants
    {
        public static readonly string[] All = new[]
        {
            Users_View,
            Users_Create,
            Users_Edit,
            Users_Delete,
            Roles_View,
            Roles_Create,
            Roles_Edit,
            Roles_Delete
        };

        public const string Users_View = "Users.View";
        public const string Users_Create = "Users.Create";
        public const string Users_Edit = "Users.Edit";
        public const string Users_Delete = "Users.Delete";

        public const string Roles_View = "Roles.View";
        public const string Roles_Create = "Roles.Create";
        public const string Roles_Edit = "Roles.Edit";
        public const string Roles_Delete = "Roles.Delete";
    }
}
