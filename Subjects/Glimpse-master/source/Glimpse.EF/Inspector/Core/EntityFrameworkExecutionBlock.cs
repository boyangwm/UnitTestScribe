﻿using Glimpse.Core.Framework.Support;

namespace Glimpse.EF.Inspector.Core
{
    internal class EntityFrameworkExecutionBlock : ExecutionBlockBase
    {
        public static readonly EntityFrameworkExecutionBlock Instance = new EntityFrameworkExecutionBlock();

        private EntityFrameworkExecutionBlock()
        {
            RegisterProvider(new DbConnectionFactoriesExecutionTask(Logger));
#if EF6Plus
            RegisterProvider(new DbConfigurationExecutionTask());
#endif
        }
    }
}
