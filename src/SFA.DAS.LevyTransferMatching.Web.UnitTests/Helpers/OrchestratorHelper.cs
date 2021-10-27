using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Moq;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Helpers
{
    public static class OrchestratorHelper
    {
        public static void SetupOrchestrator<TModel, TInterface>(this Mock<TInterface> orchestrator, Expression<Func<TInterface, Task<TModel>>> methodExpression, TModel modelToReturn)
            where TInterface : class
        {
            orchestrator.Setup(methodExpression).ReturnsAsync(() => modelToReturn);
        }
    }
}
