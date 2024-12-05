using ApiPyrus.Extentions;
using ApiPyrus.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiPyrus.Models.Methods.Roles
{
    public class UpdateRole
    {
        public UpdateRole(long roleId, string roleName = null, List<long> addMembersIds = null, List<long> removeMembersIds = null, bool banned = false)
        {
            _roleId = roleId;
            _entityForJson = new PyrusRequestRole()
            {
                Name = roleName,
                AddMember = addMembersIds,
                RemoveMember = removeMembersIds,
                Banned = banned
            };
        }

        private PyrusRequestRole _entityForJson = new PyrusRequestRole();
        private long _roleId;

        /// <summary>
        /// Отправить запрос на создание сотрудника.
        /// </summary>
        public async Task<Role> Send(ApiClient apiClient, string request = "")
            => await apiClient.UpdateRole(_roleId, _entityForJson.GetJson(), request);

    }
}
