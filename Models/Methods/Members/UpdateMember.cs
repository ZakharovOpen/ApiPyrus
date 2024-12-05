using ApiPyrus.Extentions;
using ApiPyrus.Models.DTOs;
using System.Threading.Tasks;

namespace ApiPyrus.Models.Methods.Members
{
    public class UpdateMember
    {
        public UpdateMember(long memberId, string firstName = null, string lastName = null, string email = null, string position = null, long? departmentId = null, string skype = null, string phone = null, bool? banned = null)
        {
            _memberId = memberId;
            if (firstName != null) _entityForJson.FirstName = firstName;
            if (lastName != null) _entityForJson.LastName = lastName;
            if (email != null) _entityForJson.Email = email;
            if (position != null) _entityForJson.Position = position;
            if (departmentId != null) _entityForJson.DepartmentId = departmentId.Value;
            if (skype != null) _entityForJson.Skype = skype;
            if (phone != null) _entityForJson.Phone = phone;
            if (banned != null) _entityForJson.Banned = banned.Value;
        }

        private PyrusRequestMember _entityForJson = new PyrusRequestMember();
        private long _memberId;

        /// <summary>
        /// Отправить запрос на изменение сотрудника.
        /// </summary>
        public async Task<ValuePersone> Send(ApiClient apiClient, string extRequestId = "")
            => await apiClient.UpdateMembers(_memberId, _entityForJson.GetJson(), extRequestId);

    }
}
