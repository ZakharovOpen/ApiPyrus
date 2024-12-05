using ApiPyrus.Extentions;
using ApiPyrus.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiPyrus.Models.Methods.Roles
{
    public class CreateRole
    {
        public CreateRole(string roleName, List<long> membersIds)
        {
            _entityForJson = new PyrusRequestRole()
            {
                Name = roleName,
                AddMember = membersIds
            };
        }

        private PyrusRequestRole _entityForJson = new PyrusRequestRole();

        /// <summary>
        /// Отправить запрос на создание сотрудника.
        /// </summary>
        public async Task<Role> Send(ApiClient apiClient, string extRequestId = "")
            => await apiClient.CreateRole(_entityForJson.GetJson(), extRequestId);

    }
}
