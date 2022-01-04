using System;

namespace MediaVC
{
    public struct ExternalLoopController : IExternalLoopController
    {
        public ExternalLoopController(ExternalLoopControllerSource factory) =>
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));

        public ExternalLoopController() => this.factory = null;

        private readonly ExternalLoopControllerSource? factory;

        public void Break()
        {
            if(this.factory is not null)
                this.factory.IsBreakRequested = true;
        }
    }
}
