using ApiPyrus.Extentions;
using ApiPyrus.Models.DTOs;
using System.Threading.Tasks;

namespace ApiPyrus.Models.Methods.Members
{
    public class CreateMember
    {
        public CreateMember(string firstName, string lastName, string email, string position = null, long? departmentId = null, string skype = null, string phone = null)
        {
            _entityForJson = new PyrusRequestMember()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Position = position,
                DepartmentId = departmentId,
                Skype = skype,
                Phone = phone
            };
        }

        private PyrusRequestMember _entityForJson = new PyrusRequestMember();

        /// <summary>
        /// Отправить запрос на создание сотрудника.
        /// </summary>
        public async Task<ValuePersone> Send(ApiClient apiClient, string extRequestId = "")
            => await apiClient.AddMembers(_entityForJson.GetJson(), extRequestId);
    }
}
