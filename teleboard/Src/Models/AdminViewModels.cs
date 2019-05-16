using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Teleboard.Attributes;
using Teleboard.Common.Enum;
using Teleboard.DomainModel.Core;
using Teleboard.Localization.Attribute;
using Teleboard.Validation.Attribute;

namespace Teleboard.Models
{
    public class AddRoleViewModel
    {
        [RequiredField]
        [LocalizedName("Name")]
        public string Name { get; set; }

        [RequiredField]
        [LocalizedName("Description")]
        public string Description { get; set; }

    }

    public class GetRolesViewModel
    {
        public string Id { get; set; }

        [RequiredField]
        [LocalizedName("Name")]
        public string Name { get; set; }

        [RequiredField]
        [LocalizedName("Description")]
        public string Description { get; set; }
    }

    public class EditRoleViewModel
    {
        public string Id { get; set; }

        [RequiredField]
        [LocalizedName("Name")]
        public string Name { get; set; }

        [RequiredField]
        [LocalizedName("Description")]
        public string Description { get; set; }
    }

    public class GetUsersViewModel
    {
        public string Id { get; set; }

        [LocalizedName("Email")]
        public string Email { get; set; }

        [LocalizedName("First Name")]
        public string FirstName { get; set; }

        [LocalizedName("Last Name")]
        public string LastName { get; set; }

        [LocalizedName("Roles")]
        public string Roles { get; set; }

        [LocalizedName("Tenants")]
        public string Tenants { get; set; }
        public string Language { get; set; }
    }

    public class AddUserViewModel
    {
        [RequiredField]
        [EmailAddress]
        [LocalizedName("Email")]
        public string Email { get; set; }

        [RequiredField]
        [DataType(DataType.Text)]
        [LocalizedName("FirstName")]
        public string FirstName { get; set; }

        [RequiredField]
        [DataType(DataType.Text)]
        [LocalizedName("LastName")]
        public string LastName { get; set; }

        public IEnumerable<string> SelectedRoles { get; set; }

        public IEnumerable<string> AllRoles { get; set; }

        public IEnumerable<ApplicationRole> RoleInfos { get; set; }

        public IEnumerable<string> AllTenants { get; set; }

        public IEnumerable<string> SelectedTenants { get; set; }

        public IEnumerable<Tenant> TenantInfos { get; set; }

        [LocalizedName("Language")]
        public Language Language { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }

        [RequiredField]
        [DataType(DataType.EmailAddress)]
        [LocalizedName("Email")]
        public string Email { get; set; }

        [RequiredField]
        [DataType(DataType.Text)]
        [LocalizedName("FirstName")]
        public string FirstName { get; set; }

        [RequiredField]
        [DataType(DataType.Text)]
        [LocalizedName("LastName")]
        public string LastName { get; set; }

        public IEnumerable<string> SelectedRoles { get; set; }

        public IEnumerable<string> AllRoles { get; set; }

        public IEnumerable<ApplicationRole> RoleInfos { get; set; }

        public IEnumerable<string> AllTenants { get; set; }

        public IEnumerable<string> SelectedTenants { get; set; }

        public IEnumerable<Tenant> TenantInfos { get; set; }

        [LocalizedName("Language")]
        public Language Language { get; set; }
    }
}