using ApiPyrus.Models.DTOs;

namespace ApiPyrus.Models.Methods.Tasks
{
    public class CloseTaskByForm : UpdateTaskByForm
    {
        public CloseTaskByForm(long taskId) : base(taskId)
           => _entityForJson.Action = ActionTypes.Finished.GetDescription();
    }
}
