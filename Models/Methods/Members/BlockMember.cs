using ApiPyrus.Models.DTOs;
using System.Threading.Tasks;

namespace ApiPyrus.Models.Methods.Members
{
    public class BlockMember
    {
        public BlockMember(long memberId)
            => _memberId = memberId;

        private long _memberId;

        /// <summary>
        /// Отправить запрос на удаление сотрудника.
        /// </summary>
        public async Task<ValuePersone> Send(ApiClient apiClient, string extRequestId = "")
            => await apiClient.BlockMember(_memberId, extRequestId);
    }
}
