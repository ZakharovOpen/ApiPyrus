using ApiPyrus.Models.DTOs;

namespace ApiPyrus.Models.Methods.Tasks
{
    public class ReopenTaskByForm : UpdateTaskByForm
    {
        public ReopenTaskByForm(long taskId) : base(taskId)
            => _entityForJson.Action = ActionTypes.Reopened.GetDescription();
    }
}
